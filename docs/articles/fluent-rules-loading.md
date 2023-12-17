# Fluent Rules Loading

NRules fluent API not only provides a mechanism to define rules using internal DSL, 
but also to discover, instantiate and load these rules into a rule repository, so that they can be compiled into an executable form.

See [[Fluent Rules DSL]] for an overview of the fluent rules DSL.

## Loading Rules with Fluent Specification
With the fluent rules DSL, each rule is a class, and `RuleRepository` provides a mechanism to discover and load rules classes at runtime using a fluent load specification.
Method `Load` accepts an action delegate that instructs the repository which assemblies and/or types to scan to load the rules. 
It also provides advanced filtering capabilities using metadata associated with the rules classes.

Example, load all rules from the executing assembly, where rule's name starts with "Test" or the rule is tagged with the "Test" tag. Load these rules into a rule set called "MyRuleSet".
```c#
var repository = new RuleRepository();
repository.Load(x => x
    .From(Assembly.GetExecutingAssembly())
    .Where(rule => rule.Name.StartsWith("Test") || rule.IsTagged("Test"))
    .To("MyRuleSet"));
```

If the rule set with a given name already exists then `Load` method just adds rules to it.

Once rules are loaded, the whole repository, or only a subset of rule sets can be compiled to an executable form.
```c#
var ruleSets = repository.GetRuleSets();
var compiler = new RuleCompiler();
ISessionFactory factory = compiler.Compile(ruleSets.Where(x => x.Name == "MyRuleSet"));
```

## Rule Activation
As part of translating rules from fluent DSL form to the canonical model, rule classes must be instantiated.
By default NRules instantiates rule classes using `System.Activator`.
It may be desirable to have control over the instantiation process, so that, for example, rules can be instantiated via an IoC container and injected with dependencies. 

Whenever `RuleRepository` needs to instantiate a rule, it calls into a rule activator, represented by the `IRuleActivator` interface and exposed via the `RuleRepository.RuleActivator` property.
To provide a custom rule activator, implement `IRuleActivator` interface and set it on the repository.

For example, rules can be registered with and resolved via an Autofac IoC container.
```c#
public class AutofacRuleActivator : IRuleActivator
{
    private readonly ILifetimeScope _scope;
    
    public AutofacRuleActivator(ILifetimeScope scope)
    {
        _scope = scope;
    }
    
    public IEnumerable<Rule> Activate(Type type)
    {
        yield return (Rule)_scope.Resolve(type);
    }
}

//Build container
var builder = new ContainerBuilder();
builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
    .Where(t => t.IsAssignableTo<Rule>())
    .AsSelf();
var container = builder.Build();

// Load rules
var ruleRepository = new RuleRepository();
ruleRepository.Activator = new AutofacRuleActivator(container);
ruleRepository.Load(r => r.From(Assembly.GetExecutingAssembly()));

// Compile rules
var factory = ruleRepository.Compile();
```
