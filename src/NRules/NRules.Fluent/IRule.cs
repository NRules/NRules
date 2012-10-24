using NRules.Fluent.Dsl;

namespace NRules.Fluent
{
    public interface IRule
    {
        void Define(IRuleDefinition definition);
    }
}