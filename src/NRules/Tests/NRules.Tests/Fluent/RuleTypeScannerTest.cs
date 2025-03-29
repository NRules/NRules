using NRules.Fluent;
using NRules.Fluent.Dsl;
using Xunit;

namespace NRules.Tests.Fluent
{
    public class RuleTypeScannerTest
    {
        [Fact]
        public void Where_FilterOnNamespacePrefix_FiltersCorrectly()
        {
            //Arrange
            var scanner01 = new RuleTypeScanner();
            scanner01.AssemblyOf<RuleTypeScannerTest>();
            var allRules = scanner01.GetRuleTypes();
           
            //Act
            var scanner02 = new RuleTypeScanner();
            scanner02.AssemblyOf<RuleTypeScannerTest>()
                .Where(ruleType => ruleType.Namespace == "NRules.Tests.Fluent.SubNsA");
            var subNsARules = scanner02.GetRuleTypes();
           
            //Assert
            Assert.Equal(3, allRules.Length);
            Assert.Single(subNsARules);
        }
    }

    public class TestRule001 : Rule
    {
        public override void Define()
        {
        }
    }

    namespace SubNsA
    {
        public class TestRule001 : Rule
        {
            public override void Define()
            {
            }
        }
    }

    namespace SubNsB
    {
        public class TestRule002 : Rule
        {
            public override void Define()
            {
            }
        }
    }
}
