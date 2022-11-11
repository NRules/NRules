using System;
using System.Linq;
using System.Reflection;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests
{
    public class RulesLoadTest
    {
        [Fact]
        public void Load_AssemblyWithoutRules_Empty()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            target.Load(x => x.From(typeof(string).Assembly));
            var ruleSet = target.GetRuleSets().First();

            //Assert
            Assert.Empty(ruleSet.Rules);
        }

        [Fact]
        public void Load_InvalidTypes_Empty()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            target.Load(x => x.From(typeof(string)));
            var ruleSet = target.GetRuleSets().First();

            //Assert
            Assert.Empty(ruleSet.Rules);
        }

        [Fact]
        public void Load_EmptyRule_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            var ex = Assert.Throws<RuleDefinitionException>(() => target.Load(x => x.NestedTypes().From(typeof(EmptyRule))));
            Assert.Equal(typeof(EmptyRule), ex.RuleType);
        }

        [Fact]
        public void Load_NoActionRule_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            var ex = Assert.Throws<RuleDefinitionException>(() => target.Load(x => x.NestedTypes().From(typeof(NoActionRule))));
            Assert.Equal(typeof(NoActionRule), ex.RuleType);
        }

        [Fact]
        public void Load_CannotActivateRule_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            var ex = Assert.Throws<RuleActivationException>(() => target.Load(x => x.NestedTypes().From(typeof(CannotActivateRule))));
            Assert.Equal(typeof(CannotActivateRule), ex.RuleType);
        }

        [Fact]
        public void Load_AssemblyWithRulesToNamedRuleSet_RuleSetNameMatches()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            target.Load(x => x
                .NestedTypes()
                .From(ThisAssembly)
                .Where(r => r.RuleType == typeof(ValidRule))
                .To("Test"));
            var ruleSet = target.GetRuleSets().First();

            //Assert
            Assert.Equal("Test", ruleSet.Name);
        }

        [Fact]
        public void Load_AssemblyWithRulesToDefaultRuleSet_DefaultRuleSetName()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            target.Load(x => x
                .NestedTypes()
                .From(ThisAssembly)
                .Where(r => r.RuleType == typeof(ValidRule)));
            var ruleSet = target.GetRuleSets().First();

            //Assert
            Assert.Equal("default", ruleSet.Name);
        }

        [Fact]
        public void Load_FilterRuleByName_MatchingRule()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            target.Load(x => x
                .NestedTypes()
                .From(ThisAssembly)
                .Where(r => r.Name.Contains("Valid")));
            var ruleSet = target.GetRuleSets().First();

            //Assert
            Assert.Single(ruleSet.Rules);
            Assert.Equal(typeof(ValidRule).FullName, ruleSet.Rules.First().Name);
        }

        [Fact]
        public void Load_FilterRuleByTag_MatchingRule()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            target.Load(x => x
                .NestedTypes()
                .From(ThisAssembly)
                .Where(r => r.IsTagged("LoadTest")));
            var ruleSet = target.GetRuleSets().First();

            //Assert
            Assert.Single(ruleSet.Rules);
            Assert.Equal("Rule with metadata", ruleSet.Rules.First().Name);
        }

        [Fact]
        public void Add_RuleInstanceInRuleSet_AddedToRepository()
        {
            //Arrange
            var rule = new ValidRule();

            var factory = new RuleDefinitionFactory();
            var ruleDefinition = factory.Create(rule);
            
            var ruleSet = new RuleSet("MyRuleSet");
            ruleSet.Add(ruleDefinition);

            var target = CreateTarget();

            //Act
            target.Add(ruleSet);

            //Assert
            Assert.Single(ruleSet.Rules);
            Assert.Equal(typeof(ValidRule).FullName, ruleSet.Rules.First().Name);
        }

        private Assembly ThisAssembly => GetType().Assembly;

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

        public class NoActionRule : Rule
        {
            public override void Define()
            {
                When()
                    .Match<FactType>();
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