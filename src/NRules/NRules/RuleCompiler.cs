using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Rete;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules
{
    /// <summary>
    /// Compiles rules in a canonical rule model form into an executable representation.
    /// </summary>
    public class RuleCompiler
    {
        /// <summary>
        /// Compiles a collection of rules into a session factory.
        /// </summary>
        /// <param name="ruleDefinitions">Rules to compile.</param>
        /// <returns>Session factory.</returns>
        /// <exception cref="RuleCompilationException">Any fatal error during rules compilation.</exception>
        public ISessionFactory Compile(IEnumerable<IRuleDefinition> ruleDefinitions)
        {
            var rules = new List<ICompiledRule>();
            var reteBuilder = new ReteBuilder();
            foreach (var ruleDefinition in ruleDefinitions)
            {
                try
                {
                    var compiledRule = CompileRule(reteBuilder, ruleDefinition);
                    rules.Add(compiledRule);
                }
                catch (Exception e)
                {
                    throw new RuleCompilationException("Failed to compile rule", ruleDefinition.Name, e);
                }
            }

            INetwork network = reteBuilder.Build();
            var factory = new SessionFactory(network);
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

        private ICompiledRule CompileRule(ReteBuilder reteBuilder, IRuleDefinition ruleDefinition)
        {
            var transformation = new RuleTransformation();
            var transformedRule = transformation.Transform(ruleDefinition);
            var ruleDeclarations = transformedRule.LeftHandSide.Declarations.ToList();

            IEnumerable<ITerminalNode> terminals = reteBuilder.AddRule(transformedRule);
            var rightHandSide = transformedRule.RightHandSide;
            var actions = new List<IRuleAction>();
            foreach (var action in rightHandSide.Actions)
            {
                var factIndexMap = FactIndexMap.CreateMap(action.Declarations, ruleDeclarations);
                var ruleAction = new RuleAction(action.Expression, factIndexMap);
                actions.Add(ruleAction);
            }

            var rule = new CompiledRule(ruleDefinition, actions);
            BuildRuleNode(rule, terminals);
            return rule;
        }

        private void BuildRuleNode(ICompiledRule rule, IEnumerable<ITerminalNode> terminalNodes)
        {
            var ruleNode = new RuleNode(rule);
            foreach (var terminalNode in terminalNodes)
            {
                terminalNode.Attach(ruleNode);
            }
        }
    }
}