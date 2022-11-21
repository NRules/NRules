using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NRules.Fluent.Dsl;

namespace NRules.Fluent;

/// <summary>
/// Assembly scanner that finds fluent rule classes.
/// </summary>
public interface IRuleTypeScanner
{
    /// <summary>
    /// Enables/disables discovery of private rule classes.
    /// Default is off.
    /// </summary>
    /// <param name="include">Include private types if <c>true</c>, don't include otherwise.</param>
    /// <returns>Rule type scanner to continue scanning specification.</returns>
    IRuleTypeScanner PrivateTypes(bool include = true);

    /// <summary>
    /// Enables/disables discovery of nested rule classes.
    /// Default is off.
    /// </summary>
    /// <param name="include">Include nested types if <c>true</c>, don't include otherwise.</param>
    /// <returns>Rule type scanner to continue scanning specification.</returns>
    IRuleTypeScanner NestedTypes(bool include = true);

    /// <summary>
    /// Finds rule types in the specified assemblies.
    /// </summary>
    /// <param name="assemblies">Assemblies to scan.</param>
    /// <returns>Rule type scanner to continue scanning specification.</returns>
    IRuleTypeScanner Assembly(params Assembly[] assemblies);

    /// <summary>
    /// Finds rule types in the assembly of the specified type.
    /// </summary>
    /// <typeparam name="T">Type, whose assembly to scan.</typeparam>
    /// <returns>Rule type scanner to continue scanning specification.</returns>
    IRuleTypeScanner AssemblyOf<T>();

    /// <summary>
    /// Finds rule types in the assembly of the specified type.
    /// </summary>
    /// <param name="type">Type, whose assembly to scan.</param>
    /// <returns>Rule type scanner to continue scanning specification.</returns>
    IRuleTypeScanner AssemblyOf(Type type);

    /// <summary>
    /// Finds rule types in the specifies types.
    /// </summary>
    /// <param name="types">Types to scan.</param>
    /// <returns>Rule type scanner to continue scanning specification.</returns>
    IRuleTypeScanner Type(params Type[] types);
}

/// <summary>
/// Assembly scanner that finds fluent rule classes.
/// </summary>
public class RuleTypeScanner : IRuleTypeScanner
{
    private readonly List<Type> _ruleTypes = new();
    private bool _nestedTypes = false;
    private bool _privateTypes = false;

    /// <summary>
    /// Enables/disables discovery of private rule classes.
    /// Default is off.
    /// </summary>
    /// <param name="include">Include private types if <c>true</c>, don't include otherwise.</param>
    /// <returns>Rule type scanner to continue scanning specification.</returns>
    public IRuleTypeScanner PrivateTypes(bool include = true)
    {
        _privateTypes = include;
        return this;
    }

    /// <summary>
    /// Enables/disables discovery of nested rule classes.
    /// Default is off.
    /// </summary>
    /// <param name="include">Include nested types if <c>true</c>, don't include otherwise.</param>
    /// <returns>Rule type scanner to continue scanning specification.</returns>
    public IRuleTypeScanner NestedTypes(bool include = true)
    {
        _nestedTypes = include;
        return this;
    }

    /// <summary>
    /// Finds rule types in the specified assemblies.
    /// </summary>
    /// <param name="assemblies">Assemblies to scan.</param>
    /// <returns>Rule type scanner to continue scanning specification.</returns>
    public IRuleTypeScanner Assembly(params Assembly[] assemblies)
    {
        var ruleTypes = assemblies.SelectMany(a => a.DefinedTypes.Where(IsRuleType));
        _ruleTypes.AddRange(ruleTypes);
        return this;
    }

    /// <summary>
    /// Finds rule types in the assembly of the specified type.
    /// </summary>
    /// <typeparam name="T">Type, whose assembly to scan.</typeparam>
    /// <returns>Rule type scanner to continue scanning specification.</returns>
    public IRuleTypeScanner AssemblyOf<T>()
    {
        return AssemblyOf(typeof(T));
    }

    /// <summary>
    /// Finds rule types in the assembly of the specified type.
    /// </summary>
    /// <param name="type">Type, whose assembly to scan.</param>
    /// <returns>Rule type scanner to continue scanning specification.</returns>
    public IRuleTypeScanner AssemblyOf(Type type)
    {
        return Assembly(type.Assembly);
    }

    /// <summary>
    /// Finds rule types in the specifies types.
    /// </summary>
    /// <param name="types">Types to scan.</param>
    /// <returns>Rule type scanner to continue scanning specification.</returns>
    public IRuleTypeScanner Type(params Type[] types)
    {
        var ruleTypes = types
            .Where(IsRuleType);
        _ruleTypes.AddRange(ruleTypes);
        return this;
    }

    /// <summary>
    /// Retrieves found types.
    /// </summary>
    /// <returns>Rule types.</returns>
    public Type[] GetRuleTypes()
    {
        var ruleTypes = _ruleTypes
            .Where(t => _privateTypes || !t.IsNotPublic)
            .Where(t => _nestedTypes || !t.IsNested);
        return ruleTypes.ToArray();
    }

    /// <summary>
    /// Determines if a given CLR type is a rule type.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>Result of the check.</returns>
    public static bool IsRuleType(Type type)
    {
        var ruleType = typeof(Rule);
        if (IsConcrete(type) &&
            ruleType.IsAssignableFrom(type))
            return true;

        return false;
    }

    private static bool IsConcrete(Type type)
    {
        if (type.IsAbstract) return false;
        if (type.IsInterface) return false;
        if (type.IsGenericTypeDefinition) return false;

        return true;
    }
}