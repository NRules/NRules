using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace NRules.Testing;

public class RulesTestFixture : IRulesTestFixture
{
    private readonly Lazy<ISession> _lazySession;
    private readonly IRuleActivator _ruleActivator;
    private readonly RuleCompiler _compiler;
    private readonly RepositorySetup _setup;

    public RulesTestFixture(IRuleActivator ruleActivator, RuleCompiler compiler, IRuleAsserter asserter)
    {
        _ruleActivator = ruleActivator;
        _compiler = compiler;
        var repository = new RuleRepository
        {
            Activator = ruleActivator
        };
        _setup = new RepositorySetup(repository);
        Verify = new RulesVerification(asserter, _setup);

        _lazySession = new(CreateSession);

        ISession CreateSession()
        {
            var ruleDefinitions = repository.GetRules();
            var factory = _compiler.Compile(ruleDefinitions);
            var session = factory.CreateSession();
            session.Events.RuleFiredEvent += (_, args) => _setup.OnRuleFired(args.Match);
            return session;
        }
    }

    public ISession Session => _lazySession.Value;

    public IRepositorySetup Setup => _setup;

    public IRulesVerification Verify { get; }

    public T GetRuleInstance<T>()
        where T : Rule =>
        _ruleActivator.Activate<T>().Single();

    public IEnumerable<T> GetFiredFacts<T>()
    {
        var factMatchess = GetFiredFactMatches<T>((ruleFirings, ruleMetadata) =>
            ruleFirings.LastOrDefault() ?? throw new InvalidOperationException($"Unable to retrieve last firing of the rule {ruleMetadata.Name}"));
        return factMatchess.Select(f => f.Value).Cast<T>();
    }

    public T GetFiredFact<T>() =>
        GetFiredFact<T>((ruleFirings, ruleMetadata) =>
            ruleFirings.LastOrDefault() ?? throw new InvalidOperationException($"Unable to retrieve last firing of the rule {ruleMetadata.Name}"));

    public T GetFiredFact<T>(int index) =>
        GetFiredFact<T>((ruleFirings, ruleMetadata) =>
        {
            if (ruleFirings.Count < index + 1)
                throw new InvalidOperationException($"Unable to retrieve firing of the rule {ruleMetadata.Name} at index {index}");

            return ruleFirings.ElementAt(index);
        });

    private T GetFiredFact<T>(Func<IReadOnlyCollection<IMatch>, IRuleMetadata, IMatch> selector)
    {
        var factMatchess = GetFiredFactMatches<T>(selector);
        var factMatch = factMatchess.FirstOrDefault()
            ?? throw new InvalidOperationException($"Did not find any facts of type {typeof(T)} in the rule match");
        return (T)factMatch.Value;
    }

    private IEnumerable<IFactMatch> GetFiredFactMatches<T>(Func<IReadOnlyCollection<IMatch>, IRuleMetadata, IMatch> selector)
    {
        var ruleMetadata = _setup.GetRule();
        var ruleFirings = _setup.GetFiredRuleMatches(ruleMetadata.Name);

        var match = selector(ruleFirings, ruleMetadata);
        return match.Facts
            .Where(f => typeof(T).IsAssignableFrom(f.Declaration.Type));
    }
}
