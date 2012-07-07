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
$libDir = "$baseDir\lib" 
$toolsDir = "$baseDir\tools"
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
  
task Compile -depends InitEnvironment -description "Builds NRules and keeps the output in \binaries" { 
	$solutions = dir "$srcDir\NRules\*.sln"
	$solutions | % {
		$solutionFile = $_.FullName
		exec { &$script:msBuild $solutionFile /p:OutDir="$outDir\" /p:Configuration=$buildConfiguration }
	}
}

task Build -depends Init, Compile -description "Builds all the source code" {
	if(Test-Path $binariesDir) {
		Delete-Directory "binaries"
	}
	
	Create-Directory $binariesDir
	
	Copy-Item $outDir\*.dll $binariesDir -Force;
	Copy-Item $outDir\*.pdb $binariesDir -Force;
}
