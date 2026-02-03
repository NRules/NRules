# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

NRules is an open-source cross-platform production rules engine for .NET, based on the Rete matching algorithm. Rules are authored in C# using a fluent DSL. The core libraries target netstandard2.0/netstandard2.1; tests target net6/net8.

## Build Commands

The build system uses PowerShell (psake). The `build.sh` wrapper works on Unix-like systems; `build.bat` works on Windows.

```bash
# Build core NRules (default)
./build.sh Build Core

# Build everything (core, integrations, samples, benchmarks, docs)
./build.sh Build All
```

## Test Projects

- **NRules.Tests** — Unit tests for core engine (uses xUnit, Moq)
- **NRules.IntegrationTests** — End-to-end rule execution tests (uses xUnit, NRules.Testing)
- **NRules.Json.Tests** — JSON serialization round-trip tests

## Architecture

NRules has a three-layer architecture:

### 1. Fluent DSL (`NRules.Fluent`)
Rule authoring layer. Users subclass `Rule` and define conditions/actions using a fluent API. `RuleRepository` discovers rules via assembly scanning and translates them to the canonical rule model.

### 2. Rule Model (`NRules.RuleModel`)
Canonical AST representation of rules. Key types: `IRuleDefinition`, `RuleElement`, `PatternElement`, `GroupElement`, `ActionElement`, `Declaration`. Uses the Visitor pattern (`RuleElementVisitor`) for traversal. This layer is independent of both the DSL and the runtime.

### 3. Runtime Engine (`NRules`)
Compiles the rule model into a **Rete network** and executes rules. Key flow:
- `RuleCompiler` takes `IRuleDefinition` instances and builds the Rete network, returning an `ISessionFactory`
- `ISessionFactory.CreateSession()` creates an `ISession` with its own working memory
- Facts are inserted/updated/retracted through `ISession`
- `ISession.Fire()` processes the agenda (matched rule activations) until empty

Rete network node types: `RootNode` → `TypeNode` → `AlphaMemoryNode` → `JoinNode`/`NotNode`/`ExistsNode`/`AggregateNode`/`BindingNode` → `RuleNode`.

### Supporting Libraries
- **NRules.Json** — JSON serialization of the rule model (System.Text.Json)
- **NRules.Testing** — Test harness for unit testing rules
- **NRules.Meta** — Meta package for nuget publishing
- **NRules.Integration.*** — DI container integrations (Autofac, SimpleInjector, Microsoft.Extensions.DependencyInjection)

### Key Extension Points
- `IDependencyResolver` — Inject dependencies into rules
- `IActionInterceptor` — Intercept rule action execution
- `IAgendaFilter` — Custom filters for rule activations on the agenda
- `IRuleActivator` — Custom rule instantiation
- `IExpressionCompiler` — Custom expression compilation

## Code Conventions

- **Warnings as errors** — `TreatWarningsAsErrors` is enabled in `Directory.Build.props`
- **Strong-named assemblies** — All projects sign with `SigningKey.snk`
- **Centralized package versions** — All dependency versions managed in `Directory.Packages.props`
- **Nullable reference types** enabled in test projects (`<Nullable>enable</Nullable>`)
- **C# latest** language version used in test projects
