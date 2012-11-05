function Delete-Directory($directoryName) {
	Remove-Item -Force -Recurse $directoryName -ErrorAction SilentlyContinue
}
 
function Create-Directory($directoryName) {
	New-Item $directoryName -ItemType Directory | Out-Null
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

	$versionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
	$versionAssembly = 'AssemblyVersion("' + $version + '")';
	$versionFilePattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
	$versionAssemblyFile = 'AssemblyFileVersion("' + $version + '")';

	Get-ChildItem -r -filter $assemblyInfoFileName | % {
		$filename = $_.fullname

		$tmp = ($file + ".tmp")
		if (Test-Path ($tmp)) { Remove-Item $tmp }

		(Get-Content $filename) | % {$_ -replace $versionFilePattern, $versionAssemblyFile } | % {$_ -replace $versionPattern, $versionAssembly }  > $tmp
		Write-Host Updating file AssemblyInfo and AssemblyFileInfo: $filename --> $versionAssembly / $versionAssemblyFile

		if (Test-Path ($filename)) { Remove-Item $filename }
		Move-Item $tmp $filename -Force	
	}
}

function Update-AssemblyVersion([string] $version){
	Update-AssemblyInfoFiles $version "CommonAssemblyInfo.cs"
}

function Reset-AssemblyVersion(){
	Update-AssemblyInfoFiles "1.0.0.0" "CommonAssemblyInfo.cs"
}

