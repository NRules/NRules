function New-Directory([string] $directoryName) {
    New-Item $directoryName -ItemType Directory -ErrorAction SilentlyContinue | Out-Null
}

function Remove-Directory([string] $directoryName) {
    Remove-Item -Force -Recurse $directoryName -ErrorAction SilentlyContinue
}

function Remove-File([string] $fileName) {
    if ($fileName) {
        Remove-Item $fileName -Force -ErrorAction SilentlyContinue | Out-Null
    } 
}

function Update-InternalsVisible([string] $path, [string] $publicKey, [string] $assemblyInfoFileName = "InternalsVisibleTo.props") {
    Write-Host Patching InternalsVisibleTo with public key
    
    $internalsVisibleToPattern = '\<InternalsVisibleTo\s+Include=\"(NRules.+?),PublicKey=.*?\"\s+\/\>'
    $internalsVisibleTo = '<InternalsVisibleTo Include="$1,PublicKey=' + $publicKey + '" />'

    Get-ChildItem -Path $path -Recurse -Filter $assemblyInfoFileName | % {
        $filename = $_.fullname

        $tmp = ($filename + ".tmp")
        Remove-File $tmp

        (Get-Content $filename) |
            % {$_ -replace $internalsVisibleToPattern, $internalsVisibleTo } |
            out-file $tmp -Encoding ASCII
        Move-Item $tmp $filename -Force
    }
}

function Assert-DotNetSdk([string] $sdkVersion, [string[]] $runtimes) {
    Assert ($sdkVersion -ne $null) '.NET SDK version should not be null'

    $env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"

    Assert ($null -ne (Get-Command "dotnet" -ErrorAction SilentlyContinue)) ".NET SDK not found. Install .NET SDK $sdkVersion"

    $installedSdks = dotnet --list-sdks | % { ($_ -split ' ')[0] }
    $matchingSdks = @($installedSdks | ? { $_.StartsWith("$sdkVersion.") })
    Assert ($matchingSdks.Count -gt 0) ".NET SDK $sdkVersion not found. Install .NET SDK $sdkVersion"
    Write-Host "Found .NET SDK $($matchingSdks -join ', ')"

    $installedRuntimes = dotnet --list-runtimes | ? { $_.StartsWith("Microsoft.NETCore.App ") } | % { ($_ -split ' ')[1] }
    foreach ($runtime in $runtimes) {
        $matchingRuntimes = @($installedRuntimes | ? { $_.StartsWith("$runtime.") })
        Assert ($matchingRuntimes.Count -gt 0) ".NET Runtime $runtime not found. Install .NET Runtime $runtime"
        Write-Host "Found .NET Runtime $($matchingRuntimes -join ', ')"
    }
}

function IsOnWindows() {
    return !(Get-Variable -Name IsWindows -ErrorAction SilentlyContinue) -or $IsWindows
}

function GetOsName() {
    if (IsOnWindows) {
        return "windows"
    } elseif ($IsMacOS) {
        return "macos"
    } elseif ($IsLinux) {
        return "linux"
    } else {
        throw "Unknown OS"
    }
}

function IsCompatibleOs([string[]] $oslist) {
    $osName = GetOsName
    return ($null -eq $oslist) -or ($oslist -contains $osName)
}
