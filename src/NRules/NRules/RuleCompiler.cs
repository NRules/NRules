using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        /// <param name="cancellationToken">Enables cooperative cancellation of the rules execution cycle.</param>
        /// <returns>Session factory.</returns>
        /// <exception cref="RuleCompilationException">Any fatal error during rules compilation.</exception>
        /// <seealso cref="IRuleRepository"/>
        public ISessionFactory Compile(IEnumerable<IRuleDefinition> ruleDefinitions, CancellationToken cancellationToken = default)
        {
            IReteBuilder reteBuilder = new ReteBuilder(_aggregatorRegistry);
            var compiledRules = new List<ICompiledRule>();
            foreach (var ruleDefinition in ruleDefinitions)
            {
                try
                {
                    var compiledRule = CompileRule(reteBuilder, ruleDefinition);
                    compiledRules.AddRange(compiledRule);
                }
                catch (Exception e)
                {
                    throw new RuleCompilationException("Failed to compile rule", ruleDefinition.Name, e);
                }

                if (cancellationToken.IsCancellationRequested) break;
            }

            INetwork network = reteBuilder.Build();
            var factory = new SessionFactory(network, compiledRules);
            return factory;
        }

        /// <summary>
        /// Compiles rules from rule sets into a session factory.
        /// </summary>
        /// <param name="ruleSets">Rule sets to compile.</param>
        /// <param name="cancellationToken">Enables cooperative cancellation of the rules execution cycle.</param>
        /// <returns>Session factory.</returns>
        public ISessionFactory Compile(IEnumerable<IRuleSet> ruleSets, CancellationToken cancellationToken = default)
        {
            var rules = ruleSets.SelectMany(x => x.Rules);
            return Compile(rules, cancellationToken);
        }

        private IEnumerable<ICompiledRule> CompileRule(IReteBuilder reteBuilder, IRuleDefinition ruleDefinition)
        {
            var rules = new List<ICompiledRule>();

            var transformation = new RuleTransformation();
            var transformedRule = transformation.Transform(ruleDefinition);
            var ruleDeclarations = transformedRule.LeftHandSide.Exports.ToList();

            var dependencies = transformedRule.DependencyGroup.Dependencies.ToList();
            var terminals = reteBuilder.AddRule(transformedRule);

            foreach (var terminal in terminals)
            {
                IRuleFilter filter = CompileFilters(transformedRule, ruleDeclarations, terminal.FactMap);

                var rightHandSide = transformedRule.RightHandSide;
                var actions = new List<IRuleAction>();
                foreach (var action in rightHandSide.Actions)
                {
                    var ruleAction = ExpressionCompiler.CompileAction(action, ruleDeclarations, dependencies, terminal.FactMap);
                    actions.Add(ruleAction);
                }

                var rule = new CompiledRule(ruleDefinition, ruleDeclarations, actions, filter, terminal.FactMap);
                BuildRuleNode(rule, terminal);
                rules.Add(rule);
            }

            return rules;
        }

        private IRuleFilter CompileFilters(IRuleDefinition ruleDefinition, List<Declaration> ruleDeclarations, IndexMap tupleFactMap)
        {
            var conditions = new List<IActivationExpression<bool>>();
            var keySelectors = new List<IActivationExpression<object>>();
            foreach (var filter in ruleDefinition.FilterGroup.Filters)
            {
                switch (filter.FilterType)
                {
                    case FilterType.Predicate:
                        var condition = ExpressionCompiler.CompileActivationExpression<bool>(filter, ruleDeclarations, tupleFactMap);
                        conditions.Add(condition);
                        break;
                    case FilterType.KeyChange:
                        var keySelector = ExpressionCompiler.CompileActivationExpression<object>(filter, ruleDeclarations, tupleFactMap);
                        keySelectors.Add(keySelector);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unrecognized filter type. FilterType={filter.FilterType}");
                }
            }
            var compiledFilter = new RuleFilter(conditions, keySelectors);
            return compiledFilter;
        }

        private void BuildRuleNode(ICompiledRule compiledRule, ITerminal terminal)
        {
            var ruleNode = new RuleNode(compiledRule);
            terminal.Source.Attach(ruleNode);
        }
    }
}
