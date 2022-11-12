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
            var builder = new RuleBuilder();
            builder.Name("Test Rule");
            builder.Description("My test rule");
            builder.Priority(2);
            builder.Repeatability(RuleRepeatability.NonRepeatable);

            builder.Tag("Test");
            builder.Tag("Debug");

            builder.Property("ClrType", typeof(RuleSerializerTest));

            var fact1Pattern = builder.LeftHandSide().Pattern(typeof(FactType1), "fact1");
            Expression<Func<FactType1, bool>> fact1Condition = fact1 => fact1.BooleanProperty;
            fact1Pattern.Condition(fact1Condition);

            Expression<Action<IContext, FactType1>> action = (ctx, fact1) => Calculations.DoSomething(fact1);
            builder.RightHandSide().Action(action);
            var ruleDefinition = builder.Build();

            TestRoundtrip(ruleDefinition);
        }

        [Fact]
        public void Roundtrip_RuleWithFilter_Equals()
        {
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            builder.LeftHandSide().Pattern(typeof(FactType1), "fact1");

            Expression<Func<FactType1, bool>> filter1 = (fact1) => fact1.BooleanProperty;
            builder.Filters()
                .Filter(FilterType.Predicate, filter1);
            Expression<Func<FactType1, object?>> filter2 = (fact1) => fact1.StringProperty;
            builder.Filters()
                .Filter(FilterType.KeyChange, filter2);

            Expression<Action<IContext, FactType1>> action = (ctx, fact1) => Calculations.DoSomething(fact1);
            builder.RightHandSide().Action(action);
            var ruleDefinition = builder.Build();

            TestRoundtrip(ruleDefinition);
        }

        [Fact]
        public void Roundtrip_RuleWithDependency_Equals()
        {
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            builder.Dependencies()
                .Dependency(typeof(ITestService), "service");

            builder.LeftHandSide().Pattern(typeof(FactType1), "fact1");

            Expression<Action<IContext, FactType1, ITestService>> action = (ctx, fact1, service) => Calculations.DoSomething(fact1, service);
            builder.RightHandSide().Action(action);
            var ruleDefinition = builder.Build();

            TestRoundtrip(ruleDefinition);
        }

        [Fact]
        public void Roundtrip_TwoFactJoinRule_Equals()
        {
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
            var ruleDefinition = builder.Build();

            TestRoundtrip(ruleDefinition);
        }

        [Fact]
        public void Roundtrip_ExistsRule_Equals()
        {
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            builder.LeftHandSide().Exists().Pattern(typeof(FactType1), "fact1");

            Expression<Action<IContext>> action = ctx
                => Calculations.DoSomething();
            builder.RightHandSide().Action(action);
            var ruleDefinition = builder.Build();

            TestRoundtrip(ruleDefinition);
        }

        [Fact]
        public void Roundtrip_NotRule_Equals()
        {
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            builder.LeftHandSide().Not().Pattern(typeof(FactType1), "fact1");

            Expression<Action<IContext>> action = ctx
                => Calculations.DoSomething();
            builder.RightHandSide().Action(action);
            var ruleDefinition = builder.Build();

            TestRoundtrip(ruleDefinition);
        }

        [Fact]
        public void Roundtrip_AggregateRule_Equals()
        {
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            var factGroupPattern = builder.LeftHandSide().Pattern(typeof(IEnumerable<FactType1>), "factGroup");

            var aggregate = factGroupPattern.Aggregate();
            Expression<Func<FactType1, string?>> keySelector = fact1 => fact1.GroupKey;
            Expression<Func<FactType1, FactType1>> elementSelector = fact1 => fact1;
            aggregate.GroupBy(keySelector, elementSelector);

            var fact1Pattern = aggregate.Pattern(typeof(FactType1), "fact1");
            Expression<Func<FactType1, bool>> fact1Condition = fact1 => fact1.BooleanProperty;
            fact1Pattern.Condition(fact1Condition);

            Expression<Action<IContext, IEnumerable<FactType1>>> action = (ctx, factGroup)
                => Calculations.DoSomething(factGroup);
            builder.RightHandSide().Action(action);
            var ruleDefinition = builder.Build();

            TestRoundtrip(ruleDefinition);
        }

        [Fact]
        public void Roundtrip_BindingRule_Equals()
        {
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            builder.LeftHandSide().Pattern(typeof(FactType1), "fact1");

            var bindingPattern = builder.LeftHandSide().Pattern(typeof(int), "length");

            var binding = bindingPattern.Binding();
            Expression<Func<FactType1, int>> expression = fact1 => fact1.StringProperty!.Length;
            binding.BindingExpression(expression);

            Expression<Action<IContext, int>> action = (ctx, length)
                => Calculations.DoSomething(length);
            builder.RightHandSide().Action(action);
            var ruleDefinition = builder.Build();

            TestRoundtrip(ruleDefinition);
        }

        [Fact]
        public void Roundtrip_ForAllRule_Equals()
        {
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            var forAll = builder.LeftHandSide().ForAll();
            var basePattern = forAll.BasePattern(typeof(FactType1));
            var baseParameter = Expression.Parameter(basePattern.Declaration.Type, basePattern.Declaration.Name);
            var baseCondition = Expression.Lambda<Func<FactType1, bool>>(
                Expression.Property(baseParameter, nameof(FactType1.BooleanProperty)),
                baseParameter);
            basePattern.Condition(baseCondition);

            var pattern1 = forAll.Pattern(typeof(FactType1));
            var parameter1 = Expression.Parameter(pattern1.Declaration.Type, pattern1.Declaration.Name);
            var condition1 = Expression.Lambda<Func<FactType1, bool>>(
                Expression.Call(
                    Expression.Property(parameter1, nameof(FactType1.StringProperty)),
                    nameof(string.StartsWith),
                    Type.EmptyTypes,
                    Expression.Constant("Valid")),
                parameter1);
            pattern1.Condition(condition1);

            Expression<Action<IContext>> action = ctx => Calculations.DoSomething();
            builder.RightHandSide().Action(action);
            var ruleDefinition = builder.Build();

            TestRoundtrip(ruleDefinition);
        }

        private void TestRoundtrip(IRuleDefinition original)
        {
            var jsonString = JsonSerializer.Serialize(original, _options);
            //System.IO.File.WriteAllText(@"C:\temp\rule.json", jsonString);
            var deserialized = JsonSerializer.Deserialize<IRuleDefinition>(jsonString, _options);
            Assert.True(RuleDefinitionComparer.AreEqual(original, deserialized));
        }
    }
}
