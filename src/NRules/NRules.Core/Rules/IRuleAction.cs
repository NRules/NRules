namespace NRules.Core.Rules
{
    internal interface IRuleAction
    {
        void Invoke(IActionContext context);
    }
}