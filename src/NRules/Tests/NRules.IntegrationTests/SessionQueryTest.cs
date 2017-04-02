using System.Linq;
using NRules.IntegrationTests.TestAssets;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class SessionQueryTest : BaseRuleTestFixture
    {
        [Test]
        public void Query_NoFacts_Empty()
        {
            //Arrange
            //Act
            var query = Session.Query<object>();
            var facts = query.ToList();

            //Assert
            Assert.AreEqual(0, facts.Count);
        }

        [Test]
        public void Query_OneFact_RetrievesFactFromQuery()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1", JoinProperty = "Valid Value 1"};
            Session.Insert(fact1);

            //Act
            var query = Session.Query<object>();
            var facts = query.ToList();

            //Assert
            Assert.AreEqual(1, facts.Count);
            Assert.AreSame(fact1, facts[0]);
        }

        [Test]
        public void Query_RuleInsertsSecondFact_TwoFactsInMemory()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1", JoinProperty = "Valid Value 1"};
            Session.Insert(fact1);
            Session.Fire();

            //Act
            var query = Session.Query<object>();
            var facts = query.ToList();

            //Assert
            Assert.AreEqual(2, facts.Count);
        }

        [Test]
        public void Query_QueryFactsByType_OnlyReturnsFactsOfThatType()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1", JoinProperty = "Valid Value 1"};
            Session.Insert(fact1);
            Session.Fire();

            //Act
            var query = Session.Query<FactType2>();
            var facts = query.ToList();

            //Assert
            Assert.AreEqual(1, facts.Count);
            Assert.AreEqual(fact1.TestProperty, facts[0].JoinProperty);
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType1
        {
            public string TestProperty { get; set; }
            public string JoinProperty { get; set; }
        }

        public class FactType2
        {
            public string TestProperty { get; set; }
            public string JoinProperty { get; set; }
        }

        public class TestRule : BaseRule
        {
            public override void Define()
            {
                FactType1 fact1 = null;

                When()
                    .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"));
                Then()
                    .Do(ctx => Action(ctx))
                    .Do(ctx => ctx.Insert(new FactType2()
                    {
                        TestProperty = fact1.JoinProperty,
                        JoinProperty = fact1.TestProperty
                    }));
            }
        }
    }
}