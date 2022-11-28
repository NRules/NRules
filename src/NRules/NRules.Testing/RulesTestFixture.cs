using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace NRules.Testing;

/// <summary>
/// Fixture to test rules.
/// </summary>
public class RulesTestFixture
{
    private readonly Lazy<ISession> _lazySession;
    private readonly CachedRuleActivator _ruleActivator;
    private readonly RepositorySetup _setup;

    /// <summary>
    /// Initializes a new instance of the <see cref="RulesTestFixture"/> class.
    /// </summary>
    public RulesTestFixture(IRuleAsserter asserter)
    {
        var compiler = new RuleCompiler();
        var repository = new RuleRepository();
        _ruleActivator = new CachedRuleActivator(repository.Activator);
        repository.Activator = _ruleActivator;
        _setup = new RepositorySetup(compiler, repository);
        Verify = new RulesVerification(asserter, _setup);

        _lazySession = new(CreateSession);

        ISession CreateSession()
        {
            var ruleDefinitions = repository.GetRules();
            var factory = compiler.Compile(ruleDefinitions);
            var session = factory.CreateSession();
            session.Events.RuleFiredEvent += (_, args) => _setup.OnRuleFired(args.Match);
            return session;
        }
    }

    /// <summary>
    /// Gets the current rules engine session.
    /// </summary>
    /// <remarks>Lazily created</remarks>
    public ISession Session => _lazySession.Value;

    /// <summary>
    /// Gets the setup helper to register rules under test.
    /// </summary>
    public IRepositorySetup Setup => _setup;

    /// <summary>
    /// Gets the rule verification builder to configure rule firing assertions.
    /// </summary>
    public IRulesVerification Verify { get; }

    /// <summary>
    /// Gets the instance of the rule under test by the specified rule type.
    /// </summary>
    /// <typeparam name="T">Type of the rule under test to get the instance of.</typeparam>
    /// <returns>Activated rule instance.</returns>
    public T GetRuleInstance<T>()
        where T : Rule =>
        _ruleActivator.Activate<T>().Single();

    /// <summary>
    /// Gets the matched facts of a given type from the last firing of the rule under test.
    /// </summary>
    /// <typeparam name="T">Type of the fact.</typeparam>
    /// <returns>Matching facts.</returns>
    public IEnumerable<T> GetFiredFacts<T>()
    {
        var factMatchess = GetFiredFactMatches<T>(GetLastFiring);
        return factMatchess.Select(f => f.Value).Cast<T>();
    }

    /// <summary>
    /// Gets the first matched fact of a given type from the last firing of the rule under test.
    /// </summary>
    /// <typeparam name="T">Type of the fact.</typeparam>
    /// <returns>Matching fact.</returns>
    public T GetFiredFact<T>() =>
         GetFiredFact<T>(GetLastFiring);

    /// <summary>
    /// Gets the first matched fact of a given type from the firing of the rule under test at <paramref name="index"/> position.
    /// </summary>
    /// <typeparam name="T">Type of the fact.</typeparam>
    /// <param name="index">Index of firing of the rule under test (1-based).</param>
    /// <returns>Matching fact.</returns>
    public T GetFiredFact<T>(int index) =>
        GetFiredFact<T>((ruleFirings, ruleMetadata) =>
        {
            if (ruleFirings.Count < index + 1)
                throw new InvalidOperationException($"Unable to retrieve firing of the rule {ruleMetadata.Name} at index {index}");

            return ruleFirings.ElementAt(index);
        });

    private static IMatch GetLastFiring(IReadOnlyCollection<IMatch> ruleFirings, IRuleMetadata ruleMetadata) =>
        ruleFirings.LastOrDefault() ?? throw new InvalidOperationException($"Unable to retrieve last firing of the rule {ruleMetadata.Name}");

    private T GetFiredFact<T>(Func<IReadOnlyCollection<IMatch>, IRuleMetadata, IMatch> selector)
    {
        var factMatchess = GetFiredFactMatches<T>(selector);
        var factMatch = factMatchess.FirstOrDefault()
            ?? throw new InvalidOperationException($"Did not find any facts of type {typeof(T)} in the rule match");
        return (T)factMatch.Value;
    }

    private IEnumerable<IFactMatch> GetFiredFactMatches<T>(Func<IReadOnlyCollection<IMatch>, IRuleMetadata, IMatch> selector)
    {
        var ruleType = _setup.RegisteredRuleTypes.Single();
        var ruleMetadata = _setup.GetRule(ruleType);
        var ruleFirings = _setup.GetFiredRuleMatches(ruleMetadata.Name);

        var match = selector(ruleFirings, ruleMetadata);
        return match.Facts
            .Where(f => typeof(T).IsAssignableFrom(f.Declaration.Type));
    }
}
