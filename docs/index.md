# Rules Engine for .NET

NRules is an open source rules engine for .NET that is based on the [Rete](http://en.wikipedia.org/wiki/Rete_algorithm) matching algorithm. Rules are authored in C# using internal DSL.

NRules is also an inference engine, where, unlike with scripting engines, there is no predefined order in which rules are executed.
Instead, inference engine figures out which rules should be activated based on the facts given to it, and then executes them according to a conflict resolution algorithm. Among other features, NRules supports forward chaining, complex fact queries, negative, existential and universal quantifiers. 

To learn more, go to the corresponding sections of the NRules documentation.

## Getting Started with NRules

To install NRules, get the package from nuget
[![NuGet](https://img.shields.io/nuget/v/NRules.svg)](https://nuget.org/packages/NRules)

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

## Getting Help

Use the following discussion and Q&A platforms to get help with NRules

- [Discussions](https://github.com/NRules/NRules/discussions)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/nrules)
- [Gitter Chat](https://gitter.im/NRules/NRules)

## Contributing

Clone NRules on [GitHub](https://github.com/NRules/NRules).

See [Contributor Guide](https://github.com/NRules/NRules/blob/main/CONTRIBUTING.md) for the guidelines on how to contribute to the project.
