#!/usr/bin/env pwsh

param (
    [string]$target = 'Build',
    [string]$component = 'Core'
)

& ./build.ps1 $target $component
