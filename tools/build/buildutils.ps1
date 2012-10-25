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
