using System;
using NRules.Rule.Builders;
using NUnit.Framework;

namespace NRules.Rule.Tests
{
    [TestFixture]
    public class RuleBuilderTest
    {
        [Test]
        public void Build_BuilderNotInitialized_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            Assert.Throws<InvalidOperationException>(() => target.Build());
        }

        private RuleBuilder CreateTarget()
        {
            return new RuleBuilder();
        }
    }
}