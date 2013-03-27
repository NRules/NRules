using NRules.Dsl;

namespace NRules.Core.Rete
{
    public interface IRuleAction
    {
        void Invoke(IActionContext context);
    }
}