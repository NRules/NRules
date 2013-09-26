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
            Assert.AreEqual(0, _rule.LeftSide.ChildElements.Count());
            Assert.AreEqual(0, _rule.RightSide.Count());
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
            Assert.AreEqual(1, _rule.LeftSide.ChildElements.Count());
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
            Assert.AreEqual(1, _rule.RightSide.Count());
        }

        private RuleBuilder CreateTarget()
        {
            return new RuleBuilder(_rule);
        }
    }
}