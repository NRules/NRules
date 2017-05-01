param (
    [hashtable] $component
)

properties {
    $version = $null
    $sdkVersion = "1.0.1"
    $nugetVersion = "4.1.0"
    $configuration = "Release"
}

$base_dir  = resolve-path .
$tools_dir = "$base_dir\tools"

include $tools_dir\build\buildutils.ps1

task default -depends Build

task Init {
    Assert ($version -ne $null) 'Version should not be null'
    Assert ($component -ne $null) 'Component should not be null'
    
    Write-Host "Building $($component.name) version $version ($configuration)" -ForegroundColor Green
    
    $comp_name = $component.name
    $src_root = if ($component.ContainsKey('src_root')) { $component.src_root } else { 'src' }
    
    $script:binaries_dir = "$base_dir\binaries\$comp_name"
    $script:src_dir = "$base_dir\$src_root\$comp_name"
    $script:build_dir = "$base_dir\build"
    $script:pkg_out_dir = "$build_dir\packages\$comp_name"
    $script:packages_dir = "$base_dir\packages"
    $script:help_dir = "$base_dir\help"
    
    Install-DotNetCli $tools_dir\.dotnet $sdkVersion
    Install-NuGet $tools_dir\.nuget $nugetVersion
}

task Clean -depends Init {
    Delete-Directory $pkg_out_dir
    Delete-Directory $binaries_dir
}

task SetVersion {
    Write-Host Build Version: $version
    Update-Version $version
}

task ResetVersion {
    Reset-Version
}

task RestoreDependencies -precondition { return $component.ContainsKey('restore') } {
    if ($component.restore.tool -eq 'nuget') {
        exec { nuget restore $src_dir -NonInteractive }
    }
    if ($component.restore.tool -eq 'dotnet') {
        exec { dotnet restore $src_dir --verbosity minimal }
    }
}

task Compile -depends Init, Clean, SetVersion, RestoreDependencies -precondition { return $component.ContainsKey('build') } { 
    Create-Directory $build_dir
    
    $solution_file = "$src_dir\$($component.name).sln"
    if ($component.build.tool -eq 'msbuild') {
        $framework_root = Get-RegistryValue 'HKLM:\SOFTWARE\Microsoft\.NETFramework\' 'InstallRoot' 
        $framework_root = $framework_root + "v4.0.30319"
        $msbuild_exec = $framework_root + "\msbuild.exe"
        exec { &$msbuild_exec $solution_file /p:Configuration=$configuration /v:m /nologo }
    }
    if ($component.build.tool -eq 'dotnet') {
        exec { dotnet build $solution_file --configuration $configuration --verbosity minimal }
    }
}

task Test -depends Compile -precondition { return $component.ContainsKey('test') } {
    $tests_dir = "$src_dir\$($component.test.location)"
    $projects = Get-DotNetProjects $tests_dir
    foreach ($project in $projects) {
        Push-Location $project
        exec { dotnet test --no-build --configuration $configuration --framework net46 --verbosity minimal --logger "trx;LogFileName=TestResult.trx" }
        Pop-Location
    }
	
	if (Test-Path Env:CI) {
		Write-Host "Uploading test results to CI server"
		$wc = New-Object 'System.Net.WebClient'
		Get-ChildItem $tests_dir -recurse -filter "*.trx" | % {
			$test_file = $_.fullname
			$wc.UploadFile("https://ci.appveyor.com/api/testresults/mstest/$($Env:APPVEYOR_JOB_ID)", (Resolve-Path $test_file))
		}
	}
}

task Build -depends Compile, Test, ResetVersion -precondition { return $component.ContainsKey('build') } {
    if (-not $component.ContainsKey('bin')) {
        return
    }
    Create-Directory $binaries_dir
    foreach ($framework in $component.bin.frameworks) {
        $dest_dir = "$binaries_dir\$framework"
        Create-Directory $dest_dir
        foreach ($include_dir in $component.bin.$framework.include) {
            $source_dir = "$src_dir\$include_dir"
            Get-ChildItem "$source_dir\**" | Copy-Item -Destination $dest_dir -Force
        }
    }
}

task PackageNuGet -depends Build -precondition { return $component.ContainsKey('package') -and $component.package.ContainsKey('nuget') } {
    Create-Directory $pkg_out_dir
    foreach ($package in $component.package.nuget) {
        $nuspec = "$packages_dir\$($package).nuspec"
        exec { nuget pack $nuspec -Version $version -OutputDirectory $pkg_out_dir }
    }
}

task Package -depends Build, PackageNuGet {
}

task PublishNuGet -precondition { return $component.ContainsKey('package') -and $component.package.ContainsKey('nuget') } {
    $accessKeyFile = "$base_dir\..\Nuget-Access-Key.txt"
    if ( (Test-Path $accessKeyFile) ) {
        $accessKey = Get-Content $accessKeyFile
        $accessKey = $accessKey.Trim()
        
        foreach ($package in $component.package.nuget) {
            $nupkg = "$pkg_out_dir\$($package).$($version).nupkg"
            exec { nuget push $nupkg $accessKey }
        }
    } else {
        Write-Host "Nuget-Access-Key.txt does not exist. Cannot publish the nuget package." -ForegroundColor Yellow
    }
}

task Publish -depends Package, PublishNuGet {
}

task Help -depends Init, Build -precondition { return $component.ContainsKey('help') } {
    Assert (Test-Path Env:\SHFBROOT) 'Sandcastle root environment variable SHFBROOT is not set'
    
    Create-Directory $build_dir
    
    $help_proj_file = "$help_dir\$($component.help)"
    exec { &$script:msbuild_exec $help_proj_file /v:m /nologo }
}
