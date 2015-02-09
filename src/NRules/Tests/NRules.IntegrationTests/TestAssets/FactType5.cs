namespace NRules.IntegrationTests.TestAssets
{
    public class FactType5
    {
        public string TestProperty { get; set; }
        public int TestCount { get; set; }

        public void IncrementCount()
        {
            TestCount++;
        }
    }
}