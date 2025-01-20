using System;
using System.Collections.Generic;
using System.Threading;
using NRules.Extensibility;
using NRules.RuleModel;

namespace NRules;

internal interface IActionContext : IContext
{
    ICompiledRule CompiledRule { get; }
    Activation Activation { get; }
    bool IsHalted { get; }
}

internal class ActionContext(ISessionInternal session, Activation activation, CancellationToken cancellationToken)
    : IActionContext
{
    public IRuleDefinition Rule => CompiledRule.Definition;
    public IMatch Match => Activation;
    public ICompiledRule CompiledRule => Activation.CompiledRule;

    public Activation Activation { get; } = activation;
    public CancellationToken CancellationToken { get; } = cancellationToken;
    public bool IsHalted { get; private set; }

    public void Halt()
    {
        IsHalted = true;
    }

    public void Insert(object fact)
    {
        session.Insert(fact);
    }

    public void InsertAll(IEnumerable<object> facts)
    {
        session.InsertAll(facts);
    }

    public bool TryInsert(object fact)
    {
        return session.TryInsert(fact);
    }

    public void Update(object fact)
    {
        session.Update(fact);
    }

    public void UpdateAll(IEnumerable<object> facts)
    {
        session.UpdateAll(facts);
    }

    public bool TryUpdate(object fact)
    {
        return session.TryUpdate(fact);
    }

    public void Retract(object fact)
    {
        session.Retract(fact);
    }

    public void RetractAll(IEnumerable<object> facts)
    {
        session.RetractAll(facts);
    }

    public bool TryRetract(object fact)
    {
        return session.TryRetract(fact);
    }

    public IReadOnlyCollection<object> GetLinkedKeys()
    {
        return session.GetLinkedKeys(Activation);
    }

    public object? GetLinked(object key)
    {
        return session.GetLinked(Activation, key);
    }

    public void InsertLinked(object key, object fact)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        if (fact == null)
            throw new ArgumentNullException(nameof(fact));
        
        var keyedFact = new KeyValuePair<object, object>(key, fact);
        InsertAllLinked(new[] {keyedFact});
    }

    public void InsertAllLinked(IEnumerable<KeyValuePair<object, object>> keyedFacts)
    {
        session.QueueInsertLinked(Activation, keyedFacts);
    }

    public void UpdateLinked(object key, object fact)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        if (fact == null)
            throw new ArgumentNullException(nameof(fact));

        var keyedFact = new KeyValuePair<object, object>(key, fact);
        UpdateAllLinked(new[] {keyedFact});
    }

    public void UpdateAllLinked(IEnumerable<KeyValuePair<object, object>> keyedFacts)
    {
        session.QueueUpdateLinked(Activation, keyedFacts);
    }

    public void RetractLinked(object key, object fact)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        if (fact == null)
            throw new ArgumentNullException(nameof(fact));

        var keyedFact = new KeyValuePair<object, object>(key, fact);
        RetractAllLinked(new[] {keyedFact});
    }

    public void RetractAllLinked(IEnumerable<KeyValuePair<object, object>> keyedFacts)
    {
        session.QueueRetractLinked(Activation, keyedFacts);
    }

    public object Resolve(Type serviceType)
    {
        var resolutionContext = new ResolutionContext(session, Rule);
        var service = session.DependencyResolver.Resolve(resolutionContext, serviceType);
        return service;
    }
}
