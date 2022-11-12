namespace NRules.Json.Tests.TestAssets
{
    public delegate string TransformDelegate(string value1, string value2);

    public static class Calculations
    {
        public static void DoSomething()
        {
        }
        
        public static void DoSomething(FactType1 fact1)
        {
        }
        
        public static void DoSomething(FactType1 fact1, FactType2 fact2)
        {
        }

        public static void DoSomething(FactType1 fact1, ITestService service)
        {
        }

        public static void DoSomething(IEnumerable<FactType1> factGroup)
        {
        }

        public static void DoSomething(int value)
        {
        }

        public static string Transform(this string value)
        {
            return value;
        }

        public static string Transform(string s, Func<string, string> map)
        {
            return map(s);
        }

        public static bool CallOnInterface(IFactType1 fact1)
        {
            return fact1.BooleanProperty;
        }

        public static TransformDelegate Concat = string.Concat;
    }
}