using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NRules.Diagnostics;
using NRules.Extensibility;
using NRules.Rete;

namespace NRules;

/// <summary>
/// Represents a rules engine session. Created by <see cref="ISessionFactory"/>.
/// Each session has its own working memory, and exposes operations that 
/// manipulate facts in it, as well as fire matching rules.
/// </summary>
/// <event cref="IEventProvider.FactInsertingEvent">Before processing fact insertion.</event>
/// <event cref="IEventProvider.FactInsertedEvent">After processing fact insertion.</event>
/// <event cref="IEventProvider.FactUpdatingEvent">Before processing fact update.</event>
/// <event cref="IEventProvider.FactUpdatedEvent">After processing fact update.</event>
/// <event cref="IEventProvider.FactRetractingEvent">Before processing fact retraction.</event>
/// <event cref="IEventProvider.FactRetractedEvent">After processing fact retraction.</event>
/// <event cref="IEventProvider.ActivationCreatedEvent">When a set of facts matches a rule.</event>
/// <event cref="IEventProvider.ActivationUpdatedEvent">When a set of facts is updated and re-matches a rule.</event>
/// <event cref="IEventProvider.ActivationDeletedEvent">When a set of facts no longer matches a rule.</event>
/// <event cref="IEventProvider.RuleFiringEvent">Before rule's actions are executed.</event>
/// <event cref="IEventProvider.RuleFiredEvent">After rule's actions are executed.</event>
/// <event cref="IEventProvider.LhsExpressionEvaluatedEvent">When an left-hand side expression was evaluated.</event>
/// <event cref="IEventProvider.LhsExpressionFailedEvent">When there is an error during left-hand side expression evaluation,
/// before throwing exception to the client.</event>
/// <event cref="IEventProvider.AgendaExpressionEvaluatedEvent">When an agenda expression was evaluated.</event>
/// <event cref="IEventProvider.AgendaExpressionFailedEvent">When there is an error during agenda expression evaluation,
/// before throwing exception to the client.</event>
/// <event cref="IEventProvider.RhsExpressionEvaluatedEvent">When an right-hand side expression was evaluated.</event>
/// <event cref="IEventProvider.RhsExpressionFailedEvent">When there is an error during right-hand side expression evaluation,
/// before throwing exception to the client.</event>
/// <exception cref="RuleLhsExpressionEvaluationException">Error while evaluating any of the rules' left-hand side expressons.
/// This exception can also be observed as an event <see cref="IEventProvider.LhsExpressionEvaluatedEvent"/>.</exception>
/// <exception cref="AgendaExpressionEvaluationException">Error while evaluating any of the agenda expressions.
/// This exception can also be observed as an event <see cref="IEventProvider.AgendaExpressionFailedEvent"/>.</exception>
/// <exception cref="RuleRhsExpressionEvaluationException">Error while evaluating any of the rules' right-hand side expressions.
/// This exception can also be observed as an event <see cref="IEventProvider.RhsExpressionFailedEvent"/>.</exception>
/// <seealso cref="ISessionFactory"/>
/// <threadsafety instance="false" />
public interface ISession : ISessionSchemaProvider
{
    /// <summary>
    /// Controls how the engine propagates linked facts from rules that insert/update/retract linked facts in their actions.
    /// By default, <see cref="AutoPropagateLinkedFacts"/> is <c>true</c> and the engine automatically
    /// propagates linked facts at the end of the rule's actions.
    /// If <see cref="AutoPropagateLinkedFacts"/> is <c>false</c>, linked facts are queued, and have to be
    /// explicitly propagated by calling <see cref="PropagateLinked"/> method.
    /// </summary>
    bool AutoPropagateLinkedFacts { get; set; }

    /// <summary>
    /// Agenda, which represents a store for rule matches.
    /// </summary>
    IAgenda Agenda { get; }

    /// <summary>
    /// Provider of events from the current rule session.
    /// Use it to subscribe to various rules engine lifecycle events.
    /// </summary>
    IEventProvider Events { get; }

    /// <summary>
    /// Provider of session performance metrics for the current rule session.
    /// </summary>
    IMetricsProvider Metrics { get; }

    /// <summary>
    /// Rules dependency resolver for the current rules session.
    /// </summary>
    IDependencyResolver DependencyResolver { get; set; }

    /// <summary>
    /// Action interceptor for the current rules session.
    /// If provided, invocation of rule actions is delegated to the interceptor.
    /// </summary>
    IActionInterceptor ActionInterceptor { get; set; }

    /// <summary>
    /// Inserts new facts to the rules engine memory.
    /// </summary>
    /// <remarks>
    /// Bulk session operations are more performant than individual operations on a set of facts.
    /// </remarks>
    /// <param name="facts">Facts to insert.</param>
    /// <exception cref="ArgumentException">If any fact already exists in working memory.</exception>
    void InsertAll(IEnumerable<object> facts);

    /// <summary>
    /// Inserts new facts to the rules engine memory if the facts don't exist.
    /// If any of the facts exists in the engine, none of the facts are inserted.
    /// </summary>
    /// <remarks>
    /// Bulk session operations are more performant than individual operations on a set of facts.
    /// </remarks>
    /// <param name="facts">Facts to insert.</param>
    /// <returns>Result of facts insertion.</returns>
    IFactResult TryInsertAll(IEnumerable<object> facts);

    /// <summary>
    /// Inserts new facts to the rules engine memory if the facts don't exist.
    /// If any of the facts exists in the engine, the behavior is defined by <see cref="BatchOptions"/>.
    /// </summary>
    /// <remarks>
    /// Bulk session operations are more performant than individual operations on a set of facts.
    /// </remarks>
    /// <param name="facts">Facts to insert.</param>
    /// <param name="options">Options that define behavior of the batch operation.</param>
    /// <returns>Result of facts insertion.</returns>
    IFactResult TryInsertAll(IEnumerable<object> facts, BatchOptions options);

    /// <summary>
    /// Inserts new fact to the rules engine memory.
    /// </summary>
    /// <remarks>
    /// Bulk session operations are more performant than individual operations on a set of facts.
    /// </remarks>
    /// <param name="fact">Facts to insert.</param>
    /// <exception cref="ArgumentException">If fact already exists in working memory.</exception>
    void Insert(object fact);

    /// <summary>
    /// Inserts a fact to the rules engine memory if the fact does not exist.
    /// </summary>
    /// <remarks>
    /// Bulk session operations are more performant than individual operations on a set of facts.
    /// </remarks>
    /// <param name="fact">Fact to insert.</param>
    /// <returns>Whether the fact was inserted or not.</returns>
    bool TryInsert(object fact);

    /// <summary>
    /// Updates existing facts in the rules engine memory.
    /// </summary>
    /// <remarks>
    /// Bulk session operations are more performant than individual operations on a set of facts.
    /// </remarks>
    /// <param name="facts">Facts to update.</param>
    /// <exception cref="ArgumentException">If any fact does not exist in working memory.</exception>
    void UpdateAll(IEnumerable<object> facts);

    /// <summary>
    /// Updates existing facts in the rules engine memory if the facts exist.
    /// If any of the facts don't exist in the engine, none of the facts are updated.
    /// </summary>
    /// <remarks>
    /// Bulk session operations are more performant than individual operations on a set of facts.
    /// </remarks>
    /// <param name="facts">Facts to update.</param>
    /// <returns>Result of facts update.</returns>
    IFactResult TryUpdateAll(IEnumerable<object> facts);

    /// <summary>
    /// Updates existing facts in the rules engine memory if the facts exist.
    /// If any of the facts don't exist in the engine, the behavior is defined by <see cref="BatchOptions"/>.
    /// </summary>
    /// <remarks>
    /// Bulk session operations are more performant than individual operations on a set of facts.
    /// </remarks>
    /// <param name="facts">Facts to update.</param>
    /// <param name="options">Options that define behavior of the batch operation.</param>
    /// <returns>Result of facts update.</returns>
    IFactResult TryUpdateAll(IEnumerable<object> facts, BatchOptions options);

    /// <summary>
    /// Updates existing fact in the rules engine memory.
    /// </summary>
    /// <remarks>
    /// Bulk session operations are more performant than individual operations on a set of facts.
    /// </remarks>
    /// <param name="fact">Fact to update.</param>
    /// <exception cref="ArgumentException">If fact does not exist in working memory.</exception>
    void Update(object fact);

    /// <summary>
    /// Updates a fact in the rules engine memory if the fact exists.
    /// </summary>
    /// <remarks>
    /// Bulk session operations are more performant than individual operations on a set of facts.
    /// </remarks>
    /// <param name="fact">Fact to update.</param>
    /// <returns>Whether the fact was updated or not.</returns>
    bool TryUpdate(object fact);

    /// <summary>
    /// Removes existing facts from the rules engine memory.
    /// </summary>
    /// <remarks>
    /// Bulk session operations are more performant than individual operations on a set of facts.
    /// </remarks>
    /// <param name="facts">Facts to remove.</param>
    /// <exception cref="ArgumentException">If any fact does not exist in working memory.</exception>
    void RetractAll(IEnumerable<object> facts);

    /// <summary>
    /// Removes existing facts from the rules engine memory if the facts exist.
    /// If any of the facts don't exist in the engine, none of the facts are removed.
    /// </summary>
    /// <remarks>
    /// Bulk session operations are more performant than individual operations on a set of facts.
    /// </remarks>
    /// <param name="facts">Facts to remove.</param>
    /// <returns>Result of facts removal.</returns>
    IFactResult TryRetractAll(IEnumerable<object> facts);

    /// <summary>
    /// Removes existing facts from the rules engine memory if the facts exist.
    /// If any of the facts don't exist in the engine, the behavior is defined by <see cref="BatchOptions"/>.
    /// </summary>
    /// <remarks>
    /// Bulk session operations are more performant than individual operations on a set of facts.
    /// </remarks>
    /// <param name="facts">Facts to remove.</param>
    /// <param name="options">Options that define behavior of the batch operation.</param>
    /// <returns>Result of facts removal.</returns>
    IFactResult TryRetractAll(IEnumerable<object> facts, BatchOptions options);

    /// <summary>
    /// Removes existing fact from the rules engine memory.
    /// </summary>
    /// <remarks>
    /// Bulk session operations are more performant than individual operations on a set of facts.
    /// </remarks>
    /// <param name="fact">Fact to remove.</param>
    /// <exception cref="ArgumentException">If fact does not exist in working memory.</exception>
    void Retract(object fact);

    /// <summary>
    /// Removes a fact from the rules engine memory if the fact exists.
    /// </summary>
    /// <remarks>
    /// Bulk session operations are more performant than individual operations on a set of facts.
    /// </remarks>
    /// <param name="facts">Fact to remove.</param>
    /// <returns>Whether the fact was retracted or not.</returns>
    bool TryRetract(object facts);

    /// <summary>
    /// Propagates all queued linked facts.
    /// </summary>
    /// <returns>Collection of propagated sets of linked facts.</returns>
    IEnumerable<ILinkedFactSet> PropagateLinked();

    /// <summary>
    /// Starts rules execution cycle.
    /// This method blocks until there are no more rules to fire.
    /// </summary>
    /// <returns>Number of rules that fired.</returns>
    int Fire();

    /// <summary>
    /// Starts rules execution cycle.
    /// This method blocks until there are no more rules to fire or cancellation is requested.
    /// </summary>
    /// <param name="cancellationToken">Enables cooperative cancellation of the rules execution cycle.</param>
    /// <returns>Number of rules that fired.</returns>
    int Fire(CancellationToken cancellationToken);

    /// <summary>
    /// Starts rules execution cycle.
    /// This method blocks until maximum number of rules fired or there are no more rules to fire.
    /// </summary>
    /// <param name="maxRulesNumber">Maximum number of rules to fire.</param>
    /// <returns>Number of rules that fired.</returns>
    int Fire(int maxRulesNumber);

    /// <summary>
    /// Starts rules execution cycle.
    /// This method blocks until maximum number of rules fired, cancellation is requested or there are no more rules to fire.
    /// </summary>
    /// <param name="maxRulesNumber">Maximum number of rules to fire.</param>
    /// <param name="cancellationToken">Enables cooperative cancellation of the rules execution cycle.</param>
    /// <returns>Number of rules that fired.</returns>
    int Fire(int maxRulesNumber, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a LINQ query to retrieve facts of a given type from the rules engine's memory.
    /// </summary>
    /// <typeparam name="TFact">Type of facts to query. Use <see cref="object"/> to query all facts.</typeparam>
    /// <returns>Queryable working memory of the rules engine.</returns>
    IQueryable<TFact> Query<TFact>();
}

internal interface ISessionInternal : ISession
{
    new IAgendaInternal Agenda { get; }

    IEnumerable<object> GetLinkedKeys(Activation activation);
    object GetLinked(Activation activation, object key);
    void QueueInsertLinked(Activation activation, IEnumerable<KeyValuePair<object, object>> keyedFacts);
    void QueueUpdateLinked(Activation activation, IEnumerable<KeyValuePair<object, object>> keyedFacts);
    void QueueRetractLinked(Activation activation, IEnumerable<KeyValuePair<object, object>> keyedFacts);
    void QueueRetractLinked(Activation activation);
}

internal sealed class Session : ISessionInternal
{
    private static readonly ILinkedFactSet[] EmptyLinkedFactResult = Array.Empty<ILinkedFactSet>();

    private readonly IAgendaInternal _agenda;
    private readonly INetwork _network;
    private readonly IWorkingMemory _workingMemory;
    private readonly IEventAggregator _eventAggregator;
    private readonly IMetricsAggregator _metricsAggregator;
    private readonly IActionExecutor _actionExecutor;
    private readonly IExecutionContext _executionContext;
    private readonly Queue<LinkedFactSet> _linkedFacts = new();

    internal Session(
        INetwork network,
        IAgendaInternal agenda,
        IWorkingMemory workingMemory,
        IEventAggregator eventAggregator,
        IMetricsAggregator metricsAggregator,
        IActionExecutor actionExecutor,
        IIdGenerator idGenerator,
        IDependencyResolver dependencyResolver,
        IActionInterceptor actionInterceptor)
    {
        _network = network;
        _workingMemory = workingMemory;
        _agenda = agenda;
        _eventAggregator = eventAggregator;
        _metricsAggregator = metricsAggregator;
        _actionExecutor = actionExecutor;
        _executionContext = new ExecutionContext(this, _workingMemory, _agenda, _eventAggregator, _metricsAggregator, idGenerator);
        DependencyResolver = dependencyResolver;
        ActionInterceptor = actionInterceptor;
        AutoPropagateLinkedFacts = true;
    }

    public bool AutoPropagateLinkedFacts { get; set; }
    public IAgenda Agenda => _agenda;
    public IEventProvider Events => _eventAggregator;
    public IMetricsProvider Metrics => _metricsAggregator;
    public IDependencyResolver DependencyResolver { get; set; }
    public IActionInterceptor ActionInterceptor { get; set; }

    IAgendaInternal ISessionInternal.Agenda => _agenda;

    internal void Activate()
    {
        _agenda.Initialize(_executionContext);
        _network.Activate(_executionContext);
    }

    public void InsertAll(IEnumerable<object> facts)
    {
        var result = TryInsertAll(facts);
        if (result.FailedCount > 0)
            throw new ArgumentException("Facts for insert already exist", nameof(facts));
    }

    public IFactResult TryInsertAll(IEnumerable<object> facts)
    {
        return TryInsertAll(facts, BatchOptions.AllOrNothing);
    }

    public IFactResult TryInsertAll(IEnumerable<object> facts, BatchOptions options)
    {
        if (facts == null)
            throw new ArgumentNullException(nameof(facts));

        var failed = new List<object>();
        var toPropagate = new List<Fact>();
        foreach (var factObject in facts)
        {
            var factWrapper = _workingMemory.GetFact(factObject);
            if (factWrapper == null)
            {
                factWrapper = new Fact(factObject);
                toPropagate.Add(factWrapper);
            }
            else
            {
                failed.Add(factObject);
            }
        }

        var result = new FactResult(failed);
        if (result.FailedCount == 0 || options == BatchOptions.SkipFailed)
        {
            foreach (var fact in toPropagate)
            {
                _workingMemory.AddFact(fact);
            }

            _network.PropagateAssert(_executionContext, toPropagate);

            PropagateLinked();
        }
        return result;
    }

    public void Insert(object fact)
    {
        InsertAll(new[] { fact });
    }

    public bool TryInsert(object fact)
    {
        var result = TryInsertAll(new[] {fact});
        return result.FailedCount == 0;
    }

    public void UpdateAll(IEnumerable<object> facts)
    {
        var result = TryUpdateAll(facts);
        if (result.FailedCount > 0)
            throw new ArgumentException("Facts for update do not exist", nameof(facts));
    }

    public IFactResult TryUpdateAll(IEnumerable<object> facts)
    {
        return TryUpdateAll(facts, BatchOptions.AllOrNothing);
    }

    public IFactResult TryUpdateAll(IEnumerable<object> facts, BatchOptions options)
    {
        if (facts == null)
            throw new ArgumentNullException(nameof(facts));

        var failed = new List<object>();
        var toPropagate = new List<Fact>();
        foreach (var fact in facts)
        {
            var factWrapper = _workingMemory.GetFact(fact);
            if (factWrapper != null && factWrapper.Source == null)
            {
                UpdateFact(factWrapper, fact);
                toPropagate.Add(factWrapper);
            }
            else
            {
                failed.Add(fact);
            }
        }

        var result = new FactResult(failed);
        if (result.FailedCount == 0 || options == BatchOptions.SkipFailed)
        {
            foreach (var fact in toPropagate)
            {
                _workingMemory.UpdateFact(fact);
            }

            _network.PropagateUpdate(_executionContext, toPropagate);

            PropagateLinked();
        }
        return result;
    }

    public void Update(object fact)
    {
        UpdateAll(new[] {fact});
    }

    public bool TryUpdate(object fact)
    {
        var result = TryUpdateAll(new[] {fact});
        return result.FailedCount == 0;
    }

    public void RetractAll(IEnumerable<object> facts)
    {
        var result = TryRetractAll(facts);
        if (result.FailedCount > 0)
            throw new ArgumentException("Facts for retract do not exist", nameof(facts));
    }

    public IFactResult TryRetractAll(IEnumerable<object> facts)
    {
        return TryRetractAll(facts, BatchOptions.AllOrNothing);
    }

    public IFactResult TryRetractAll(IEnumerable<object> facts, BatchOptions options)
    {
        if (facts == null)
            throw new ArgumentNullException(nameof(facts));

        var failed = new List<object>();
        var toPropagate = new List<Fact>();
        foreach (var fact in facts)
        {
            var factWrapper = _workingMemory.GetFact(fact);
            if (factWrapper != null && factWrapper.Source == null)
            {
                toPropagate.Add(factWrapper);
            }
            else
            {
                failed.Add(fact);
            }
        }

        var result = new FactResult(failed);
        if (result.FailedCount == 0 || options == BatchOptions.SkipFailed)
        {
            _network.PropagateRetract(_executionContext, toPropagate);

            foreach (var fact in toPropagate)
            {
                _workingMemory.RemoveFact(fact);
            }

            PropagateLinked();
        }
        return result;
    }

    public void Retract(object fact)
    {
        RetractAll(new[] {fact});
    }

    public bool TryRetract(object fact)
    {
        var result = TryRetractAll(new[] {fact});
        return result.FailedCount == 0;
    }

    public IEnumerable<ILinkedFactSet> PropagateLinked()
    {
        if (_linkedFacts.Count == 0)
            return EmptyLinkedFactResult;

        var factSets = new List<ILinkedFactSet>(_linkedFacts.Count);
        while (_linkedFacts.Count > 0)
        {
            var item = _linkedFacts.Dequeue();
            factSets.Add(item);
            switch (item.Action)
            {
                case LinkedFactAction.Insert:
                    _network.PropagateAssert(_executionContext, item.Facts);
                    break;
                case LinkedFactAction.Update:
                    _network.PropagateUpdate(_executionContext, item.Facts);
                    break;
                case LinkedFactAction.Retract:
                    _network.PropagateRetract(_executionContext, item.Facts);
                    foreach (var fact in item.Facts)
                    {
                        _workingMemory.RemoveFact(fact);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unrecognized linked fact action. Action={item.Action}");
            }
        }

        return factSets;
    }

    public IEnumerable<object> GetLinkedKeys(Activation activation)
    {
        var keys = _workingMemory.GetLinkedKeys(activation);
        return keys;
    }

    public object GetLinked(Activation activation, object key)
    {
        var factWrapper = _workingMemory.GetLinkedFact(activation, key);
        return factWrapper?.Object;
    }

    public void QueueInsertLinked(Activation activation, IEnumerable<KeyValuePair<object, object>> keyedFacts)
    {
        var toAdd = new List<Tuple<object, Fact>>();
        var toPropagate = new List<Fact>();
        foreach (var keyedFact in keyedFacts)
        {
            var key = keyedFact.Key;
            var factWrapper = _workingMemory.GetLinkedFact(activation, key);
            if (factWrapper != null)
            {
                throw new ArgumentException($"Linked fact already exists. Key={key}");
            }
            factWrapper = new SyntheticFact(keyedFact.Value);
            factWrapper.Source = new LinkedFactSource(activation);
            toAdd.Add(System.Tuple.Create(key, factWrapper));
            toPropagate.Add(factWrapper);
        }
        foreach (var item in toAdd)
        {
            _workingMemory.AddLinkedFact(activation, item.Item1, item.Item2);
        }

        LinkedFactSet current;
        if (_linkedFacts.Count == 0 || (current = _linkedFacts.Peek()).Action != LinkedFactAction.Insert)
        {
            current = new LinkedFactSet(LinkedFactAction.Insert);
            _linkedFacts.Enqueue(current);
        }
        current.Facts.AddRange(toPropagate);
    }

    public void QueueUpdateLinked(Activation activation, IEnumerable<KeyValuePair<object, object>> keyedFacts)
    {
        var toUpdate = new List<Tuple<object, Fact, object>>();
        var toPropagate = new List<Fact>();
        foreach (var keyedFact in keyedFacts)
        {
            var key = keyedFact.Key;
            var factWrapper = _workingMemory.GetLinkedFact(activation, key);
            if (factWrapper == null)
            {
                throw new ArgumentException($"Linked fact does not exist. Key={key}");
            }
            factWrapper.Source = new LinkedFactSource(activation);
            toUpdate.Add(System.Tuple.Create(key, factWrapper, keyedFact.Value));
            toPropagate.Add(factWrapper);
        }
        foreach (var item in toUpdate)
        {
            _workingMemory.UpdateLinkedFact(activation, item.Item1, item.Item2, item.Item3);
        }

        LinkedFactSet current;
        if (_linkedFacts.Count == 0 || (current = _linkedFacts.Peek()).Action != LinkedFactAction.Update)
        {
            current = new LinkedFactSet(LinkedFactAction.Update);
            _linkedFacts.Enqueue(current);
        }
        current.Facts.AddRange(toPropagate);
    }

    public void QueueRetractLinked(Activation activation, IEnumerable<KeyValuePair<object, object>> keyedFacts)
    {
        var toRemove = new List<Tuple<object, Fact>>();
        var toPropagate = new List<Fact>();
        foreach (var keyedFact in keyedFacts)
        {
            var key = keyedFact.Key;
            var factWrapper = _workingMemory.GetFact(keyedFact.Value);
            if (factWrapper == null)
            {
                throw new ArgumentException($"Linked fact does not exist. Key={key}");
            }
            factWrapper.Source = new LinkedFactSource(activation);
            toRemove.Add(System.Tuple.Create(key, factWrapper));
            toPropagate.Add(factWrapper);
        }
        foreach (var item in toRemove)
        {
            _workingMemory.RemoveLinkedFact(activation, item.Item1, item.Item2);
            item.Item2.Source = null;
        }

        LinkedFactSet current;
        if (_linkedFacts.Count == 0 || (current = _linkedFacts.Peek()).Action != LinkedFactAction.Retract)
        {
            current = new LinkedFactSet(LinkedFactAction.Retract);
            _linkedFacts.Enqueue(current);
        }
        current.Facts.AddRange(toPropagate);
    }

    public void QueueRetractLinked(Activation activation)
    {
        var linkedKeys = GetLinkedKeys(activation);
        var keyedFacts = new List<KeyValuePair<object, object>>();
        foreach (var key in linkedKeys)
        {
            var linkedFact = GetLinked(activation, key);
            keyedFacts.Add(new KeyValuePair<object, object>(key, linkedFact));
        }
        QueueRetractLinked(activation, keyedFacts);
    }

    public int Fire()
    {
        return Fire(default(CancellationToken));
    }

    public int Fire(CancellationToken cancellationToken)
    {
        return Fire(Int32.MaxValue, cancellationToken);
    }

    public int Fire(int maxRulesNumber)
    {
        return Fire(maxRulesNumber, default);
    }

    public int Fire(int maxRulesNumber, CancellationToken cancellationToken)
    {
        int ruleFiredCount = 0;
        while (!_agenda.IsEmpty && ruleFiredCount < maxRulesNumber)
        {
            Activation activation = _agenda.Pop();
            IActionContext actionContext = new ActionContext(this, activation, cancellationToken);

            try
            {
                _actionExecutor.Execute(_executionContext, actionContext);
            }
            finally
            {
                ruleFiredCount++;
                if (AutoPropagateLinkedFacts) PropagateLinked();
            }

            if (actionContext.IsHalted || cancellationToken.IsCancellationRequested) break;
        }
        return ruleFiredCount;
    }

    public IQueryable<TFact> Query<TFact>()
    {
        return _workingMemory.Facts.Select(x => x.Object).OfType<TFact>().AsQueryable();
    }

    private static void UpdateFact(Fact fact, object factObject)
    {
        if (ReferenceEquals(fact.RawObject, factObject)) return;
        fact.RawObject = factObject;
    }

    ReteGraph ISessionSchemaProvider.GetSchema() => _network.GetSchema();
}
