function Create-Directory([string] $directoryName) {
    New-Item $directoryName -ItemType Directory -ErrorAction SilentlyContinue | Out-Null
}

function Delete-Directory([string] $directoryName) {
    Remove-Item -Force -Recurse $directoryName -ErrorAction SilentlyContinue
}

function Delete-File([string] $fileName) {
    if ($fileName) {
        Remove-Item $fileName -Force -ErrorAction SilentlyContinue | Out-Null
    } 
}

function Get-Msbuild() {
    $vsRoot = "${env:ProgramFiles(x86)}\Microsoft Visual Studio"
    $msbuild = "msbuild.exe"
    if (Test-Path $vsRoot) {
        Get-ChildItem -Path $vsRoot | Where {$_ -match "^\d+$"} | Sort-Object -Descending |% {
            $vsPath = "$vsRoot\$_"
            Get-ChildItem -Path $vsPath |% {
                $msbuildPath = "$vsPath\$_\MSBuild\Current\Bin"
                $msbuild = "$msbuildPath\msbuild.exe"
                return
            }
        }
    }
    return $msbuild
}

function Update-InternalsVisible([string] $path, [string] $publicKey, [string] $assemblyInfoFileName = "AssemblyInfo.cs") {
    Write-Host Patching AssemblyInfo files with public key
    
    $internalsVisibleToPattern = '\[assembly\:\s*InternalsVisibleTo\(\"(NRules.+?),PublicKey=.*?\"\)\]'
    $internalsVisibleTo = '[assembly: InternalsVisibleTo("$1,PublicKey=' + $publicKey + '")]';

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

function Update-Properties([string] $path, [string] $version, [string] $propsFileName = "Common.props") {
    if ($version -notmatch "[0-9]+(\.([0-9]+|\*)){1,3}") {
        Write-Error "Version number incorrect format: $version"
    }
    Write-Host Patching project properties files with version $version
    
    $versionPrefixPattern = '<VersionPrefix>[0-9]+(\.([0-9]+|\*)){1,3}<\/VersionPrefix>'
    $versionPrefix = '<VersionPrefix>' + $version + '</VersionPrefix>';

    Get-ChildItem -Path $path -Recurse -Filter $propsFileName | % {
        $filename = $_.fullname

        $tmp = ($filename + ".tmp")
        Delete-File $tmp

        (Get-Content $filename) |
            % {$_ -replace $versionPrefixPattern, $versionPrefix } |
            out-file $tmp -Encoding ASCII
        Move-Item $tmp $filename -Force
    }
}

function Update-AssemblyInfoFiles([string] $path, [string] $version, [string] $assemblyInfoFileName = "AssemblyInfo.cs") {
    if ($version -notmatch "[0-9]+(\.([0-9]+|\*)){1,3}") {
        Write-Error "Version number incorrect format: $version"
    }
    Write-Host Patching AssemblyInfo files with version $version
    
    $assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $assemblyVersion = 'AssemblyVersion("' + $version + '")';
    $assemblyFileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $assemblyFileVersion = 'AssemblyFileVersion("' + $version + '")';
    $assemblyInfoVersionPattern = 'AssemblyInformationalVersionAttribute\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $assemblyInfoVersion = 'AssemblyInformationalVersionAttribute("' + $version + '")';

    Get-ChildItem -Path $path -Recurse -Filter $assemblyInfoFileName | % {
        $filename = $_.fullname

        $tmp = ($filename + ".tmp")
        Delete-File $tmp

        (Get-Content $filename) |
            % {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
            % {$_ -replace $assemblyFileVersionPattern, $assemblyFileVersion } | 
            % {$_ -replace $assemblyInfoVersionPattern, $assemblyInfoVersion } |
            out-file $tmp -Encoding ASCII
        Move-Item $tmp $filename -Force
    }
}

function Update-Version([string] $path, [string] $version) {
    Update-AssemblyInfoFiles $path $version "GlobalAssemblyInfo.cs"
    Update-Properties $path $version "Common.props"
}

function Reset-Version([string] $path) {
    Update-AssemblyInfoFiles $path "1.0.0.0" "GlobalAssemblyInfo.cs"
    Update-Properties $path "1.0.0" "Common.props"
}

function Get-DotNetProjects([string] $path) {
    Get-ChildItem -Path $path -Recurse -Include "*.csproj" | Select-Object @{ Name="ParentFolder"; Expression={ $_.Directory.FullName.TrimEnd("\") } } | Select-Object -ExpandProperty ParentFolder
}

function Install-DotNetCli([string] $location, [string] $version) {
    Assert ($version -ne $null) 'DotNet CLI version should not be null'
    
    (New-Object System.Net.WebClient).Proxy.Credentials = [System.Net.CredentialCache]::DefaultNetworkCredentials
    if ((Get-Command "dotnet.exe" -ErrorAction SilentlyContinue) -ne $null) {
        $installedVersion = dotnet --version
        if ($installedVersion -eq $version) {
            Write-Host ".NET Core SDK version $version is already installed"
            return;
        }
    }
  
    $installDir = Join-Path -Path $location -ChildPath "cli"
    if (!(Test-Path $installDir)) {
        Create-Directory $installDir
    }

    if (!(Test-Path $location\dotnet-install.ps1)) {
        $url = "https://dotnet.microsoft.com/download/dotnet-core/scripts/v1/dotnet-install.ps1"
        Invoke-WebRequest $url -OutFile "$location\dotnet-install.ps1"
    }

    Write-Host "Installing .NET Core SDK"
    & $location\dotnet-install.ps1 -InstallDir "$installDir" -Version $version

    if (!($env:PATH -contains $installDir)) {
        $env:PATH = "$installDir;$env:PATH"
    }
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"
}

function Install-NuGet([string] $location, [string] $version) {
    Assert ($version -ne $null) 'NuGet version should not be null'
    
    (New-Object System.Net.WebClient).Proxy.Credentials = [System.Net.CredentialCache]::DefaultNetworkCredentials
    $installDir = $location
    if (!(Test-Path $installDir)) {
        Create-Directory $installDir
    }

    if (!(Test-Path $location\nuget.exe)) {
        Write-Host "Downloading NuGet version $version"
        $url = "https://dist.nuget.org/win-x86-commandline/v$version/nuget.exe"
        Invoke-WebRequest $url -OutFile "$location\nuget.exe"
    }

    if (!($env:PATH -contains $installDir)) {
        $env:PATH = "$installDir;$env:PATH"
    }
}
