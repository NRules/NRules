namespace NRules.Fluent.Dsl
{
    public interface IRuleDefinition
    {
        ILeftHandSide When();
        IRightHandSide Then();
    }
}