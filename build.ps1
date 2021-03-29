param (
    [string]$target = 'Build',
    [string]$component = 'Core'
)

$version = '0.9.3'
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
            frameworks = @('net48', 'netcoreapp3.1')
        }
        bin = @{
            artifacts = @('netstandard2.0', 'netstandard2.1')
            'netstandard2.0' = @{
                include = @(
                    "NRules\bin\$configuration\netstandard2.0",
                    "NRules.Fluent\bin\$configuration\netstandard2.0",
                    "NRules.RuleModel\bin\$configuration\netstandard2.0"
                )
            }
            'netstandard2.1' = @{
                include = @(
                    "NRules\bin\$configuration\netstandard2.1",
                    "NRules.Fluent\bin\$configuration\netstandard2.1",
                    "NRules.RuleModel\bin\$configuration\netstandard2.1"
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
            tool = 'dotnet'
        }
    };
    'Samples.MissManners' = @{
        name = 'MissManners'
        src_root = 'samples'
        build = @{
            tool = 'dotnet'
        }
    };
    'Samples.RuleBuilder' = @{
        name = 'RuleBuilder'
        src_root = 'samples'
        build = @{
            tool = 'dotnet'
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
            artifacts = @('net48', 'netcoreapp3.1')
            'net48' = @{
                include = @(
                    "NRules.Benchmark\bin\$configuration\net48"
                )
            }
            'netcoreapp3.1' = @{
                include = @(
                    "NRules.Benchmark\bin\$configuration\netcoreapp3.1"
                )
            }
        }
        bench = @{
            frameworks = @('net48')
            exe = 'NRules.Benchmark.exe'
            categories = @('Micro')
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

$core = @('NRules')
$visualizer = @('NRules.Debugger.Visualizer')
$integration = $components.keys | where { $_.StartsWith("NRules.Integration") }
$samples = $components.keys | where { $_.StartsWith("Samples.") }

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
$componentList | % {
    Invoke-psake .\tools\build\psakefile.ps1 $target -properties @{version=$version;configuration=$configuration;baseDir=$baseDir} -parameters @{component=$components[$_]}
    if (-not $psake.build_success) {
        break
    }
}