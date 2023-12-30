# Rules Engine for .NET

NRules is an open source rules engine for .NET that is based on the [Rete](http://en.wikipedia.org/wiki/Rete_algorithm) matching algorithm. Rules are authored in C# using internal DSL.

NRules is also an inference engine, where, unlike with scripting engines, there is no predefined order in which rules are executed.
Instead, inference engine figures out which rules should be activated based on the facts given to it, and then executes them according to a conflict resolution algorithm. Among other features, NRules supports forward chaining, complex fact queries, negative, existential and universal quantifiers. 

To learn more, go to the corresponding sections of the NRules documentation.

## Getting Started with NRules

To install NRules, get the package from nuget

# [CLI](#tab/cli)
```console
> dotnet add package NRules
```
# [Package Manager](#tab/pm)
```console
> Install-Package NRules
```
---

Use the following resources to get up and running with NRules

- [Getting Started Guide](articles/getting-started.md)
- [Documentation](articles/architecture.md)
- [API Documentation](api/index.md)

## Getting the Right Package

Choose the right package from the NRules suite, depending on your needs and the project structure

Name | Nuget | Overview | Documentation
--- | --- | --- | ---
NRules | [![NuGet](https://img.shields.io/nuget/v/NRules.svg)](https://nuget.org/packages/NRules) | Meta package that installs the canonical rules model, fluent DSL and the engine runtime. Reference this package from the projects that don't separate rules from the application code. | [Guide](articles/getting-started.md)
NRules.Fluent | [![NuGet](https://img.shields.io/nuget/v/NRules.Fluent.svg)](https://nuget.org/packages/NRules.Fluent) | Fluent DSL for NRules. Reference this package from the projects that contain rules. | [API](xref:NRules.Fluent)
NRules.Runtime | [![NuGet](https://img.shields.io/nuget/v/NRules.Runtime.svg)](https://nuget.org/packages/NRules.Runtime) | Rules engine runtime. Reference this package from the projects that compile and execute rules. | [API](xref:NRules)
NRules.RuleModel | [![NuGet](https://img.shields.io/nuget/v/NRules.RuleModel.svg)](https://nuget.org/packages/NRules.RuleModel) | Canonical rules model. Reference this package from the projects that deal with the intermediate rules representation. This package is transitively referenced by most other NRules packages. | [API](xref:NRules.RuleModel)
NRules.Json | [![NuGet](https://img.shields.io/nuget/v/NRules.Json.svg)](https://nuget.org/packages/NRules.Json) | Rules serialization to and from JSON. Reference this package from the projects that handle rules serialization. | [Guide](articles/advanced/json-serialization.md) [API](xref:NRules.Json)
NRules.Testing | [![NuGet](https://img.shields.io/nuget/v/NRules.Testing.svg)](https://nuget.org/packages/NRules.Testing) | Rules unit testing and expectations assertion. Reference this package from the projects that implement rules unit tests. | [Guide](articles/unit-testing-rules.md) [API](xref:NRules.Testing)

## Getting Help

Use the following discussion and Q&A platforms to get help with NRules

- [Discussions](https://github.com/NRules/NRules/discussions)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/nrules)
- [Gitter Chat](https://gitter.im/NRules/NRules)

## Building NRules from Sources

To build NRules from sources, clone the repo from GitHub, make sure PowerShell is installed, then run the build script
# [Windows](#tab/windows)
```console
> build.bat
```
# [MacOS/Linux](#tab/nix)
```console
> ./build.sh
```
---

Build artifacts are found under the `build` folder.

## Contributing

Clone NRules on [GitHub](https://github.com/NRules/NRules).

See [Contributor Guide](https://github.com/NRules/NRules/blob/main/CONTRIBUTING.md) for the guidelines on how to contribute to the project.
