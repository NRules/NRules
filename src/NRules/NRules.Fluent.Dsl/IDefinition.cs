namespace NRules.Fluent.Dsl
{
    public interface IDefinition
    {
        ILeftHandSide When();
        IRightHandSide Then();
    }
}