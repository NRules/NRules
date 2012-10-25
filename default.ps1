properties {
	$ProductVersion = "0.1"
	$BuildNumber = "0";
	$PatchVersion = "0"
	$PreRelease = "-build"	
	$PackageNameSuffix = ""
	$TargetFramework = "net-4.0"
	$UploadPackage = $false;
	$NugetKey = ""
	$PackageIds = ""
	$DownloadDependentPackages = $false
	$buildConfiguration = "Release"
}

$baseDir  = resolve-path .
$binariesDir = "$baseDir\binaries"
$srcDir = "$baseDir\src"
$buildBase = "$baseDir\build"
$outDir =  "$buildBase\output"
$mergeDir = "$buildBase\merge"
$libDir = "$baseDir\lib" 
$toolsDir = "$baseDir\tools"
$ilMergeExec = "$toolsDir\IlMerge\ilmerge.exe"
$ilMergeExclude = "$toolsDir\IlMerge\ilmerge.exclude"
$script:architecture = "x86"
$script:msBuild = ""
$script:msBuildTargetFramework = ""	
$script:nunitTargetFramework = "/framework=4.0";
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
			$netfxInstallroot ="" 
			$netfxInstallroot =	Get-RegistryValue 'HKLM:\SOFTWARE\Microsoft\.NETFramework\' 'InstallRoot' 
			$netfxCurrent = $netfxInstallroot + "v4.0.30319"
			$script:msBuild = $netfxCurrent + "\msbuild.exe"
			
			echo ".NET 4.0 build requested - $script:msBuild" 
			
			$script:ilmergeTargetFramework  = "/targetplatform:v4," + $netfxCurrent
			$script:msBuildTargetFramework ="/p:TargetFrameworkVersion=v4.0 /ToolsVersion:4.0"
			$script:nunitTargetFramework = "/framework=4.0";
			$script:isEnvironmentInitialized = $true
		}
	}
	$binariesExists = Test-Path $binariesDir;
	if ($binariesExists -eq $false) {	
		Create-Directory $binariesDir
		echo "created binaries"
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
	Get-ChildItem "$outDir\**" -Include *.dll, *.pdb, *.xml -Exclude NRules** | Copy-Item -Destination $binariesDir -Force
}
