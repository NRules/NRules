param (
    [string]$target = 'Build',
    [string]$component_name = 'Core'
)

$version = '0.6.2'
$configuration = 'Release'

if (Test-Path Env:CI) { $version = $Env:APPVEYOR_BUILD_VERSION }
if (Test-Path Env:CI) { $configuration = $Env:CONFIGURATION }

$components = @{
    'NRules' = @{
        name = 'NRules'
        restore = @{
            tool = 'dotnet'
        }
        build = @{
            tool = 'dotnet'
        }
        test = @{
            location = 'Tests'
        }
        bin = @{
            frameworks = @('net45', 'netstandard1.0')
            'net45' = @{
                include = @(
                    "NRules\bin\$configuration\net45",
                    "NRules.Fluent\bin\$configuration\net45",
                    "NRules.RuleModel\bin\$configuration\net45"
                )
            }
            'netstandard1.0' = @{
                include = @(
                    "NRules\bin\$configuration\netstandard1.0",
                    "NRules.Fluent\bin\$configuration\netstandard1.0",
                    "NRules.RuleModel\bin\$configuration\netstandard1.0"
                )
            }
        }
        package = @{
            nuget = @(
                'NRules.RuleModel',
                'NRules.Fluent',
                'NRules.Runtime',
                'NRules'
            )
        }
        help = 'NRules.shfbproj'
    };
    'NRules.Debugger.Visualizer' = @{
        name = 'NRules.Debugger.Visualizer'
        restore = @{
            tool = 'dotnet'
        }
        build = @{
            tool = 'dotnet'
        }
        bin = @{
            frameworks = @('net45')
            'net45' = @{
                include = @(
                    "NRules.Debugger.Visualizer\bin\$configuration\net45"
                )
            }
        }
    };
    'NRules.Integration.Autofac' = @{
        name = 'NRules.Integration.Autofac'
        src_root = 'src/NRules.Integration'
        restore = @{
            tool = 'dotnet'
        }
        build = @{
            tool = 'dotnet'
        }
        bin = @{
            frameworks = @('net45')
            'net45' = @{
                include = @(
                    "NRules.Integration.Autofac\bin\$configuration\net45"
                )
            }
        }
        package = @{
            nuget = @(
                'NRules.Integration.Autofac'
            )
        }
    };
    'NRules.Integration' = @{
        name = 'NRules.Integration'
        help = 'NRules.Integration.shfbproj'
    };
    'Samples.SimpleRules' = @{
        name = 'SimpleRules'
        src_root = 'samples'
        build = @{
            tool = 'msbuild'
        }
    };
    'Samples.MissManners' = @{
        name = 'MissManners'
        src_root = 'samples'
        build = @{
            tool = 'msbuild'
        }
    };
    'Samples.RuleBuilder' = @{
        name = 'RuleBuilder'
        src_root = 'samples'
        build = @{
            tool = 'msbuild'
        }
    };
    'Samples.ClaimsAdjudication' = @{
        name = 'ClaimsAdjudication'
        src_root = 'samples'
        restore = @{
            tool = 'nuget'
        }
        build = @{
            tool = 'msbuild'
        }
    };
}

$core = @('NRules', 'NRules.Debugger.Visualizer')
$integration = $components.keys | where { $_.StartsWith("NRules.Integration") }
$samples = $components.keys | where { $_.StartsWith("Samples.") }

$component_list = @()
if ($component_name -eq "Core") {
    $component_list += $core
} elseif ($component_name -eq "Integration") {
    $component_list += $integration
} elseif ($component_name -eq "Samples") {
    $component_list += $samples
} elseif ($component_name -eq "All") {
    $component_list += $core
    $component_list += $integration
    $component_list += $samples
} else {
    $component_list += $component_name
}

Import-Module .\tools\build\psake.psm1
$component_list | % {
    Invoke-psake .\default.ps1 $target -properties @{version=$version;configuration=$configuration} -parameters @{component=$components[$_]}
    if (-not $psake.build_success) {
        break
    }
}