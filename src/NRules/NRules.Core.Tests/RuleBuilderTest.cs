using System;
using System.Linq.Expressions;
using NRules.Core.Rules;
using NUnit.Framework;

namespace NRules.Core.Tests
{
    [TestFixture]
    public class RuleBuilderTest
    {
        private CompiledRule _rule;

        [SetUp]
        public void SetUp()
        {
            _rule = new CompiledRule();
        }

        [Test]
        public void Ctor_Called_RuleUninitialized()
        {
            //Arrange
            //Act
            var target = CreateTarget();

            //Assert
            Assert.AreEqual(string.Empty, _rule.Name);
            Assert.AreEqual(0, _rule.Conditions.Count);
            Assert.AreEqual(0, _rule.Actions.Count);
        }

        [Test]
        public void Name_Called_SetsRuleName()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            target.Name("Test Name");

            //Assert
            Assert.AreEqual("Test Name", _rule.Name);
        }

        [Test]
        public void Condition_Called_AddsCondition()
        {
            //Arrange
            var target = CreateTarget();
            Expression<Func<string, bool>> expression = s => s.StartsWith("Prefix");

            //Act
            target.Condition(expression);

            //Assert
            Assert.AreEqual(1, _rule.Conditions.Count);
        }

        [Test]
        public void Action_Called_AddsAction()
        {
            //Arrange
            var target = CreateTarget();
            Action<IActionContext> action = ctx => ctx.Arg<string>();

            //Act
            target.Action(action);

            //Assert
            Assert.AreEqual(1, _rule.Actions.Count);
        }

        private RuleBuilder CreateTarget()
        {
            return new RuleBuilder(_rule);
        }
    }
}