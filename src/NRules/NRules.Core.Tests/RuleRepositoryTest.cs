using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NRules.Core.Rete;
using NRules.Core.Rules;
using NRules.Core.Tests.TestAssets;
using NUnit.Framework;

namespace NRules.Core.Tests
{
    [TestFixture]
    public class RuleRepositoryTest
    {
        private Assembly _ruleAssembly;
        private Assembly _badAssembly;

        private Fact _helloFact;
        private Fact _goodbyeFact;
        private Fact _relevantFact;
        private Fact _irrelevantFact;

        [SetUp]
        public void Setup()
        {
            _ruleAssembly = Assembly.GetAssembly(typeof (TestRule1));
            _badAssembly = Assembly.GetAssembly(typeof (Object));

            var testFact1 = new TestFact1 {Name = "Hello"};
            var testFact2 = new TestFact1 {Name = "Goodbye"};
            _helloFact = new Fact(testFact1);
            _goodbyeFact = new Fact(testFact2);

            _relevantFact = new Fact(new TestFact2 {Amount = 100, Fact1 = testFact1});
            _irrelevantFact = new Fact(new TestFact2 {Amount = 100, Fact1 = testFact2});
        }

        public static RuleRepository CreateTarget()
        {
            return new RuleRepository();
        }

        [Test]
        public void AddRuleSet_NoRulesInAssembly_ThrowsException()
        {
            // Arrange
            var target = CreateTarget();

            // Act - Assert
            Assert.Throws<ArgumentException>(() => target.AddRuleSet(_badAssembly));
        }

        [Test]
        public void AddRuleSet_ValidRulesInAssembly_DoesNotThrowException()
        {
            // Arrange
            var target = CreateTarget();

            // Act - Assert
            Assert.DoesNotThrow(() => target.AddRuleSet(_ruleAssembly));
        }

        [Test]
        public void Compile_NoValidRuleSetAdded_ThrowsException()
        {
            // Arrange
            var target = CreateTarget();

            // Act - Assert
            var rules = target.Compile(); //delayed execution
            Assert.Throws<ArgumentException>(() => rules.ToList());
        }

        [Test]
        public void Compile_ValidRulesInAssembly_ReturnsCorrectDeclarations()
        {
            // Arrange
            var target = CreateTarget();
            target.AddRuleSet(_ruleAssembly);

            // Act
            List<Rule> rules = target.Compile().ToList();

            // Assert
            Assert.AreEqual(1, rules.Count);
            List<IDeclaration> declarations = rules[0].Declarations.ToList();
            Assert.AreEqual(2, declarations.Count);
            Assert.True(declarations.Contains(new Declaration("", typeof (TestFact1))));
            Assert.True(declarations.Contains(new Declaration("", typeof (TestFact2))));
        }

        [Test]
        public void Compile_ValidRulesInAssembly_ReturnsCorrectConditions()
        {
            // Arrange
            var target = CreateTarget();
            target.AddRuleSet(_ruleAssembly);

            // Act
            List<Rule> rules = target.Compile().ToList();

            // Assert
            Assert.AreEqual(1, rules.Count);
            List<ICondition> conditions = rules[0].Conditions.ToList();
            Assert.AreEqual(1, conditions.Count);
            Assert.AreEqual(typeof (TestFact1), conditions[0].FactType);
            Assert.True(conditions[0].IsSatisfiedBy(_helloFact));
            Assert.False(conditions[0].IsSatisfiedBy(_goodbyeFact));
        }

        [Test]
        public void Compile_ValidRulesInAssembly_ReturnsCorrectJoinConditions()
        {
            // Arrange
            var target = CreateTarget();
            target.AddRuleSet(_ruleAssembly);

            // Act
            List<Rule> rules = target.Compile().ToList();

            // Assert
            Assert.AreEqual(1, rules.Count);
            List<IJoinCondition> joinConditions = rules[0].JoinConditions.ToList();
            Assert.AreEqual(1, joinConditions.Count);
            Assert.True(joinConditions[0].FactTypes.Contains(typeof (TestFact1)));
            Assert.True(joinConditions[0].FactTypes.Contains(typeof (TestFact2)));
            Assert.True(joinConditions[0].IsSatisfiedBy(new[] {_helloFact, _relevantFact}));
            Assert.False(joinConditions[0].IsSatisfiedBy(new[] {_helloFact, _irrelevantFact}));
            Assert.False(joinConditions[0].IsSatisfiedBy(new[] {_goodbyeFact, _relevantFact}));
            Assert.True(joinConditions[0].IsSatisfiedBy(new[] {_goodbyeFact, _irrelevantFact}));
        }

        [Test]
        public void Compile_ValidRulesInAssembly_ReturnsCorrectActions()
        {
            // Arrange
            var target = CreateTarget();
            target.AddRuleSet(_ruleAssembly);

            // Act
            List<Rule> rules = target.Compile().ToList();

            // Assert
            Assert.AreEqual(1, rules.Count);
            List<IRuleAction> actions = rules[0].Actions.ToList();
            Assert.AreEqual(2, actions.Count);
            //todo: this can probably be better.
        }
    }
}