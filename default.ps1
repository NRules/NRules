properties {
	$ProductVersion = "0.1"
	$BuildNumber = "0";
	$PackageNameSuffix = ""
	$TargetFramework = "net-4.0"
	$buildConfiguration = "Release"
}

$baseDir  = resolve-path .
$binariesDir = "$baseDir\binaries"
$srcDir = "$baseDir\src"
$buildDir = "$baseDir\build"
$outDir =  "$buildDir\output"
$mergeDir = "$buildDir\merge"
$pkgDir = "$baseDir\packages"
$libDir = "$baseDir\lib" 
$toolsDir = "$baseDir\tools"
$nugetExec = "$toolsDir\NuGet\nuget.exe"
$ilMergeExec = "$toolsDir\IlMerge\ilmerge.exe"
$ilMergeExclude = "$toolsDir\IlMerge\ilmerge.exclude"

$script:version = ""
$script:msBuild = ""
$script:msBuildTargetFramework = ""
$script:ilmergeTargetFramework = ""
$script:isEnvironmentInitialized = $false

include $toolsDir\build\buildutils.ps1

task default -depends Build -description "Invokes Build task"
 
task Clean -description "Cleans the environment for the build" {
	if (Test-Path $buildDir) {
		Delete-Directory $buildDir
	}
	
	if (Test-Path $binariesDir) {
		Delete-Directory $binariesDir
	}
}

task Init -depends Clean -description "Initializes the build" {
	Write-Host "Creating build directory at the following path $buildDir"
	Create-Directory $buildDir
	
	Write-Host "Creating binaries directory at the following path $binariesDir"
	Create-Directory $binariesDir
	
	$currentDirectory = Resolve-Path .
	Write-Host "Current Directory: $currentDirectory" 
}

task InitEnvironment -depends Init -description "Initializes the environment for build" {
	if ($script:isEnvironmentInitialized -ne $true) {
		if ($TargetFramework -eq "net-4.0") {
			$netfxInstallroot = "" 
			$netfxInstallroot =	Get-RegistryValue 'HKLM:\SOFTWARE\Microsoft\.NETFramework\' 'InstallRoot' 
			$netfxCurrent = $netfxInstallroot + "v4.0.30319"
			$script:msBuild = $netfxCurrent + "\msbuild.exe"
			
			Write-Host ".NET 4.0 build requested - $script:msBuild" 
			
			$script:ilmergeTargetFramework  = "/targetplatform:v4," + $netfxCurrent
			$script:msBuildTargetFramework ="/p:TargetFrameworkVersion=v4.0 /ToolsVersion:4.0"
			$script:isEnvironmentInitialized = $true
		}
	}
}

task Version -description "Sets version in the source files" {
	$script:version = "$ProductVersion.$BuildNumber"
	Write-Host Build Version: $script:version
	
	Update-AssemblyVersion $script:version
}

task Compile -depends InitEnvironment, Version -description "Compiles source code into assemblies" { 
	$solutions = Get-ChildItem $srcDir\NRules\*.sln
	$solutions | % {
		$solutionFile = $_.FullName
		exec { &$script:msBuild $solutionFile /p:OutDir="$outDir\" /p:Configuration=$buildConfiguration }
	}
}

task Merge -depends Compile -description "Merges compiled assemblies into coarse-grained components" {
	$assemblies = @()
	$assemblies += Get-ChildItem $outDir\NRules.*.dll -Exclude **Tests.dll
	
	$attributeFile = "$outDir\NRules.Core.dll"
	
	$logFileName = "$buildDir\NRulesMergeLog.txt"
	Create-Directory $mergeDir
	
	&$ilMergeExec /out:"$mergeDir\NRules.dll" /log:$logFileName /internalize:$ilMergeExclude $script:ilmergeTargetFramework $assemblies /xmldocs /attr:$attributeFile
	$mergeLogContent = Get-Content "$logFileName"
	echo $mergeLogContent
}

task ResetVersion -description "Resets version in source files to default" {
	Reset-AssemblyVersion
}

task Build -depends Merge, ResetVersion -description "Builds final binaries" {
	Get-ChildItem "$mergeDir\**" -Include *.dll, *.pdb, *.xml | Copy-Item -Destination $binariesDir -Force	
	Get-ChildItem "$outDir\**" -Include *.dll, *.pdb, *.xml -Exclude NRules**, nunit**, Rhino.Mocks** | Copy-Item -Destination $binariesDir -Force
}

task Package -depends Build -description "Generates NuGet package" {
	Remove-Item $pkgDir\NRules*.nupkg

	$nugetDir = "$buildDir\NuGet"
	Remove-Item $nugetDir -Force -Recurse -ErrorAction SilentlyContinue
	New-Item $nugetDir -Type directory | Out-Null

	New-Item $nugetDir\NRules\lib\net40 -Type directory | Out-Null
	
	Copy-Item $pkgDir\NRules.dll.nuspec $nugetDir\NRules\NRules.nuspec
	@("NRules.???") |% { Copy-Item "$binariesDir\$_" $nugetDir\NRules\lib\net40 }

	$nugetVersion = "$ProductVersion.$BuildNumber"

	# Sets the package version in all the nuspec
	$packages = Get-ChildItem $nugetDir *.nuspec -recurse
	$packages |% { 
		$nuspec = [xml](Get-Content $_.FullName)
		$nuspec.package.metadata.version = $nugetVersion
		$nuspec | Select-Xml '//dependency' |% {
			if($_.Node.Id.StartsWith('NRules')){
				$_.Node.Version = "[$nugetVersion]"
			}
		}
		$nuspec.Save($_.FullName);
		&"$nugetExec" pack $_.FullName -OutputDirectory $nugetDir
	}

	# Upload packages
	$accessPath = "$baseDir\..\Nuget-Access-Key.txt"
	if ( (Test-Path $accessPath) ) {
		$accessKey = Get-Content $accessPath
		$accessKey = $accessKey.Trim()

		# Push to nuget repository
		$packages | ForEach-Object {
			&"$nugetExec" push "$nugetDir\$($_.BaseName).$nugetVersion.nupkg" $accessKey
		}
	}
	else {
		Write-Host "Nuget-Access-Key.txt does not exit. Cannot publish the nuget package." -ForegroundColor Yellow
	}
}
