namespace NRules.Json.Tests.TestAssets
{
    public class FactType1
    {
        public bool BooleanProperty { get; set; }
        public string StringProperty { get; set; }
        public string GroupKey { get; set; }
    }
    
    public class FactType2
    {
        public FactType1 JoinProperty { get; set; }
    }
}