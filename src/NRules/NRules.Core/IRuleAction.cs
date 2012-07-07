namespace NRules.Core
{
    internal interface IRuleAction
    {
        void Invoke(IActionContext context);
    }
}