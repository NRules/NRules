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

function Install-DotNetCli([string] $location, [string] $version, [string[]] $runtimes) {
    Assert ($version -ne $null) '.NET SDK version should not be null'

    $env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"
    
    if ($null -ne (Get-Command "dotnet" -ErrorAction SilentlyContinue)) {
        $installedVersion = dotnet --version
        if ($installedVersion -eq $version) {
            Write-Host ".NET SDK version $version is already installed"
            return;
        }
    }

    $installDir = Join-Path -Path $location -ChildPath "cli"
    if (!(Test-Path $installDir)) {
        New-Directory $installDir
    }

    $installScriptName = if (IsOnWindows) { "dotnet-install.ps1" } else { "dotnet-install.sh" }
    $installScriptPath = Join-Path $location $installScriptName

    (New-Object System.Net.WebClient).Proxy.Credentials = [System.Net.CredentialCache]::DefaultNetworkCredentials

    if (!(Test-Path $installScriptPath)) {
        $url = "https://dot.net/v1/$installScriptName"
        Invoke-WebRequest $url -OutFile $installScriptPath
        if (!(IsOnWindows)) {
            & chmod +x $installScriptPath
        }
    }

    Write-Host "Installing .NET SDK $version"
    & $installScriptPath --install-dir "$installDir" --version $version

    if ($null -ne $runtimes) {
        foreach ($runtime in $runtimes) {
            Write-Host "Installing .NET Runtime $runtime"
            & $installScriptPath --install-dir "$installDir" --runtime dotnet --version $runtime
        }
    }

    if (!($env:PATH -contains $installDir)) {
        $envPathSeparator = if (IsOnWindows) { ';' } else { ':' }
        $env:PATH = $installDir + $envPathSeparator + $env:PATH
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
