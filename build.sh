#!/usr/bin/env pwsh

param (
    [string]$target = $null,
    [string]$component = $null
)

& ./build.ps1 $target $component
