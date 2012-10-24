using System.Collections.Generic;

namespace NRules.Rule
{
    public interface ICompiledRule
    {
        string Handle { get; }
        string Name { get; }
        int Priority { get; }
        ISet<IDeclaration> Declarations { get; }
        IList<IPredicate> Predicates { get; }
        IList<ICondition> Conditions { get; }
        IList<IRuleAction> Actions { get; }
    }
}