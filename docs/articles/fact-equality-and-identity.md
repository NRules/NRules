# Fact Equality and Identity

In NRules, it is crucial for the engine to be able to determine which facts are the same and which are different when inserting, updating, or retracting them within the rules session. By default, NRules uses the standard .NET equality comparison to achieve this, essentially treating fact equality and fact identity the same way. This works well for the majority of cases, such as when mutating the existing fact objects or where equality comparison only uses the identity fields. However, there are scenarios where the default equality comparison is insufficient. For instance, if a fact already defines its own structural equality comparison or when using record types to define facts, which inherently use structural equality comparison under the hood.

To address these scenarios, NRules provides mechanisms to customize fact identity comparison.

NRules engine uses the following rules for fact identity comparison:
* If both facts are the same reference, they are considered identical.
* If either fact is `null` (though this should not be possible), they are considered not identical.
* If facts are of different types, they are considered not identical.
* If custom comparer is registered for the fact type, it is used to compare the identity of the facts.
* In all other cases default comparer is used to compare the identity of the facts.

The default comparer compares fact identity as follows:
* If facts implement [IIdentityProvider](xref:NRules.RuleModel.IIdentityProvider), it compares the identity objects.
* Otherwise, the facts are compared using their equality.

## Explicitly Defining Fact Identity

As noted earlier, one customization mechanism for identity comparison is the [IIdentityProvider](xref:NRules.RuleModel.IIdentityProvider) interface. By implementing this interface, a fact can explicitly define its identity, which NRules will use to compare facts. This is particularly useful when the default equality comparison does not align with the desired behavior for fact comparison within the rules session.

Here is an example of how to implement the [IIdentityProvider](xref:NRules.RuleModel.IIdentityProvider) interface:

```c#
public record FactType(int Id, string TestProperty) : IIdentityProvider
{
    public object GetIdentity() => Id;
}
```

In this case the following code that updates the rules session will work as expected:

```c#
Session.Insert(new FactType(1, "Original Value"));
Session.Update(new FactType(1, "New Value"));
```

## Custom Equality Comparer

Additionally, NRules allows the use of custom [IEqualityComparer&lt;T&gt;](xref:System.Collections.Generic.IEqualityComparer`1) implementations for given fact types. This approach provides even more flexibility in defining how facts are compared for identity.

Below is an example of a custom [IEqualityComparer&lt;T&gt;](xref:System.Collections.Generic.IEqualityComparer`1) implementation. While normally, equality comparers must handle special cases of `null` arguments and equality by reference, these are checked by NRules before giving control to the equality comparer, so the arguments can be assumed not `null` and not equal by reference.

```c#
public class FactTypeIdentityComparer : IEqualityComparer<FactType>
{
    public bool Equals(FactType? obj1, FactType? obj2)
    {
        return obj1!.Id == obj2!.Id;
    }

    public int GetHashCode(FactType obj)
    {
        return obj.Id;
    }
}
```

Custom equality comparers must be registered with the engine at the time of rules compilation like this:

```c#
var compiler = new RuleCompiler();
compiler.FactIdentityComparerRegistry.RegisterComparer(new FactTypeIdentityComparer());
```

## Customizing Default Identity Comparer

In some cases the default identity comparer in NRules may be insufficient. As noted before, it uses [IIdentityProvider](xref:NRules.RuleModel.IIdentityProvider) interface, if it's implemented by the fact type, or default equality comparison otherwise. It may be desirable for fact types to not depend on any framework code or implement any infrastructure interfaces, when subscribing to the POCO paradigm for the domain model. In this case, one can define a custom implementation of the default comparer, like so:

```c#
public interface IMyIdentityProvider
{
    int GetIdentity();
}

internal class MyDefaultFactIdentityComparer : IEqualityComparer<object>
{
    bool IEqualityComparer<object>.Equals(object? obj1, object? obj2)
    {
        if (obj1 is IMyIdentityProvider provider1 && obj2 is IMyIdentityProvider provider2)
            return provider1.GetIdentity() == provider2.GetIdentity();

        return Equals(obj1, obj2);
    }

    int IEqualityComparer<object>.GetHashCode(object obj)
    {
        if (obj is IMyIdentityProvider provider)
            return provider.GetIdentity();

        return obj.GetHashCode();
    }
}
```

Custom version of the default equality comparer is then registered with the engine at the time of rules compilation like this:

```c#
var compiler = new RuleCompiler();
compiler.FactIdentityComparerRegistry.DefaultFactIdentityComparer = new MyDefaultFactIdentityComparer();
```