namespace NRules.RuleModel.Builders
{
    internal interface IBuilder<out TElement> where TElement : RuleElement
    {
        TElement Build();
    }
}