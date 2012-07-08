using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NRules.Core.Tests
{
    [TestFixture]
    public class RuleSetTest
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void RuleSet_Initialized_ExposesAllRuleTypes()
        {
            // Arrange
            List<Type> types = new List<Type>();
            types.Add(typeof(Agenda));
            types.Add(typeof(Session));
            types.Add(typeof(SessionFactory));

            // Act
            var target = CreateTarget(types);

            // Assert
            List<Type> ruleTypes = target.RuleTypes.ToList();
            Assert.AreEqual(3, ruleTypes.Count);
            Assert.AreEqual(typeof(Agenda), ruleTypes[0]);
            Assert.AreEqual(typeof(Session), ruleTypes[1]);
            Assert.AreEqual(typeof(SessionFactory), ruleTypes[2]);
        }

        public static RuleSet CreateTarget(IEnumerable<Type> types)
        {
            return new RuleSet(types, null);
        }
    }
}
