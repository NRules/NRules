using System;
using System.Linq;
using System.Linq.Expressions;
using NRules.Dsl;
using NUnit.Framework;

namespace NRules.Rule.Tests
{
    [TestFixture]
    public class RuleBuilderTest
    {
        private RuleDefinition _ruleDefinition;

        [SetUp]
        public void SetUp()
        {
            _ruleDefinition = new RuleDefinition();
        }

        [Test]
        public void Ctor_Called_RuleUninitialized()
        {
            //Arrange
            //Act
            var target = CreateTarget();

            //Assert
            Assert.AreEqual(string.Empty, _ruleDefinition.Name);
            Assert.AreEqual(0, _ruleDefinition.LeftHandSide.ChildElements.Count());
            Assert.AreEqual(0, _ruleDefinition.RightHandSide.Count());
        }

        [Test]
        public void Name_Called_SetsRuleName()
        {
            //Arrange
            var target = CreateTarget();

            //Act
            target.Name("Test Name");

            //Assert
            Assert.AreEqual("Test Name", _ruleDefinition.Name);
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
            Assert.AreEqual(1, _ruleDefinition.LeftHandSide.ChildElements.Count());
        }

        [Test]
        public void Action_Called_AddsAction()
        {
            //Arrange
            var target = CreateTarget();
            Expression<Action<IActionContext>> action = ctx => ctx.Arg<string>();

            //Act
            target.Action(action);

            //Assert
            Assert.AreEqual(1, _ruleDefinition.RightHandSide.Count());
        }

        private RuleBuilder CreateTarget()
        {
            return new RuleBuilder(_ruleDefinition);
        }
    }
}