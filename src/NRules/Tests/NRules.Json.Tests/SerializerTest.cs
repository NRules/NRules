using System;
using System.IO;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Tests.TestAssets;
using NRules.Json.Tests.Utilities;
using NRules.RuleModel;
using NRules.RuleModel.Builders;
using Xunit;

namespace NRules.Json.Tests
{
    public class SerializerTest
    {
        [Fact]
        public void RoundTrip()
        {
            //Arrange
            var rule = BuildTestRule();
            var options = JsonOptionsFactory.Create();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            //Act
            var jsonString = JsonSerializer.Serialize(rule, options);
            //File.WriteAllText(@"C:\temp\rule.json", jsonString);
            var ruleClone = JsonSerializer.Deserialize<IRuleDefinition>(jsonString, options);

            //Assert
            Assert.True(RuleDefinitionComparer.AreEqual(rule, ruleClone));
        }

        private IRuleDefinition BuildTestRule()
        {
            var builder = new RuleBuilder();
            builder.Name("Test Rule");
            builder.Description("My test rule");
            builder.Priority(2);
            builder.Repeatability(RuleRepeatability.NonRepeatable);

            builder.Tag("Test");
            builder.Tag("Debug");

            builder.Property("ClrType", typeof(SerializerTest));

            builder.Dependencies()
                .Dependency(typeof(ITestService), "service");

            PatternBuilder fact1Pattern = builder.LeftHandSide().Pattern(typeof(FactType1), "fact1");
            Expression<Func<FactType1, bool>> fact1Condition = fact1 => fact1.BooleanProperty;
            fact1Pattern.Condition(fact1Condition);

            Expression<Func<FactType1, bool>> filter1 = (fact1) => fact1.BooleanProperty;
            builder.Filters()
                .Filter(FilterType.Predicate, filter1);
            Expression<Func<FactType1, object>> filter2 = (fact1) => fact1.StringProperty;
            builder.Filters()
                .Filter(FilterType.KeyChange, filter2);

            Expression<Action<IContext, FactType1, ITestService>> action = (ctx, fact1, service) => Calculations.DoSomething(fact1, service);
            builder.RightHandSide().Action(action);

            return builder.Build();
        }
    }
}
