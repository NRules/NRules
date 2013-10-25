namespace NRules.Dsl
{
    public interface IDefinition
    {
        ILeftHandSide When();
        IRightHandSide Then();
    }
}