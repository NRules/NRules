using Moq;
using NRules.Fluent;
using NRules.RuleModel;
using System.Linq;
using System.Threading;
using Xunit;

namespace NRules.Tests
{
    public class RuleCompilerTest
    {
        [Fact]
        public void Compile_CancellationRequested()
        {
            using (var cancellationSource = new CancellationTokenSource())
            {
                // Arrange
                var ruleCompiler = new RuleCompiler();

                var rule1 = CreateRuleDefinitionMock();
                var rule2 = CreateRuleDefinitionMock();
                rule2.Setup(r => r.LeftHandSide).Returns(() =>
                    {
                        cancellationSource.Cancel();
                        return new AndElement(Enumerable.Empty<RuleElement>());
                    });

                var rule3 = CreateRuleDefinitionMock();
                var rules = new[] { rule1.Object, rule2.Object, rule3.Object };

                // Act
                ruleCompiler.Compile(rules, cancellationSource.Token);

                // Assert
                rule1.Verify(r => r.LeftHandSide, Times.AtLeastOnce);
                rule2.Verify(r => r.LeftHandSide, Times.AtLeastOnce);
                rule3.Verify(r => r.LeftHandSide, Times.Never);
            }
        }

        [Fact]
        public void Compile_RuleSets_CancellationRequested()
        {
            using (var cancellationSource = new CancellationTokenSource())
            {
                // Arrange
                var ruleCompiler = new RuleCompiler();

                var rule1_1 = CreateRuleDefinitionMock();
                var rule2_1 = CreateRuleDefinitionMock();
                var rule2_2 = CreateRuleDefinitionMock();
                rule2_2.Setup(r => r.LeftHandSide).Returns(() =>
                {
                    cancellationSource.Cancel();
                    return new AndElement(Enumerable.Empty<RuleElement>());
                });

                var rule2_3 = CreateRuleDefinitionMock();
                var rule3_1 = CreateRuleDefinitionMock();

                var rules1 = new RuleSet("R1");
                rules1.Add(new[] { rule1_1.Object });

                var rules2 = new RuleSet("R1");
                rules2.Add(new[] { rule2_1.Object, rule2_2.Object, rule2_3.Object });

                var rules3 = new RuleSet("R1"); ;
                rules3.Add(new[] { rule3_1.Object });

                var ruleSets = new[] { rules1, rules2, rules3 };

                // Act
                ruleCompiler.Compile(ruleSets, cancellationSource.Token);

                // Assert
                rule1_1.Verify(r => r.LeftHandSide, Times.AtLeastOnce);
                rule2_1.Verify(r => r.LeftHandSide, Times.AtLeastOnce);
                rule2_2.Verify(r => r.LeftHandSide, Times.AtLeastOnce);
                rule2_3.Verify(r => r.LeftHandSide, Times.Never);
                rule3_1.Verify(r => r.LeftHandSide, Times.Never);
            }
        }

        [Fact]
        public void RuleRepositoryExtension_Compile_CancellationRequested()
        {
            using (var cancellationSource = new CancellationTokenSource())
            {
                // Arrange
                var ruleCompiler = new RuleCompiler();

                var rule1 = CreateRuleDefinitionMock();
                var rule2 = CreateRuleDefinitionMock();
                rule2.Setup(r => r.LeftHandSide).Returns(() =>
                {
                    cancellationSource.Cancel();
                    return new AndElement(Enumerable.Empty<RuleElement>());
                });

                var rule3 = CreateRuleDefinitionMock();
                var rules = new RuleSet("R1");
                rules.Add(new[] { rule1.Object, rule2.Object, rule3.Object });

                var ruleRepository = new RuleRepository();
                ruleRepository.Add(rules);

                // Act
                ruleRepository.Compile(cancellationSource.Token);

                // Assert
                rule1.Verify(r => r.LeftHandSide, Times.AtLeastOnce);
                rule2.Verify(r => r.LeftHandSide, Times.AtLeastOnce);
                rule3.Verify(r => r.LeftHandSide, Times.Never);
            }
        }

        private Mock<IRuleDefinition> CreateRuleDefinitionMock()
        {
            var rule = new Mock<IRuleDefinition>();
            rule.Setup(r => r.Name).Returns("Rule");
            rule.Setup(r => r.LeftHandSide).Returns(new AndElement(Enumerable.Empty<RuleElement>()));
            rule.Setup(r => r.DependencyGroup).Returns(new DependencyGroupElement(Enumerable.Empty<DependencyElement>()));
            rule.Setup(r => r.RightHandSide).Returns(new ActionGroupElement(Enumerable.Empty<ActionElement>()));
            rule.Setup(r => r.FilterGroup).Returns(new FilterGroupElement(Enumerable.Empty<FilterElement>()));
            return rule;
        }
    }
}
