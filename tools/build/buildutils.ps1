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

function Get-Msbuild() {
    $vsRoot = "${env:ProgramFiles}\Microsoft Visual Studio"
    $msbuild = "msbuild.exe"
    if (Test-Path $vsRoot) {
        Get-ChildItem -Path $vsRoot | Where {$_ -match "^\d+$"} | Sort-Object -Descending |% {
            $vsPath = "$vsRoot\$_"
            Get-ChildItem -Path $vsPath |% {
                $msbuildPath = "$vsPath\$_\MSBuild\Current\Bin"
                $msbuild = "$msbuildPath\msbuild.exe"
            }
        }
    }
    return $msbuild
}

function Update-InternalsVisible([string] $path, [string] $publicKey, [string] $assemblyInfoFileName = "InternalsVisibleTo.props") {
    Write-Host Patching InternalsVisibleTo with public key
    
    $internalsVisibleToPattern = '\<InternalsVisibleTo\s+Include=\"(NRules.+?),PublicKey=.*?\"\s+\/\>'
    $internalsVisibleTo = '<InternalsVisibleTo Include="$1,PublicKey=' + $publicKey + '" />'

    Get-ChildItem -Path $path -Recurse -Filter $assemblyInfoFileName | % {
        $filename = $_.fullname

        $tmp = ($filename + ".tmp")
        Delete-File $tmp

        (Get-Content $filename) |
            % {$_ -replace $internalsVisibleToPattern, $internalsVisibleTo } |
            out-file $tmp -Encoding ASCII
        Move-Item $tmp $filename -Force
    }
}

function Install-DotNetCli([string] $location, [string] $version) {
    Assert ($version -ne $null) 'DotNet CLI version should not be null'
    
    (New-Object System.Net.WebClient).Proxy.Credentials = [System.Net.CredentialCache]::DefaultNetworkCredentials
    if ($null -ne (Get-Command "dotnet" -ErrorAction SilentlyContinue)) {
        $installedVersion = dotnet --version
        if ($installedVersion -eq $version) {
            Write-Host ".NET Core SDK version $version is already installed"
            return;
        }
    }

    $installDir = Join-Path -Path $location -ChildPath "cli"
    if (!(Test-Path $installDir)) {
        New-Directory $installDir
    }

    $installScriptName = if ($IsWindows) { "dotnet-install.ps1" } else { "dotnet-install.sh" }
    $installScriptPath = Join-Path $location $installScriptName

    if (!(Test-Path $location\dotnet-install.ps1)) {
        $url = "https://dot.net/v1/$installScriptName"
        Invoke-WebRequest $url -OutFile $installScriptPath
    }

    Write-Host "Installing .NET Core SDK"
    & $installScriptPath -InstallDir "$installDir" -Version $version

    if (!($env:PATH -contains $installDir)) {
        $env:PATH = "$installDir;$env:PATH"
    }

    $env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"
}
