# Rule Dependecies

In a production rules engine rules consist of conditions that match facts in the rules engine's memory, and actions that insert, update or retract facts.
In a real software project, however, this is not enough. Rules also need ability to interact with the rest of the application. This can be achieved in several ways. 

Since rules are regular .NET classes, they could just access various singleton and static services. This is obviously less than desirable, since the use of singletons results in tightly-coupled designs and is generally discouraged.

A much better design is to resolve dependencies via a DI container. And one option here is to resolve rule types via a container and inject dependencies into them (see [Rule Activation](fluent-rules-loading.md#rule-activation)).
This however presents a problem. Rule classes are instantiated only once in the lifetime of the application, and therefore can only be injected with single-instance services. This may be sufficient for some applications, but when it's not enough there is another alternative.

Rules can declare their dependencies using fluent DSL, and rules engine will dynamically resolve those dependencies at runtime.
The benefit of this approach is that dependencies are resolved every time the rule fires, which means that the lifetime of the dependencies is now managed by the container.

```c#
public class DeniedClaimNotificationRule : Rule
{
    public override void Define()
    {
        INotificationService service = default!;
        Claim claim = default!;

        Dependency()
            .Resolve(() => service);

        When()
            .Match(() => claim, c => c.Status == ClaimStatus.Denied);

        Then()
            .Do(_ => service.ClaimDenied(claim));
    }
}
```

When declaring a dependency using rules DSL, the dependency is bound to a variable in the same exact way the patterns are bound to fact variables.
The service variable can then be used in rule actions. The rules engine will inject the dependency when the rule fires.

> [!WARNING]
> Rule dependencies cannot be used in rule conditions.

In order to be able to use rule dependencies, one must implement [IDependencyResolver](xref:NRules.Extensibility.IDependencyResolver) interface and set resolver instance either at the [ISession](xref:NRules.ISession) or at the [ISessionFactory](xref:NRules.ISessionFactory) level.

## .NET Dependency Injection Integration
NRules ships with the implementation of [IDependencyResolver](xref:NRules.Extensibility.IDependencyResolver) as well as [IRuleActivator](xref:NRules.Fluent.IRuleActivator) for .NET built-in IoC container in a separate integration assembly (see [NRules.Integration.DependencyInjection](xref:NRules.Integration.DependencyInjection)).
With the integration package, the following fully bootstraps and registers NRules with the .NET service collection. 

```c#
Host.CreateDefaultBuilder(args)  
    .ConfigureServices((context, services) =>
    {
        services.AddRules(x => x.AssemblyOf(typeof(MyRule)));
    });
```

## Autofac Integration
NRules ships with the implementation of [IDependencyResolver](xref:NRules.Extensibility.IDependencyResolver) as well as [IRuleActivator](xref:NRules.Fluent.IRuleActivator) for Autofac IoC container in a separate integration assembly (see [NRules.Integration.Autofac](xref:NRules.Integration.Autofac)).
With the integration package, the following fully bootstraps and registers NRules with Autofac container. Registration extensions return registration builders that allow customization of individual registrations.

```c#
var types = builder.RegisterRules(x => x.AssemblyOf(typeof(MyRule)));
builder.RegisterRepository(r => r.Load(x => x.From(types)));
builder.RegisterSessionFactory();
builder.RegisterSession();
```
