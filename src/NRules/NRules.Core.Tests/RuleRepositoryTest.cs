using System;
using System.Linq;
using System.Reflection;
using NRules.Config;
using NRules.Core.Tests.TestAssets;
using NUnit.Framework;
using Rhino.Mocks;

namespace NRules.Core.Tests
{
    [TestFixture]
    public class RuleRepositoryTest
    {
        private Assembly _ruleAssembly;
        private Assembly _badAssembly;
        private IContainer _container;

        [SetUp]
        public void Setup()
        {
            _ruleAssembly = Assembly.GetAssembly(typeof (TestRule1));
            _badAssembly = Assembly.GetAssembly(typeof (Object));
            _container = MockRepository.GenerateStub<IContainer>();
        }

        private RuleRepository CreateTarget()
        {
            var repository = new RuleRepository();
            repository.Container = _container;
            return repository;
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
    }
}