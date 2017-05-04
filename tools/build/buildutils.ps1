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

function Update-Properties([string] $version, [string] $propsFileName = "Common.props") {
    if ($version -notmatch "[0-9]+(\.([0-9]+|\*)){1,3}") {
        Write-Error "Version number incorrect format: $version"
    }
    Write-Host Patching project properties files with version $version
    
    $versionPrefixPattern = '<VersionPrefix>[0-9]+(\.([0-9]+|\*)){1,3}<\/VersionPrefix>'
    $versionPrefix = '<VersionPrefix>' + $version + '</VersionPrefix>';

    Get-ChildItem -Recurse -Filter $propsFileName | % {
        $filename = $_.fullname

        $tmp = ($filename + ".tmp")
        Delete-File $tmp

        (Get-Content $filename) |
            % {$_ -replace $versionPrefixPattern, $versionPrefix } |
            out-file $tmp -Encoding ASCII
        Move-Item $tmp $filename -Force
    }
}

function Update-AssemblyInfoFiles([string] $version, [string] $assemblyInfoFileName = "AssemblyInfo.cs") {
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

    Get-ChildItem -Recurse -Filter $assemblyInfoFileName | % {
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

function Update-Version([string] $version) {
    Update-AssemblyInfoFiles $version "GlobalAssemblyInfo.cs"
    Update-Properties $version "Common.props"
}

function Reset-Version() {
    Update-AssemblyInfoFiles "1.0.0.0" "GlobalAssemblyInfo.cs"
    Update-Properties "1.0.0" "Common.props"
}

function Get-DotNetProjects([string] $path) {
    Get-ChildItem -Path $path -Recurse -Include "*.csproj" | Select-Object @{ Name="ParentFolder"; Expression={ $_.Directory.FullName.TrimEnd("\") } } | Select-Object -ExpandProperty ParentFolder
}

function Install-DotNetCli([string] $location, [string] $version = "Latest") {
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
        Invoke-WebRequest "https://raw.githubusercontent.com/dotnet/cli/rel/1.0.1/scripts/obtain/dotnet-install.ps1" -OutFile "$location\dotnet-install.ps1"
    }

    Write-Host "Installing .NET Core SDK"
    & $location\dotnet-install.ps1 -InstallDir "$installDir" -Version $version

    if (!($env:PATH -contains $installDir)) {
        $env:PATH = "$installDir;$env:PATH"
        $env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"
    }
}

function Install-NuGet([string] $location, [string] $version = "latest") {
    (New-Object System.Net.WebClient).Proxy.Credentials = [System.Net.CredentialCache]::DefaultNetworkCredentials
    $installDir = $location
    if (!(Test-Path $installDir)) {
        Create-Directory $installDir
    }

    if (!(Test-Path $location\nuget.exe)) {
        Write-Host "Downloading NuGet version $version"
        $url = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
        if ($version -ne "latest") {
            $url = "https://dist.nuget.org/win-x86-commandline/v$version/nuget.exe"
        }
        Invoke-WebRequest $url -OutFile "$location\nuget.exe"
    }

    if (!($env:PATH -contains $installDir)) {
        $env:PATH = "$installDir;$env:PATH"
    }
}
