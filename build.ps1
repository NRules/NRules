param (
	[string]$target = 'Build',
	[string]$component_name
)

$product_version = '0.2'
$build_number = '4'
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
}

$component_list = @()
if ($component_name) {
	$component_list += $component_name
} else {
	$component_list += @('NRules', 'NRules.Debugger.Visualizer')
	$component_list += $components.keys | where { $_.StartsWith("Samples.") }
}

Import-Module .\tools\build\psake.psm1
$component_list | % {
	Invoke-psake .\default.ps1 $target -properties @{version=$version} -parameters @{component=$components[$_]}
	if (-not $psake.build_success) {
		break
	}
}
