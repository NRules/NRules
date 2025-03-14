﻿using System;
using System.Collections.Generic;
using NRules.AgendaFilters;
using NRules.Diagnostics;
using NRules.Extensibility;
using NRules.Rete;

namespace NRules;

/// <summary>
/// Represents compiled production rules that can be used to create rules sessions.
/// Created by <see cref="RuleCompiler"/> by compiling rule model into an executable form.
/// </summary>
/// <remarks>
/// Session factory is expensive to create (because rules need to be compiled into an executable form).
/// Therefore there needs to be only a single instance of session factory for a given set of rules for the lifetime of the application.
/// If repeatedly running rules for different sets of facts, don't create a new session factory for each rules run.
/// Instead, have a single session factory and create a new rules session for each independent universe of facts.
/// </remarks>
/// <seealso cref="ISession"/>
/// <seealso cref="RuleCompiler"/>
/// <threadsafety instance="true" />
public interface ISessionFactory : ISessionSchemaProvider
{
    /// <summary>
    /// Provider of events aggregated across all rule sessions. 
    /// Event sender is used to convey the session instance responsible for the event.
    /// Use it to subscribe to various rules engine lifecycle events.
    /// </summary>
    IEventProvider Events { get; }

    /// <summary>
    /// Rules dependency resolver for all rules sessions.
    /// </summary>
    IDependencyResolver DependencyResolver { get; set; }

    /// <summary>
    /// Action interceptor for all rules sessions.
    /// If provided, invocation of rule actions is delegated to the interceptor.
    /// </summary>
    IActionInterceptor? ActionInterceptor { get; set; }

    /// <summary>
    /// Creates a new rules session.
    /// </summary>
    /// <returns>New rules session.</returns>
    ISession CreateSession();

    /// <summary>
    /// Creates a new rules session.
    /// </summary>
    /// <param name="initializationAction">Action invoked on the newly created session, before the session is activated (which could result in rule matches placed on the agenda).</param>
    /// <returns>New rules session.</returns>
    ISession CreateSession(Action<ISession> initializationAction);
}

internal sealed class SessionFactory : ISessionFactory
{
    private readonly INetwork _network;
    private readonly IFactIdentityComparer _factIdentityComparer;
    private readonly List<ICompiledRule> _compiledRules;
    private readonly IEventAggregator _eventAggregator = new EventAggregator();

    public SessionFactory(INetwork network, IEnumerable<ICompiledRule> compiledRules,
        IFactIdentityComparer factIdentityComparer)
    {
        _network = network;
        _factIdentityComparer = factIdentityComparer;
        _compiledRules = new List<ICompiledRule>(compiledRules);
        DependencyResolver = new DependencyResolver();
    }

    public IEventProvider Events => _eventAggregator;
    public IDependencyResolver DependencyResolver { get; set; }
    public IActionInterceptor? ActionInterceptor { get; set; }

    public ISession CreateSession()
    {
        return CreateSession(null);
    }

    public ISession CreateSession(Action<ISession>? initializationAction)
    {
        var agenda = CreateAgenda();
        var workingMemory = new WorkingMemory(_factIdentityComparer);
        var eventAggregator = new EventAggregator(_eventAggregator);
        var metricsAggregator = new MetricsAggregator();
        var actionExecutor = new ActionExecutor();
        var idGenerator = new IdGenerator();
        var session = new Session(_network, agenda, workingMemory, eventAggregator, metricsAggregator, actionExecutor, idGenerator, DependencyResolver, ActionInterceptor);
        initializationAction?.Invoke(session);
        session.Activate();
        return session;
    }

    private IAgendaInternal CreateAgenda()
    {
        var agenda = new Agenda();
        foreach (var compiledRule in _compiledRules)
        {
            var ruleFilters = CreateRuleFilters(compiledRule);
            foreach (var filter in ruleFilters)
            {
                agenda.AddFilter(compiledRule.Definition, filter);
            }
        }
        return agenda;
    }

    private static IEnumerable<IAgendaFilter> CreateRuleFilters(ICompiledRule compiledRule)
    {
        var filterConditions = compiledRule.Filter.Conditions;
        if (filterConditions.Count > 0)
        {
            var filter = new PredicateAgendaFilter(filterConditions);
            yield return filter;
        }
        var filterKeySelectors = compiledRule.Filter.KeySelectors;
        if (filterKeySelectors.Count > 0)
        {
            var filter = new KeyChangeAgendaFilter(filterKeySelectors);
            yield return filter;
        }
    }

    ReteGraph ISessionSchemaProvider.GetSchema() => _network.GetSchema();
}