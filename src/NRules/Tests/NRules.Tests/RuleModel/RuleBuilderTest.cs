using System;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;
using NRules.RuleModel.Builders;
using Xunit;

namespace NRules.Tests.RuleModel
{
    public class RuleBuilderTest
    {
        [Fact]
        public void Build_ValidRule_Builds()
        {
            //Arrange
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            var lhs = builder.LeftHandSide();

            var patternBuilder1 = lhs.Pattern(typeof(FactType1), "fact1");
            Expression<Func<FactType1, bool>> condition11 = fact1 => fact1.TestProperty.StartsWith("Valid");
            patternBuilder1.Condition(condition11);

            var patternBuilder2 = lhs.Pattern(typeof(FactType2), "fact2");
            Expression<Func<FactType2, bool>> condition21 = fact2 => fact2.TestProperty.StartsWith("Valid");
            patternBuilder2.Condition(condition21);
            Expression<Func<FactType1, FactType2, bool>> condition22 = (fact1, fact2) => fact2.JoinProperty == fact1.TestProperty;
            patternBuilder2.Condition(condition22);

            Expression<Func<FactType1, bool>> filter1 = fact1 => fact1.TestProperty == "Valid Value";
            builder.Filters().Filter(FilterType.Predicate, filter1);

            Expression<Action<IContext, FactType1, FactType2>> action = (context, fact1, fact2) => NoOp();
            builder.RightHandSide().Action(action);

            //Act
            var rule = builder.Build();

            //Assert
            Assert.Equal(2, rule.LeftHandSide.ChildElements.Count());
        }

        [Fact]
        public void Build_InvalidConditionDependency_Throws()
        {
            //Arrange
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            var lhs = builder.LeftHandSide();

            var patternBuilder1 = lhs.Pattern(typeof(FactType1), "fact1");
            Expression<Func<FactType1, bool>> condition11 = invalidName => invalidName.TestProperty.StartsWith("Valid");
            patternBuilder1.Condition(condition11);
            
            Expression<Action<IContext>> action = context => NoOp();
            builder.RightHandSide().Action(action);

            //Act - Assert
            var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
            Assert.Equal("Undefined variables in rule match conditions. Variables=invalidName", ex.Message);
        }

        [Fact]
        public void Build_InvalidFilterDependency_Throws()
        {
            //Arrange
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            var lhs = builder.LeftHandSide();

            var patternBuilder1 = lhs.Pattern(typeof(FactType1), "fact1");
            Expression<Func<FactType1, bool>> condition11 = fact1 => fact1.TestProperty.StartsWith("Valid");
            patternBuilder1.Condition(condition11);
            
            Expression<Func<FactType1, bool>> filter1 = invalidName => invalidName.TestProperty == "Valid Value";
            builder.Filters().Filter(FilterType.Predicate, filter1);

            Expression<Action<IContext>> action = context => NoOp();
            builder.RightHandSide().Action(action);

            //Act - Assert
            var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
            Assert.Equal("Undefined variables in rule filter. Variables=invalidName", ex.Message);
        }

        [Fact]
        public void Build_InvalidActionDependency_Throws()
        {
            //Arrange
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            var lhs = builder.LeftHandSide();

            var patternBuilder1 = lhs.Pattern(typeof(FactType1), "fact1");
            Expression<Func<FactType1, bool>> condition11 = fact1 => fact1.TestProperty.StartsWith("Valid");
            patternBuilder1.Condition(condition11);
            
            Expression<Action<IContext, FactType1>> action = (context, invalidName) => NoOp();
            builder.RightHandSide().Action(action);

            //Act - Assert
            var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
            Assert.Equal("Undefined variables in rule actions. Variables=invalidName", ex.Message);
        }
        
        [Fact]
        public void Build_DuplicatePatternDefinition_Throws()
        {
            //Arrange
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            var lhs = builder.LeftHandSide();
            lhs.Pattern(typeof(FactType1), "fact1");
            lhs.Pattern(typeof(FactType2), "fact1");
            
            Expression<Action<IContext>> action = (context) => NoOp();
            builder.RightHandSide().Action(action);

            //Act - Assert
            var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
            Assert.Equal("Duplicate declarations. Declaration=fact1", ex.Message);
        }
        
        [Fact]
        public void Build_DuplicatePatternAndDependencyDefinition_Throws()
        {
            //Arrange
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            var dgb = builder.Dependencies();
            dgb.Dependency(typeof(ITestService), "variable1");

            var lhs = builder.LeftHandSide();
            lhs.Pattern(typeof(FactType1), "variable1");
            
            Expression<Action<IContext>> action = (context) => NoOp();
            builder.RightHandSide().Action(action);

            //Act - Assert
            var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
            Assert.Equal("Duplicate declarations. Declaration=variable1", ex.Message);
        }
        
        [Fact]
        public void Build_DuplicateDependencyDefinition_Throws()
        {
            //Arrange
            var builder = new RuleBuilder();
            builder.Name("Test Rule");

            var dgb = builder.Dependencies();
            dgb.Dependency(typeof(ITestService), "variable1");
            dgb.Dependency(typeof(ITestService), "variable1");

            var lhs = builder.LeftHandSide();
            lhs.Pattern(typeof(FactType1), "fact1");
            
            Expression<Action<IContext>> action = (context) => NoOp();
            builder.RightHandSide().Action(action);

            //Act - Assert
            var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
            Assert.Equal("Duplicate declarations. Declaration=variable1", ex.Message);
        }

        public static void NoOp()
        {
        }

        public class FactType1
        {
            public string TestProperty { get; set; }
        }

        public class FactType2
        {
            public string TestProperty { get; set; }
            public string JoinProperty { get; set; }
        }

        public interface ITestService
        {
        }    
    }
}
