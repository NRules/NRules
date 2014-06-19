param (
	[hashtable] $component
)

properties {
	$version = $null
	$target_framework = "net-4.0"
	$configuration = "Release"
}

$base_dir  = resolve-path .
$tools_dir = "$base_dir\tools"

include $tools_dir\build\buildutils.ps1

task default -depends Build

task Init {
	Assert ($version -ne $null) 'Version should not be null'
	Assert ($component -ne $null) 'Component should not be null'
	
	Write-Host "Building $($component.name) version $version" -ForegroundColor Green
	
	$comp_name = $component.name
	$script:binaries_dir = "$base_dir\binaries\$comp_name"
	$script:src_dir = "$base_dir\src\$comp_name"
	$script:build_dir = "$base_dir\build"
	$script:out_dir =  "$build_dir\output\$comp_name"
	$script:merge_dir = "$build_dir\merge\$comp_name"
	$script:nuget_dir = "$build_dir\NuGet\$comp_name"
	$script:packages_dir = "$base_dir\packages"
	
	$script:nuget_exec = "$tools_dir\NuGet\nuget.exe"
	$script:zip_exec = "$tools_dir\7-zip\7za.exe"
	$script:ilmerge_exec = "$tools_dir\IlMerge\ilmerge.exe"
	
	if ($target_framework -eq "net-4.0") {
		$framework_root = Get-RegistryValue 'HKLM:\SOFTWARE\Microsoft\.NETFramework\' 'InstallRoot' 
		$framework_root = $framework_root + "v4.0.30319"
		$script:msbuild_exec = $framework_root + "\msbuild.exe"
		$script:ilmerge_target_framework  = "/targetplatform:v4," + $framework_root
	}
}

task Clean -depends Init {
	Delete-Directory $out_dir
	Delete-Directory $merge_dir
	Delete-Directory $nuget_dir
	Delete-Directory $binaries_dir
}

task SetVersion {
	Write-Host Build Version: $version
	Update-AssemblyVersion $version
}

task ResetVersion {
	Reset-AssemblyVersion
}

task RestoreDependencies { 
	exec { &$script:nuget_exec restore $src_dir -NonInteractive }
}

task Compile -depends Init, Clean, SetVersion, RestoreDependencies {
	Create-Directory $build_dir
	Create-Directory $out_dir
	
	$solution_file = "$src_dir\$($component.name).sln"
	$output = "$out_dir\"
	exec { &$script:msbuild_exec $solution_file /p:OutDir=$output /p:Configuration=$configuration /v:m /nologo }
}

task Merge -depends Compile -precondition { return $component.ContainsKey('merge') } {
	Create-Directory $merge_dir
	
	$assemblies = @()
	$assemblies += Get-ChildItem "$out_dir\*.*" -Include $component.merge.include -Exclude $component.merge.exclude
	
	$attribute_file = "$out_dir\$($component.merge.attr_file)"
	
	$keyfile = "$base_dir\..\SigningKey.snk"
	$keyfileOption = "/keyfile:$keyfile"
	if (-not (Test-Path $keyfile) ) {
		$keyfileOption = ""
		Write-Host "Key file for assembly signing does not exist. Cannot strongly name assembly." -ForegroundColor Yellow
	}
	
	$output = "$merge_dir\$($component.merge.out_file)"
	exec { &$script:ilmerge_exec /out:$output /log $keyfileOption $script:ilmerge_target_framework $assemblies /xmldocs /attr:$attribute_file }
}

task Build -depends Compile, Merge, ResetVersion {
	Create-Directory $binaries_dir
	
	if ($component.ContainsKey('merge') -and $component.bin.merge_include) {
		Get-ChildItem "$merge_dir\**" -Include $component.bin.merge_include -Exclude $component.bin.merge_exclude | Copy-Item -Destination $binaries_dir -Force
	}
	if ($component.ContainsKey('bin') -and $component.bin.out_include) {
		Get-ChildItem "$out_dir\**" -Include $component.bin.out_include -Exclude $component.bin.out_exclude | Copy-Item -Destination $binaries_dir -Force
	}
}

task PackageNuGet -depends Build -precondition { return $component.package.ContainsKey('nuget') } {
	$nuget = $component.package.nuget
	
	Create-Directory $nuget_dir
	Create-Directory $nuget_dir\$($nuget.id)\lib\net40
	
	Copy-Item $packages_dir\$($nuget.id).dll.nuspec $nuget_dir\$($nuget.id)\$($nuget.id).nuspec -Force
	Get-ChildItem "$binaries_dir\**" -Include $nuget.include -Exclude $nuget.exclude | Copy-Item -Destination $nuget_dir\$($nuget.id)\lib\net40 -Force

	# Set the package version
	$package = "$nuget_dir\$($nuget.id)\$($nuget.id).nuspec"
	$nuspec = [xml](Get-Content $package)
	$nuspec.package.metadata.version = $version
	$nuspec | Select-Xml '//dependency' |% {
		if($_.Node.Id.StartsWith($nuget.id)) {
			$_.Node.Version = "[$version]"
		}
	}
	$nuspec.Save($package);
	exec { &$script:nuget_exec pack $package -OutputDirectory $nuget_dir }
}

task PackageZip -depends Build -precondition { $component.package.ContainsKey('zip') } {
	$zip = $component.package.zip
	$file = "$script:packages_dir\$($zip.name)"
	Delete-File $file
	exec { &$script:zip_exec a -tzip $file $binaries_dir }
}

task Package -depends Build, PackageNuGet, PackageZip {
}

task PublishNuGet -precondition { return $component.package.ContainsKey('nuget') } {
	$nuget = $component.package.nuget
	# Upload packages
	$accessKeyFile = "$base_dir\..\Nuget-Access-Key.txt"
	if ( (Test-Path $accessKeyFile) ) {
		$accessKey = Get-Content $accessKeyFile
		$accessKey = $accessKey.Trim()
		
		# Push to nuget repository
		exec { &$script:nuget_exec push "$nuget_dir\$($nuget.id).$version.nupkg" $accessKey }
	} else {
		Write-Host "Nuget-Access-Key.txt does not exist. Cannot publish the nuget package." -ForegroundColor Yellow
	}
}

task Publish -depends Package, PublishNuGet {
}
