using NRules.Dsl;

namespace NRules.Rule
{
    public interface IRuleAction
    {
        void Invoke(IActionContext context);
    }
}