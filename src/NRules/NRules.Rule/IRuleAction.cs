using NRules.Fluent;

namespace NRules.Rule
{
    public interface IRuleAction
    {
        void Invoke(IActionContext context);
    }
}