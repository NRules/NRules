param (
	[hashtable] $component
)

properties {
	$version = $null
	$sdkVersion = "1.0.1"
	$nugetVersion = "4.1.0"
	$configuration = "Release"
}

$base_dir  = resolve-path .
$tools_dir = "$base_dir\tools"

include $tools_dir\build\buildutils.ps1

task default -depends Build

task Init {
	Assert ($version -ne $null) 'Version should not be null'
	Assert ($component -ne $null) 'Component should not be null'
	
	Write-Host "Building $($component.name) version $version ($configuration)" -ForegroundColor Green
	
	$comp_name = $component.name
	$src_root = if ($component.ContainsKey('src_root')) { $component.src_root } else { 'src' }
	
	$script:binaries_dir = "$base_dir\binaries\$comp_name"
	$script:src_dir = "$base_dir\$src_root\$comp_name"
	$script:build_dir = "$base_dir\build"
	$script:out_dir =  "$build_dir\output\$comp_name"
	$script:merge_dir = "$build_dir\merge\$comp_name"
	$script:pkg_out_dir = "$build_dir\packages\$comp_name"
	$script:packages_dir = "$base_dir\packages"
	$script:help_dir = "$base_dir\help"
	
	$script:ilmerge_exec = "$tools_restore_dir\ilmerge.2.14.1203\content\ilmerge.exe"
	
	Install-DotNetCli $tools_dir\.dotnet $sdkVersion
	Install-NuGet $tools_dir\.nuget $nugetVersion
}

task Clean -depends Init {
	Delete-Directory $out_dir
	Delete-Directory $merge_dir
	Delete-Directory $pkg_out_dir
	Delete-Directory $binaries_dir
}

task SetVersion {
	Write-Host Build Version: $version
	Update-AssemblyVersion $version
}

task ResetVersion {
	Reset-AssemblyVersion
}

task RestoreDependencies -precondition { return $component.ContainsKey('restore') } {
	if ($component.restore.tool -eq 'nuget') {
		exec { nuget restore $src_dir -NonInteractive }
	}
	if ($component.restore.tool -eq 'dotnet') {
		exec { dotnet restore $src_dir --verbosity minimal }
	}
}

task Compile -depends Init, Clean, SetVersion, RestoreDependencies { 
	Create-Directory $build_dir
	
	$solution_file = "$src_dir\$($component.name).sln"
	if ($component.build.tool -eq 'msbuild') {
		exec { dotnet msbuild $solution_file /p:Configuration=$configuration /v:m /nologo }
	}
	if ($component.build.tool -eq 'dotnet') {
		exec { dotnet build $solution_file --configuration $configuration --verbosity minimal }
	}
}

task Test -depends Compile -precondition { return $component.ContainsKey('test') } {
	$tests_dir = "$src_dir\$($component.test.location)"
	$projects = Get-DotNetProjects $tests_dir
	foreach($project in $projects) {
		Push-Location $project
		exec { dotnet test --no-build --configuration $configuration --framework net46 --verbosity minimal }
		Pop-Location
	}
}

task Merge -depends Compile -precondition { return $component.ContainsKey('merge') } {
	Create-Directory $merge_dir
	
	$assemblies = @()
	$assemblies += Get-ChildItem "$out_dir\*.*" -Include $component.merge.include -Exclude $component.merge.exclude
	
	$attribute_file = "$out_dir\$($component.merge.attr_file)"
	
	$keyfile = "$base_dir\..\SigningKey.snk"
	if (-not (Test-Path $keyfile) ) {
		Write-Host "Key file for assembly signing does not exist. Signing with a development key." -ForegroundColor Yellow
		$keyfile = "$base_dir\DevSigningKey.snk"
	}
	
	$output = "$merge_dir\$($component.merge.out_file)"
	exec { &$script:ilmerge_exec /out:$output /log /keyfile:$keyfile $script:ilmerge_target_framework $assemblies /xmldocs /attr:$attribute_file }
}

task Build -depends Compile, Test, Merge, ResetVersion -precondition { return $component.ContainsKey('build') } {
    if (-not $component.ContainsKey('bin'))	{
		return
	}
	Create-Directory $binaries_dir
	foreach ($framework in $component.bin.frameworks) {
		$dest_dir = "$binaries_dir\$framework"
		Create-Directory $dest_dir
		foreach ($include_dir in $component.bin.$framework.include) {
			$source_dir = "$src_dir\$include_dir"
			Get-ChildItem "$source_dir\**" | Copy-Item -Destination $dest_dir -Force
		}
	}
}

task PackageNuGet -depends Build -precondition { return $component.ContainsKey('package') -and $component.package.ContainsKey('nuget') } {
	$nuget = $component.package.nuget
	
	Create-Directory $pkg_out_dir
	Create-Directory $pkg_out_dir\$($nuget.id)\lib\net40
	
	Copy-Item $packages_dir\$($nuget.id).dll.nuspec $pkg_out_dir\$($nuget.id)\$($nuget.id).nuspec -Force
	Get-ChildItem "$binaries_dir\**" -Include $nuget.include -Exclude $nuget.exclude | Copy-Item -Destination $pkg_out_dir\$($nuget.id)\lib\net40 -Force

	# Set the package version
	$package = "$pkg_out_dir\$($nuget.id)\$($nuget.id).nuspec"
	$nuspec = [xml](Get-Content $package)
	$nuspec.package.metadata.version = $version
	$nuspec | Select-Xml '//dependency' |% {
		$_.Node.Version = $_.Node.Version -replace '\$version\$', "$version"
	}
	$nuspec.Save($package);
	exec { &$script:nuget_exec pack $package -OutputDirectory $pkg_out_dir }
}

task Package -depends Build, PackageNuGet {
}

task PublishNuGet -precondition { return $component.ContainsKey('package') -and $component.package.ContainsKey('nuget') } {
	$nuget = $component.package.nuget
	# Upload packages
	$accessKeyFile = "$base_dir\..\Nuget-Access-Key.txt"
	if ( (Test-Path $accessKeyFile) ) {
		$accessKey = Get-Content $accessKeyFile
		$accessKey = $accessKey.Trim()
		
		# Push to nuget repository
		exec { &$script:nuget_exec push "$pkg_out_dir\$($nuget.id).$version.nupkg" $accessKey }
	} else {
		Write-Host "Nuget-Access-Key.txt does not exist. Cannot publish the nuget package." -ForegroundColor Yellow
	}
}

task Publish -depends Package, PublishNuGet {
}

task Help -depends Init, Build -precondition { return $component.ContainsKey('help') } {
	Assert (Test-Path Env:\SHFBROOT) 'Sandcastle root environment variable SHFBROOT is not set'
	
	Create-Directory $build_dir
	
	$help_proj_file = "$help_dir\$($component.help)"
	exec { &$script:msbuild_exec $help_proj_file /v:m /nologo }
}
