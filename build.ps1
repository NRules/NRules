param (
	[string]$target = 'Build',
	[string]$component_name = 'Core'
)

$product_version = '0.3'
$build_number = '2'
$target_framework = 'net-4.0'
$configuration = 'Release'

$version = "$product_version.$build_number"

$components = @{
	'NRules' = @{
		name = 'NRules'
		test = @{
			include = @('*Tests.dll')
		}
		merge = @{
			include = @('NRules*.dll')
			exclude = @('**Tests.dll')
			attr_file = 'NRules.dll'
			out_file = 'NRules.dll'
		}
		bin = @{
			merge_include = @('NRules.*')
		}
		package = @{
			nuget = @{
				id = 'NRules'
				include = @('NRules.*')
			}
		}
		help = 'NRules.shfbproj'
	};
	'NRules.Debugger.Visualizer' = @{
		name = 'NRules.Debugger.Visualizer'
		bin = @{
			out_include = @('*.dll','*.pdb','*.xml')
			out_exclude = @('**Tests**','nunit**','Moq**')
		}
		package = @{
			zip = @{
				name = "NRules.Debugger.Visualizer.$version.zip"
			}
		}
	};
	'Samples.SimpleRules' = @{
		name = 'SimpleRules'
		src_root = 'samples'
		bin = @{
			out_include = @('*.*')
		}
	};
	'Samples.MissManners' = @{
		name = 'MissManners'
		src_root = 'samples'
		bin = @{
			out_include = @('*.*')
		}
	};
	'Samples.RuleBuilder' = @{
		name = 'RuleBuilder'
		src_root = 'samples'
		bin = @{
			out_include = @('*.*')
		}
	};
		'Samples.ClaimsAdjudication' = @{
		name = 'ClaimsAdjudication'
		src_root = 'samples'
		bin = @{
			out_include = @('*.*')
		}
	};
}

$core = @('NRules', 'NRules.Debugger.Visualizer')
$samples = $components.keys | where { $_.StartsWith("Samples.") }

$component_list = @()
if ($component_name -eq "Core") {
	$component_list += $core
} elseif ($component_name -eq "Samples") {
	$component_list += $samples
} elseif ($component_name -eq "All") {
	$component_list += $core
	$component_list += $samples
} else {
	$component_list += $component_name
}

Import-Module .\tools\build\psake.psm1
$component_list | % {
	Invoke-psake .\default.ps1 $target -properties @{version=$version} -parameters @{component=$components[$_]}
	if (-not $psake.build_success) {
		break
	}
}