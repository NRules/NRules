using System.Collections.Generic;
using System.Linq;
using System.Text;
using NRules.RuleModel;

namespace NRules.Testing;

internal interface IRuleExpectation
{
    RuleAssertResult Verify(IReadOnlyList<IMatch> invocations);
    int ExpectedCount { get; }
}

internal class MultiRuleExpectation : IRuleExpectation
{
    private readonly IRuleExpectation[] _expectations;

    public MultiRuleExpectation(IRuleExpectation[] expectations)
    {
        _expectations = expectations;
        ExpectedCount = expectations.Sum(x => x.ExpectedCount);
    }

    public int ExpectedCount { get; }

    public RuleAssertResult Verify(IReadOnlyList<IMatch> invocations)
    {
        int actualCount = 0;
        foreach (var expectation in _expectations)
        {
            var slice = new ReadOnlyListSlice<IMatch>(invocations, actualCount);
            var result = expectation.Verify(slice);
            if (result.Status != RuleAssertStatus.Passed)
            {
                return result;
            }
            actualCount += expectation.ExpectedCount;
        }

        var status = RuleAssertStatus.Passed;
        if (ExpectedCount > 0 && ExpectedCount < invocations.Count)
        {
            status = RuleAssertStatus.Failed;
            actualCount = invocations.Count;
        }

        return new RuleAssertResult(null, status, "Fired", ExpectedCount, actualCount);
    }
}

internal class SingleRuleExpectation : IRuleExpectation
{
    private readonly IRuleDefinition _ruleDefinition;
    private readonly FactConstraint[] _constraints;
    private readonly Times _expectedCount;
    private readonly bool _isExact;
    private readonly IFactMatch[] _matchedFacts;

    public SingleRuleExpectation(IRuleDefinition ruleDefinition, FactConstraint[] constraints,
        Times expectedCount, bool isExact)
    {
        _ruleDefinition = ruleDefinition;
        _constraints = constraints;
        _expectedCount = expectedCount;
        _isExact = isExact;
        _matchedFacts = new IFactMatch[_constraints.Length];
    }

    public int ExpectedCount => _expectedCount.Value;

    public RuleAssertResult Verify(IReadOnlyList<IMatch> invocations)
    {
        var resultMatches = new List<IMatch>();

        foreach (var match in invocations)
        {
            if (IsRuleEqual(match.Rule) && AreConstraintsSatisfied(match))
            {
                resultMatches.Add(match);
                OnMetExpectation();
            }
            else
            {
                break;
            }

            if (_isExact && resultMatches.Count == _expectedCount.Value)
            {
                break;
            }
        }

        var status = resultMatches.Count == _expectedCount.Value ? RuleAssertStatus.Passed : RuleAssertStatus.Failed;
        var assertionText = GetAssertionText();
        var result = new RuleAssertResult(_ruleDefinition.Name, status, assertionText, _expectedCount.Value, resultMatches.Count);
        return result;
    }

    private bool IsRuleEqual(IRuleDefinition ruleDefinition)
    {
        return Equals(ruleDefinition.Name, _ruleDefinition.Name);
    }

    private bool AreConstraintsSatisfied(IMatch match)
    {
        int j = 0;
        var facts = match.Facts.ToArray();
        for (int i = 0; i < _constraints.Length; i++)
        {
            bool isSatisfied = false;
            for (; j < facts.Length && !isSatisfied; j++)
            {
                isSatisfied = _constraints[i].IsSatisfied(facts[j]);
                _matchedFacts[i] = facts[j];
            }

            if (!isSatisfied) return false;
        }
        return true;
    }

    private void OnMetExpectation()
    {
        for (int i = 0; i < _constraints.Length; i++)
        {
            _constraints[i].OnSatisfied(_matchedFacts[i]);
        }
    }

    private string GetAssertionText()
    {
        var s = new StringBuilder("Fired");
        if (_constraints.Any())
        {
            s.Append(" With: ");
            s.Append(string.Join(", ", _constraints.Select(c => c.GetText())));
        }

        return s.ToString();
    }
}