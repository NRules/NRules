using System;
using System.Linq.Expressions;
using System.Text.Json;
using NRules.Json.Tests.TestAssets;
using NRules.Json.Tests.Utilities;
using NRules.RuleModel;
using NRules.RuleModel.Builders;
using Xunit;

namespace NRules.Json.Tests
{
    public class RuleSerializerTest
    {
        private readonly JsonSerializerOptions _options;

        public RuleSerializerTest()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            RuleSerializer.Setup(_options);
        }

        [Fact]
        public void Roundtrip_SimpleMatchRuleWithMetadata_Equals()
        {
            //Arrange
            var builder = new RuleBuilder();
            builder.Name("Test Rule");
            builder.Description("My test rule");
            builder.Priority(2);
            builder.Repeatability(RuleRepeatability.NonRepeatable);

            builder.Tag("Test");
            builder.Tag("Debug");

            builder.Property("ClrType", typeof(RuleSerializerTest));

            PatternBuilder fact1Pattern = builder.LeftHandSide().Pattern(typeof(FactType1), "fact1");
            Expression<Func<FactType1, bool>> fact1Condition = fact1 => fact1.BooleanProperty;
            fact1Pattern.Condition(fact1Condition);

            Expression<Action<IContext, FactType1>> action = (ctx, fact1) => Calculations.DoSomething(fact1);
            builder.RightHandSide().Action(action);
            var original = builder.Build();

            //Act
            var deserialized = Roundtrip(original);

            //Assert
            Assert.True(RuleDefinitionComparer.AreEqual(original, deserialized));
        }

        [Fact]
        public void Roundtrip_RuleWithFilter_Equals()
        {
            //Arrange
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            builder.LeftHandSide().Pattern(typeof(FactType1), "fact1");

            Expression<Func<FactType1, bool>> filter1 = (fact1) => fact1.BooleanProperty;
            builder.Filters()
                .Filter(FilterType.Predicate, filter1);
            Expression<Func<FactType1, object>> filter2 = (fact1) => fact1.StringProperty;
            builder.Filters()
                .Filter(FilterType.KeyChange, filter2);

            Expression<Action<IContext, FactType1>> action = (ctx, fact1) => Calculations.DoSomething(fact1);
            builder.RightHandSide().Action(action);
            var original = builder.Build();

            //Act
            var deserialized = Roundtrip(original);

            //Assert
            Assert.True(RuleDefinitionComparer.AreEqual(original, deserialized));
        }

        [Fact]
        public void Roundtrip_RuleWithDependency_Equals()
        {
            //Arrange
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            builder.Dependencies()
                .Dependency(typeof(ITestService), "service");

            builder.LeftHandSide().Pattern(typeof(FactType1), "fact1");

            Expression<Action<IContext, FactType1, ITestService>> action = (ctx, fact1, service) => Calculations.DoSomething(fact1, service);
            builder.RightHandSide().Action(action);
            var original = builder.Build();

            //Act
            var deserialized = Roundtrip(original);

            //Assert
            Assert.True(RuleDefinitionComparer.AreEqual(original, deserialized));
        }
        
        [Fact]
        public void Roundtrip_TwoFactJoinRule_Equals()
        {
            //Arrange
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            builder.LeftHandSide().Pattern(typeof(FactType1), "fact1");
            var pattern2 = builder.LeftHandSide().Pattern(typeof(FactType2), "fact2");
            Expression<Func<FactType1, FactType2, bool>> condition21 = (fact1, fact2) 
                => fact2.JoinProperty == fact1;
            pattern2.Condition(condition21);

            Expression<Action<IContext, FactType1, FactType2>> action = (ctx, fact1, fact2) 
                => Calculations.DoSomething(fact1, fact2);
            builder.RightHandSide().Action(action);
            var original = builder.Build();

            //Act
            var deserialized = Roundtrip(original);

            //Assert
            Assert.True(RuleDefinitionComparer.AreEqual(original, deserialized));
        }

        private IRuleDefinition Roundtrip(IRuleDefinition original)
        {
            var jsonString = JsonSerializer.Serialize(original, _options);
            //System.IO.File.WriteAllText(@"C:\temp\rule.json", jsonString);
            var deserialized = JsonSerializer.Deserialize<IRuleDefinition>(jsonString, _options);
            return deserialized;
        }
    }
}
