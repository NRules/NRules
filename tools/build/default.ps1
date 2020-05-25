param (
    [hashtable] $component
)

properties {
    $version = $null
    $sdkVersion = "2.1.4"
    $nugetVersion = "4.9.4"
    $configuration = "Release"
    $baseDir = $null
}

include .\buildutils.ps1

task default -depends Build

task Init {
    Assert ($version -ne $null) 'Version should not be null'
    Assert ($component -ne $null) 'Component should not be null'
    Assert ($baseDir -ne $null) 'Base directory should not be null'
    
    Write-Host "Building $($component.name) version $version ($configuration)" -ForegroundColor Green
    
    $compName = $component.name
    $srcRoot = if ($component.ContainsKey('src_root')) { $component.src_root } else { 'src' }
    
    $script:binariesDir = "$baseDir\binaries\$compName"
    $script:srcRootDir = "$baseDir\$srcRoot"
    $script:srcDir = "$srcRootDir\$compName"
    $script:buildDir = "$baseDir\build"
    $script:pkgOutDir = "$buildDir\packages\$compName"
    $script:packagesDir = "$baseDir\packages"
    $script:toolsDir = "$baseDir\tools"
    
    $frameworkRoot = Get-RegistryValue 'HKLM:\SOFTWARE\Microsoft\.NETFramework\' 'InstallRoot' 
    $frameworkRoot = $frameworkRoot + "v4.0.30319"
    $script:msbuild = $frameworkRoot + "\msbuild.exe"
    
    Install-DotNetCli $toolsDir\.dotnet $sdkVersion
    Install-NuGet $toolsDir\.nuget $nugetVersion
}

task Clean -depends Init {
    Delete-Directory $pkgOutDir
    Delete-Directory $binariesDir
}

task PatchFiles {
    Update-Version $baseDir $version
    
    $signingKey = "$baseDir\SigningKey.snk"
    $secureKey = "$baseDir\..\SecureSigningKey.snk"
    $secureHash = "$baseDir\..\SecureSigningKey.sha1"
    
    if ((Test-Path $secureKey) -and (Test-Path $secureHash)) {
        Write-Host "Using secure signing key." -ForegroundColor Magenta
        $publicKey = Get-Content $secureHash
        $publicKey = $publicKey.Trim()
        Update-InternalsVisible $baseDir $publicKey
        Copy-Item $secureKey -Destination $signingKey -Force
    } else {
        Write-Host "Secure signing key does not exist. Using development key." -ForegroundColor Yellow
    }
}

task ResetPatch {
    Reset-Version $baseDir
    
    $signingKey = "$baseDir\SigningKey.snk"
    $devKey = "$baseDir\DevSigningKey.snk"
    $devHash = "$baseDir\DevSigningKey.sha1"
    
    $publicKey = Get-Content $devHash
    $publicKey = $publicKey.Trim()
    Update-InternalsVisible $baseDir $publicKey
    Copy-Item $devKey -Destination $signingKey -Force
}

task RestoreDependencies -precondition { return $component.ContainsKey('restore') } {
    if ($component.restore.tool -eq 'nuget') {
        exec { nuget restore $srcDir -NonInteractive }
    }
    if ($component.restore.tool -eq 'dotnet') {
        exec { dotnet restore $srcDir --verbosity minimal }
    }
}

task Compile -depends Init, Clean, PatchFiles, RestoreDependencies -precondition { return $component.ContainsKey('build') } { 
    Create-Directory $buildDir
    
    $solutionFile = "$srcDir\$($component.name).sln"
    if ($component.build.ContainsKey('solution_file')) {
        $solutionFile = "$srcRootDir\$($component.build.solution_file)"
    }
    
    if ($component.build.tool -eq 'msbuild') {
        exec { &$msbuild $solutionFile /p:Configuration=$configuration /v:m /nologo }
    }
    if ($component.build.tool -eq 'dotnet') {
        exec { dotnet build $solutionFile --configuration $configuration --verbosity minimal }
    }
    if ($component.build.tool -eq 'shfb') {
        Assert (Test-Path Env:\SHFBROOT) 'Sandcastle root environment variable SHFBROOT is not set'
        
        exec { &$msbuild $solutionFile /v:m /nologo }
    }
}

task Test -depends Compile -precondition { return $component.ContainsKey('test') } {
    $testsDir = "$srcDir\$($component.test.location)"
    Get-ChildItem $testsDir -recurse -filter "*.trx" | % { Delete-File $_.fullname }
    
    $projects = Get-DotNetProjects $testsDir
    foreach ($project in $projects) {
        Push-Location $project
        $projectName = Split-Path $project -Leaf
        foreach ($framework in $component.test.frameworks) {
            $result = "$($projectName)_$framework.trx"
            exec { dotnet test --no-build --configuration $configuration --framework $framework --verbosity minimal --logger "trx;LogFileName=$result" }
        }
        Pop-Location
    }
    
    if (Test-Path Env:CI) {
        Write-Host "Uploading test results to CI server"
        $wc = New-Object 'System.Net.WebClient'
        Get-ChildItem $testsDir -recurse -filter "*.trx" | % {
            $testFile = $_.fullname
            $wc.UploadFile("https://ci.appveyor.com/api/testresults/mstest/$($Env:APPVEYOR_JOB_ID)", (Resolve-Path $testFile))
        }
    }
}

task Build -depends Compile, Test, ResetPatch -precondition { return $component.ContainsKey('build') } {
    if (-not $component.ContainsKey('bin')) {
        return
    }
    Create-Directory $binariesDir
    foreach ($framework in $component.bin.frameworks) {
        $destDir = "$binariesDir\$framework"
        Create-Directory $destDir
        foreach ($include_dir in $component.bin.$framework.include) {
            $sourceDir = "$srcDir\$include_dir"
            Get-ChildItem "$sourceDir\**" | Copy-Item -Destination $destDir -Force
        }
    }
}

task Bench -depends Build -precondition { return $component.ContainsKey('bench') } {
    $exe = $component.bench.exe
    $categories = $component.bench.categories -join ","
    foreach ($framework in $component.bench.frameworks) {
        $exeFile = "$binariesDir\$framework\$exe"
        $artifacts = "$buildDir\bench\$framework"
        exec { &$exeFile --join --anyCategories=$categories --artifacts=$artifacts }
    }
}

task PackageNuGet -depends Build -precondition { return $component.ContainsKey('package') -and $component.package.ContainsKey('nuget') } {
    Create-Directory $pkgOutDir
    foreach ($package in $component.package.nuget) {
        $nuspec = "$packagesDir\$($package).nuspec"
        exec { nuget pack $nuspec -Version $version -OutputDirectory $pkgOutDir }
    }
}

task Package -depends Build, PackageNuGet {
}

task PublishNuGet -precondition { return $component.ContainsKey('package') -and $component.package.ContainsKey('nuget') } {
    $accessKeyFile = "$baseDir\..\Nuget-Access-Key.txt"
    if ( (Test-Path $accessKeyFile) ) {
        $accessKey = Get-Content $accessKeyFile
        $accessKey = $accessKey.Trim()
        
        foreach ($package in $component.package.nuget) {
            $nupkg = "$pkgOutDir\$($package).$($version).nupkg"
            exec { nuget push $nupkg $accessKey }
        }
    } else {
        Write-Host "Nuget-Access-Key.txt does not exist. Cannot publish the nuget package." -ForegroundColor Yellow
    }
}

task Publish -depends Package, PublishNuGet {
}
