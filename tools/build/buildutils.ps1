function Delete-Directory($directoryName) {
	Remove-Item -Force -Recurse $directoryName -ErrorAction SilentlyContinue
}

function Delete-File($fileName) {
	if($fileName) {
		Remove-Item $fileName -Force -ErrorAction SilentlyContinue | Out-Null
	} 
}
 
function Create-Directory($directoryName) {
	New-Item $directoryName -ItemType Directory -ErrorAction SilentlyContinue | Out-Null
}

function Get-RegistryValues($key) {
	(Get-Item $key -ErrorAction SilentlyContinue).GetValueNames()
}

function Get-RegistryValue($key, $value) {
	(Get-ItemProperty $key $value -ErrorAction SilentlyContinue).$value
}

function Update-AssemblyInfoFiles ([string] $version, [string] $assemblyInfoFileName = "AssemblyInfo.cs") {
	if ($version -notmatch "[0-9]+(\.([0-9]+|\*)){1,3}") {
		Write-Error "Version number incorrect format: $version"
	}
	Write-Host Patching AssemblyInfo files with version $version
	
	$versionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
	$versionAssembly = 'AssemblyVersion("' + $version + '")';
	$versionFilePattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
	$versionAssemblyFile = 'AssemblyFileVersion("' + $version + '")';

	Get-ChildItem -r -filter $assemblyInfoFileName | % {
		$filename = $_.fullname

		$tmp = ($filename + ".tmp")
		Delete-File $tmp

		(Get-Content $filename) | % {$_ -replace $versionFilePattern, $versionAssemblyFile } | % {$_ -replace $versionPattern, $versionAssembly }  | out-file $tmp -encoding ASCII
		Move-Item $tmp $filename -Force
	}
}

function Update-AssemblyVersion([string] $version) {
	Update-AssemblyInfoFiles $version "GlobalAssemblyInfo.cs"
}

function Reset-AssemblyVersion() {
	Update-AssemblyInfoFiles "1.0.0.0" "GlobalAssemblyInfo.cs"
}

function Get-DotNetProjects([string] $path) {
	Get-ChildItem -Path $path -Recurse -Include "*.csproj" | Select-Object @{ Name="ParentFolder"; Expression={ $_.Directory.FullName.TrimEnd("\") } } | Select-Object -ExpandProperty ParentFolder
}

function Install-DotNetCli([string] $location, [string] $version = "Latest") {
	if ((Get-Command "dotnet.exe" -ErrorAction SilentlyContinue) -ne $null) {
		Write-Host ".NET Core SDK is already installed"
		return;
	}

	$installDir = Join-Path -Path $location -ChildPath "cli"
	if (!(Test-Path $installDir)) {
		Create-Directory $installDir
	}

	if (!(Test-Path $location\dotnet-install.ps1)) {
		Invoke-WebRequest "https://raw.githubusercontent.com/dotnet/cli/rel/1.0.1/scripts/obtain/dotnet-install.ps1" -OutFile "$location\dotnet-install.ps1"
	}

	Write-Host "Installing .NET Core SDK"
	& $location\dotnet-install.ps1 -InstallDir "$installDir" -Version $version
	$env:PATH = "$installDir;$env:PATH"
}

function Install-NuGet([string] $location, [string] $version = "latest") {
	if ((Get-Command "nuget.exe" -ErrorAction SilentlyContinue) -ne $null) {
		Write-Host "NuGet is already installed"
		return;
	}

	$installDir = $location
	if (!(Test-Path $installDir)) {
		Create-Directory $installDir
	}

	if (!(Test-Path $location\nuget.exe)) {
		$url = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
		if ($version -ne "latest") {
			$url = "https://dist.nuget.org/win-x86-commandline/v$version/nuget.exe"
		}
		Invoke-WebRequest $url -OutFile "$location\nuget.exe"
	}

	$env:PATH = "$installDir;$env:PATH"
}
