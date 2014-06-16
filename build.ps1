param (
	[string]$target = 'Build',
	[string]$component_name
)

$product_version = '0.1'
$build_number = '9'
$target_framework = 'net-4.0'
$configuration = 'Release'

$version = "$product_version.$build_number"

$components = @{
	'NRules' = @{
		name = 'NRules'
		merge = @{
			include = @('NRules*.dll')
			exclude = @('**Tests.dll')
			attr_file = 'NRules.dll'
			out_file = 'NRules.dll'
		}
		bin = @{
			merge_include = @('NRules.*')
			out_include = @('Common.Logging.*')
		}
		package = @{
			nuget = @{
				id = 'NRules'
				include = @('NRules.*')
			}
		}
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
	}
}

$component_list = @()
if ($component_name) {
	$component_list += $component_name
} else {
	$component_list += @('NRules', 'NRules.Debugger.Visualizer')
}

Import-Module .\tools\build\psake.psm1
$component_list | % {
	Invoke-psake .\default.ps1 $target -properties @{version=$version} -parameters @{component=$components[$_]}
}
