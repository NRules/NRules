using System.Collections.Generic;

namespace NRules.Json.Tests.TestAssets
{
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

        public static string Transform(this string value, int factor)
        {
            return value;
        }
    }
}