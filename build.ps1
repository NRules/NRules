param (
    [string]$target = 'Build',
    [string]$component = 'Core'
)

$version = '0.9.0'
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
            frameworks = @('net462', 'netcoreapp1.0', 'netcoreapp2.0')
        }
        bin = @{
            frameworks = @('net45', 'netstandard1.0', 'netstandard2.0')
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
            'netstandard2.0' = @{
                include = @(
                    "NRules\bin\$configuration\netstandard2.0",
                    "NRules.Fluent\bin\$configuration\netstandard2.0",
                    "NRules.RuleModel\bin\$configuration\netstandard2.0"
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
    'Benchmark' = @{
        name = 'NRules.Benchmark'
        src_root = 'bench'
        restore = @{
            tool = 'dotnet'
        }
        build = @{
            tool = 'dotnet'
        }
        bin = @{
            frameworks = @('net462')
            'net462' = @{
                include = @(
                    "NRules.Benchmark\bin\$configuration\net462"
                )
            }
        }
        run = @{
            exe = @('net462\NRules.Benchmark.exe')
        }
    };
    'Documentation' = @{
        name = 'Documentation'
        src_root = 'doc'
        build = @{
            tool = 'shfb'
            solution_file = 'NRules.shfbproj'
        }
    };
}

$core = @('NRules', 'NRules.Debugger.Visualizer')
$integration = $components.keys | where { $_.StartsWith("NRules.Integration") }
$samples = $components.keys | where { $_.StartsWith("Samples.") }

$componentList = @()
if ($component -eq "Core") {
    $componentList += $core
} elseif ($component -eq "Integration") {
    $componentList += $integration
} elseif ($component -eq "Samples") {
    $componentList += $samples
} elseif ($component -eq "All") {
    $componentList += $core
    $componentList += $integration
    $componentList += $samples
} else {
    $componentList += $component
}

Import-Module .\tools\build\psake.psm1
$baseDir = Resolve-Path .
$componentList | % {
    Invoke-psake .\tools\build\default.ps1 $target -properties @{version=$version;configuration=$configuration;baseDir=$baseDir} -parameters @{component=$components[$_]}
    if (-not $psake.build_success) {
        break
    }
}