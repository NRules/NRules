param (
    [string]$target = 'Build',
    [string]$component = 'Core'
)

$version = '0.9.4'
$configuration = 'Release'

if (Test-Path Env:CI) { $version = $Env:APPVEYOR_BUILD_VERSION }
if (Test-Path Env:CI) { $configuration = $Env:CONFIGURATION }

$components = @{
    'NRules' = @{
        name = 'NRules'
        solution_file = 'src\NRules\NRules.sln'
        package = @{
            bin = @{
                artifacts = @('netstandard2.0', 'netstandard2.1')
                'netstandard2.0' = @{
                    include = @(
                        "NRules\bin\$configuration\netstandard2.0",
                        "NRules.Fluent\bin\$configuration\netstandard2.0",
                        "NRules.RuleModel\bin\$configuration\netstandard2.0",
                        "NRules.Json\bin\$configuration\netstandard2.0",
                        "NRules.Testing\bin\$configuration\netstandard2.0"
                    )
                }
                'netstandard2.1' = @{
                    include = @(
                        "NRules\bin\$configuration\netstandard2.1",
                        "NRules.Fluent\bin\$configuration\netstandard2.1",
                        "NRules.RuleModel\bin\$configuration\netstandard2.1",
                        "NRules.Json\bin\$configuration\netstandard2.1",
                        "NRules.Testing\bin\$configuration\netstandard2.1"
                    )
                }
            }
            nuget = @(
                'NRules.Json',
                'NRules.RuleModel',
                'NRules.Fluent',
                'NRules.Runtime',
                'NRules',
                'NRules.Testing'
            )
        }
    };
    'NRules.Debugger.Visualizer' = @{
        name = 'NRules.Debugger.Visualizer'
        solution_file = 'src\NRules.Debugger.Visualizer\NRules.Debugger.Visualizer.sln'
        package = @{
            bin = @{
                artifacts = @('debugger-side', 'debuggee-side-netstandard2.0')
                'debugger-side' = @{
                    include = @(
                        "NRules.Debugger.Visualizer\bin\$configuration\net472\NRules.Debugger.Visualizer.dll"
                    )
                    output = "."
                }
                'debuggee-side-netstandard2.0' = @{
                    include = @(
                        "NRules.Debugger.Visualizer.DebuggeeSide\bin\$configuration\netstandard2.0\NRules.Debugger.Visualizer.DebuggeeSide.dll"
                    )
                    output = "netstandard2.0"
                }
            }
        }
    };
    'NRules.Integration.Autofac' = @{
        name = 'NRules.Integration.Autofac'
        solution_file = 'src\NRules.Integration\NRules.Integration.Autofac\NRules.Integration.Autofac.sln'
        package = @{
            bin = @{
                artifacts = @('netstandard2.0', 'netstandard2.1')
                'netstandard2.0' = @{
                    include = @(
                        "NRules.Integration.Autofac\bin\$configuration\netstandard2.0"
                    )
                }
                'netstandard2.1' = @{
                    include = @(
                        "NRules.Integration.Autofac\bin\$configuration\netstandard2.1"
                    )
                }
            }
            nuget = @(
                'NRules.Integration.Autofac'
            )
        }
    };
    'Samples.SimpleRules' = @{
        name = 'SimpleRules'
        solution_file = 'samples\SimpleRules\SimpleRules.sln'
    };
    'Samples.MissManners' = @{
        name = 'MissManners'
        solution_file = 'samples\MissManners\MissManners.sln'
    };
    'Samples.RuleBuilder' = @{
        name = 'RuleBuilder'
        solution_file = 'samples\RuleBuilder\RuleBuilder.sln'
    };
    'Benchmark' = @{
        name = 'NRules.Benchmark'
        solution_file = 'bench\NRules.Benchmark\NRules.Benchmark.sln'
        package = @{
            bin = @{
                artifacts = @('net6', 'net8')
                'net6' = @{
                    include = @(
                        "NRules.Benchmark\bin\$configuration\net6"
                    )
                }
                'net8' = @{
                    include = @(
                        "NRules.Benchmark\bin\$configuration\net8"
                    )
                }
            }
        }
        bench = @{
            frameworks = @('net8')
            runner = 'NRules.Benchmark'
            categories = @('Micro')
        }
    };
    'Documentation' = @{
        name = 'Documentation'
        doc = @{
            shfb = @{
                project_file = 'doc\NRules.shfbproj'
            }
        }
    };
}

$core = @('NRules')
$visualizer = @('NRules.Debugger.Visualizer')
$integration = $components.keys | Where-Object { $_.StartsWith("NRules.Integration") }
$samples = $components.keys | Where-Object { $_.StartsWith("Samples.") }

$componentList = @()
if ($component -eq "Core") {
    $componentList += $core
} elseif ($component -eq "Visualizer") {
    $componentList += $visualizer
} elseif ($component -eq "Integration") {
    $componentList += $integration
} elseif ($component -eq "Samples") {
    $componentList += $samples
} elseif ($component -eq "All") {
    $componentList += $core
    $componentList += $visualizer
    $componentList += $integration
    $componentList += $samples
} else {
    $componentList += $component
}

Import-Module .\tools\build\psake.psm1
$baseDir = Resolve-Path .
$componentList | ForEach-Object {
    Invoke-psake .\tools\build\psakefile.ps1 $target -properties @{version=$version;configuration=$configuration;baseDir=$baseDir} -parameters @{component=$components[$_]}
    if (-not $psake.build_success) {
        break
    }
}