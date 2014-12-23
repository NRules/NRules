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
                    var compiledRules = CompileRule(reteBuilder, ruleDefinition);
                    rules.AddRange(compiledRules);
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

        private IEnumerable<ICompiledRule> CompileRule(ReteBuilder reteBuilder, IRuleDefinition ruleDefinition)
        {
            var transformation = new RuleTransformation();
            var transformedRule = transformation.Transform(ruleDefinition);

            var rules = new List<ICompiledRule>();
            IEnumerable<RuleTerminal> terminals = reteBuilder.AddRule(transformedRule);
            foreach (var terminal in terminals)
            {
                var rightHandSide = transformedRule.RightHandSide;
                var actions = new List<IRuleAction>();
                foreach (var action in rightHandSide.Actions)
                {
                    var mask = terminal.GetTupleMask(action.Declarations);
                    var ruleAction = new RuleAction(action.Expression, mask);
                    actions.Add(ruleAction);
                }

                var rule = new CompiledRule(ruleDefinition, actions);
                rules.Add(rule);
                BuildRuleNode(rule, terminal.TerminalNode);
            }

            return rules;
        }

        private void BuildRuleNode(ICompiledRule rule, ITerminalNode terminalNode)
        {
            var ruleNode = new RuleNode(rule);
            terminalNode.Attach(ruleNode);
        }
    }
}