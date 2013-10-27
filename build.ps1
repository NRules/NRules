param (
    [string]$target = "Build"
)
 
Import-Module .\tools\build\psake.psm1
Invoke-psake .\default.ps1 $target