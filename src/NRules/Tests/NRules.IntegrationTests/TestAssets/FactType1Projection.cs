namespace NRules.IntegrationTests.TestAssets
{
    public class FactType1Projection
    {
        public FactType1Projection(FactType1 fact)
        {
            Value = fact.TestProperty;
        }

        public string Value { get; private set; }
    }
}