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
$buildBase = "$baseDir\build"
$outDir =  "$buildBase\output"
$mergeDir = "$buildBase\merge"
$pkgDir = "$baseDir\packages"
$libDir = "$baseDir\lib" 
$toolsDir = "$baseDir\tools"
$nugetExec = "$toolsDir\NuGet\nuget.exe"
$ilMergeExec = "$toolsDir\IlMerge\ilmerge.exe"
$ilMergeExclude = "$toolsDir\IlMerge\ilmerge.exclude"

$script:msBuild = ""
$script:msBuildTargetFramework = ""
$script:ilmergeTargetFramework = ""
$script:isEnvironmentInitialized = $false

include $toolsDir\build\buildutils.ps1

task default -depends Build -description "Invokes Build task"
 
task Clean -description "Cleans the eviorment for the build" {
	if(Test-Path $buildBase) {
		Delete-Directory $buildBase
	}
	
	if(Test-Path $binariesDir) {
		Delete-Directory $binariesDir
	}
}

task InitEnvironment -description "Initializes the environment for build" {
	if($script:isEnvironmentInitialized -ne $true) {
		if ($TargetFramework -eq "net-4.0") {
			$netfxInstallroot = "" 
			$netfxInstallroot =	Get-RegistryValue 'HKLM:\SOFTWARE\Microsoft\.NETFramework\' 'InstallRoot' 
			$netfxCurrent = $netfxInstallroot + "v4.0.30319"
			$script:msBuild = $netfxCurrent + "\msbuild.exe"
			
			echo ".NET 4.0 build requested - $script:msBuild" 
			
			$script:ilmergeTargetFramework  = "/targetplatform:v4," + $netfxCurrent
			$script:msBuildTargetFramework ="/p:TargetFrameworkVersion=v4.0 /ToolsVersion:4.0"
			$script:isEnvironmentInitialized = $true
		}
	}
	$binariesExists = Test-Path $binariesDir;
	if ($binariesExists -eq $false) {	
		Create-Directory $binariesDir
		echo "Created binaries"
	}
}

task Init -depends Clean -description "Initializes the build" {
	echo "Creating build directory at the following path $buildBase"
	Delete-Directory $buildBase
	Create-Directory $buildBase
	
	$currentDirectory = Resolve-Path .
	echo "Current Directory: $currentDirectory" 
 }
  
task Compile -depends InitEnvironment -description "Compiles source code into assemblies" { 
	$solutions = dir "$srcDir\NRules\*.sln"
	$solutions | % {
		$solutionFile = $_.FullName
		exec { &$script:msBuild $solutionFile /p:OutDir="$outDir\" /p:Configuration=$buildConfiguration }
	}
}

task Merge -depends Init, Compile -description "Merges compiled assemblies into coarse-grained components"{
	$assemblies = @()
	$assemblies += dir $outDir\NRules.*.dll -Exclude **Tests.dll
	
	$logFileName = "$buildBase\NRulesMergeLog.txt"
	Create-Directory $mergeDir
	
	&$ilMergeExec /out:"$mergeDir\NRules.dll" /log:$logFileName /internalize:$ilMergeExclude $script:ilmergeTargetFramework $assemblies /xmldocs
	$mergeLogContent = Get-Content "$logFileName"
	echo $mergeLogContent
}

task Build -depends Merge -description "Builds final binaries" {
	if(Test-Path $binariesDir) {
		Delete-Directory "binaries"
	}
	
	Create-Directory $binariesDir
	
	Get-ChildItem "$mergeDir\**" -Include *.dll, *.pdb, *.xml | Copy-Item -Destination $binariesDir -Force	
	Get-ChildItem "$outDir\**" -Include *.dll, *.pdb, *.xml -Exclude NRules**, nunit**, Rhino.Mocks** | Copy-Item -Destination $binariesDir -Force
}

task NuGet -depends Build -description "Generates NuGet package" {
	Remove-Item $pkgDir\NRules*.nupkg

	$nugetDir = "$buildBase\NuGet"
	Remove-Item $nugetDir -Force -Recurse -ErrorAction SilentlyContinue
	New-Item $nugetDir -Type directory | Out-Null

	New-Item $nugetDir\NRules\lib\net40 -Type directory | Out-Null
	
	Copy-Item $pkgDir\NRules.dll.nuspec $nugetDir\NRules\NRules.nuspec
	@("NRules.???") |% { Copy-Item "$binariesDir\$_" $nugetDir\NRules\lib\net40 }

	$nugetVersion = "$ProductVersion"

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
			&"$nugetExec" push "$($_.BaseName).$nugetVersion.nupkg" $accessKey
		}
	}
	else {
		Write-Host "Nuget-Access-Key.txt does not exit. Cannot publish the nuget package." -ForegroundColor Yellow
	}
}
