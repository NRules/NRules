using System;
using System.Linq;
using System.Reflection;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class RulesLoadTest
    {
        [Test]
        public void Load_AssemblyWithoutRules_Empty()
        {
            //Arrange
            RuleRepository target = CreateTarget();

            //Act
            target.Load(x => x.From(typeof (string).Assembly));
            IRuleSet ruleSet = target.GetRuleSets().First();

            //Assert
            Assert.AreEqual(0, ruleSet.Rules.Count());
        }

        [Test]
        public void Load_InvalidTypes_Empty()
        {
            //Arrange
            RuleRepository target = CreateTarget();

            //Act
            target.Load(x => x.From(typeof (string)));
            IRuleSet ruleSet = target.GetRuleSets().First();

            //Assert
            Assert.AreEqual(0, ruleSet.Rules.Count());
        }

        [Test]
        public void Load_EmptyRule_Throws()
        {
            //Arrange
            RuleRepository target = CreateTarget();

            //Act - Assert
            var ex = Assert.Throws<RuleDefinitionException>(() => target.Load(x => x.NestedTypes().From(typeof (EmptyRule))));
            Assert.AreEqual(typeof (EmptyRule), ex.RuleType);
        }

        [Test]
        public void Load_CannotActivateRule_Throws()
        {
            //Arrange
            RuleRepository target = CreateTarget();

            //Act - Assert
            var ex = Assert.Throws<RuleActivationException>(() => target.Load(x => x.NestedTypes().From(typeof (CannotActivateRule))));
            Assert.AreEqual(typeof (CannotActivateRule), ex.RuleType);
        }

        [Test]
        public void Load_AssemblyWithRulesToNamedRuleSet_RuleSetNameMatches()
        {
            //Arrange
            RuleRepository target = CreateTarget();

            //Act
            target.Load(x => x
                .NestedTypes()
                .From(ThisAssembly)
                .Where(r => r.RuleType == typeof (ValidRule))
                .To("Test"));
            IRuleSet ruleSet = target.GetRuleSets().First();

            //Assert
            Assert.AreEqual("Test", ruleSet.Name);
        }

        [Test]
        public void Load_AssemblyWithRulesToDefaultRuleSet_DefaultRuleSetName()
        {
            //Arrange
            RuleRepository target = CreateTarget();

            //Act
            target.Load(x => x
                .NestedTypes()
                .From(ThisAssembly)
                .Where(r => r.RuleType == typeof (ValidRule)));
            IRuleSet ruleSet = target.GetRuleSets().First();

            //Assert
            Assert.AreEqual("default", ruleSet.Name);
        }

        [Test]
        public void Load_FilterRuleByName_MatchingRule()
        {
            //Arrange
            RuleRepository target = CreateTarget();

            //Act
            target.Load(x => x
                .NestedTypes()
                .From(ThisAssembly)
                .Where(r => r.Name.Contains("Valid")));
            IRuleSet ruleSet = target.GetRuleSets().First();

            //Assert
            Assert.AreEqual(1, ruleSet.Rules.Count());
            Assert.AreEqual(typeof (ValidRule).FullName, ruleSet.Rules.First().Name);
        }

        [Test]
        public void Load_FilterRuleByTag_MatchingRule()
        {
            //Arrange
            RuleRepository target = CreateTarget();

            //Act
            target.Load(x => x
                .NestedTypes()
                .From(ThisAssembly)
                .Where(r => r.IsTagged("LoadTest")));
            IRuleSet ruleSet = target.GetRuleSets().First();

            //Assert
            Assert.AreEqual(1, ruleSet.Rules.Count());
            Assert.AreEqual("Rule with metadata", ruleSet.Rules.First().Name);
        }

        private static Assembly ThisAssembly
        {
            get { return Assembly.GetExecutingAssembly(); }
        }

        public RuleRepository CreateTarget()
        {
            return new RuleRepository();
        }

        public class FactType
        {
        }

        public class CannotActivateRule : Rule
        {
            public CannotActivateRule()
            {
                throw new InvalidOperationException("Failed in ctor");
            }

            public override void Define()
            {
            }
        }

        public class EmptyRule : Rule
        {
            public override void Define()
            {
            }
        }

        public class ValidRule : Rule
        {
            public override void Define()
            {
                When()
                    .Match<FactType>();
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }

        [Name("Rule with metadata")]
        [Tag("LoadTest")]
        public class TaggedRule : Rule
        {
            public override void Define()
            {
                When()
                    .Match<FactType>();
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}