namespace NRules.Dsl
{
    public interface IRuleDefinition
    {
        ILeftHandSide When();
        IRightHandSide Then();
    }
}