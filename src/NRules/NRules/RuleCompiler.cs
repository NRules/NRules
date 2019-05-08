using System;
using System.Collections.Generic;
using System.Linq;
using NRules.AgendaFilters;
using NRules.Aggregators;
using NRules.Rete;
using NRules.RuleModel;
using NRules.RuleModel.Builders;
using NRules.Utilities;

namespace NRules
{
    /// <summary>
    /// Compiles rules in a canonical rule model form into an executable representation.
    /// </summary>
    public class RuleCompiler
    {
        private readonly AggregatorRegistry _aggregatorRegistry = new AggregatorRegistry();

        /// <summary>
        /// Registry of custom aggregator factories.
        /// </summary>
        public AggregatorRegistry AggregatorRegistry => _aggregatorRegistry;

        /// <summary>
        /// Compiles a collection of rules into a session factory.
        /// </summary>
        /// <param name="ruleDefinitions">Rules to compile.</param>
        /// <returns>Session factory.</returns>
        /// <exception cref="RuleCompilationException">Any fatal error during rules compilation.</exception>
        /// <seealso cref="IRuleRepository"/>
        public ISessionFactory Compile(IEnumerable<IRuleDefinition> ruleDefinitions)
        {
            IReteBuilder reteBuilder = new ReteBuilder(_aggregatorRegistry);
            var compiledRules = new List<ICompiledRule>();
            foreach (var ruleDefinition in ruleDefinitions)
            {
                try
                {
                    var compiledRule = CompileRule(reteBuilder, ruleDefinition);
                    compiledRules.Add(compiledRule);
                }
                catch (Exception e)
                {
                    throw new RuleCompilationException("Failed to compile rule", ruleDefinition.Name, e);
                }
            }

            INetwork network = reteBuilder.Build();
            var factory = new SessionFactory(network, compiledRules);
            return factory;
        }

        /// <summary>
        /// Compiles rules from rule sets into a session factory.
        /// </summary>
        /// <param name="ruleSets">Rule sets to compile.</param>
        /// <returns>Session factory.</returns>
        public ISessionFactory Compile(IEnumerable<IRuleSet> ruleSets)
        {
            var rules = ruleSets.SelectMany(x => x.Rules);
            return Compile(rules);
        }

        private ICompiledRule CompileRule(IReteBuilder reteBuilder, IRuleDefinition ruleDefinition)
        {
            var transformation = new RuleTransformation();
            var transformedRule = transformation.Transform(ruleDefinition);
            var ruleDeclarations = transformedRule.LeftHandSide.Exports.ToList();
            var ruleDependencies = transformedRule.DependencyGroup.Dependencies.Select(x => x.Declaration).ToList();

            IEnumerable<IRuleDependency> dependencies = CompileDependencies(transformedRule);
            IEnumerable<ITerminalNode> terminals = reteBuilder.AddRule(transformedRule);

            IRuleFilter filter = CompileFilters(transformedRule, ruleDeclarations);
            
            var rightHandSide = transformedRule.RightHandSide;
            var actions = new List<IRuleAction>();
            foreach (var action in rightHandSide.Actions)
            {
                var ruleAction = ExpressionCompiler.CompileAction(action, ruleDeclarations, ruleDependencies);
                actions.Add(ruleAction);
            }

            var rule = new CompiledRule(ruleDefinition, ruleDeclarations, actions, dependencies, filter);
            BuildRuleNode(rule, terminals);

            return rule;
        }

        private IEnumerable<IRuleDependency> CompileDependencies(IRuleDefinition ruleDefinition)
        {
            foreach (var dependency in ruleDefinition.DependencyGroup.Dependencies)
            {
                var compiledDependency = new RuleDependency(dependency.Declaration, dependency.ServiceType);
                yield return compiledDependency;
            }
        }

        private IRuleFilter CompileFilters(IRuleDefinition ruleDefinition, IList<Declaration> ruleDeclarations)
        {
            var conditions = new List<IActivationCondition>();
            var keySelectors = new List<IActivationExpression>();
            foreach (var filter in ruleDefinition.FilterGroup.Filters)
            {
                switch (filter.FilterType)
                {
                    case FilterType.Predicate:
                        var condition = ExpressionCompiler.CompileFilterCondition(filter, ruleDeclarations);
                        conditions.Add(condition);
                        break;
                    case FilterType.KeyChange:
                        var keySelector = ExpressionCompiler.CompileFilterExpression(filter, ruleDeclarations);
                        keySelectors.Add(keySelector);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unrecognized filter type. FilterType={filter.FilterType}");
                }
            }
            var compiledFilter = new RuleFilter(conditions, keySelectors);
            return compiledFilter;
        }

        private void BuildRuleNode(ICompiledRule compiledRule, IEnumerable<ITerminalNode> terminalNodes)
        {
            var ruleNode = new RuleNode(compiledRule);
            foreach (var terminalNode in terminalNodes)
            {
                terminalNode.Attach(ruleNode);
            }
        }
    }
}
