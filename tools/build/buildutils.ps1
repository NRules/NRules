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

$ilMergeExec = ".\tools\IlMerge\ilmerge.exe"
function Ilmerge($directory, $name, $assemblies, $extension, $ilmergeTargetframework, $logFileName, $excludeFilePath){    
	echo "Merging $name....."	
	
    New-Item -path $directory -name "temp_merge" -type directory -ErrorAction SilentlyContinue
	
	&$ilMergeExec /out:"$directory\temp_merge\$name.$extension" /log:$logFileName /internalize:$excludeFilePath $ilmergeTargetframework $assemblies /xmldocs
	
    Get-ChildItem "$directory\temp_merge\**" -Include *.$extension, *.pdb, *.xml | Copy-Item -Destination $directory
    Remove-Item "$directory\temp_merge" -Recurse -ErrorAction SilentlyContinue
	$mergeLogContent = Get-Content "$logFileName"
	echo "------------------------------$name Merge Log-----------------------"
	echo $mergeLogContent
}
