using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules
{
    /// <summary>
    /// Compiles declarative rules into an executable representation.
    /// </summary>
    public interface IRuleCompiler
    {
        /// <summary>
        /// Compiles a collection of rules into a session factory.
        /// </summary>
        /// <param name="ruleDefinitions">Rules to compile.</param>
        /// <returns>Session factory.</returns>
        ISessionFactory Compile(IEnumerable<IRuleDefinition> ruleDefinitions);

        /// <summary>
        /// Compiles rules from rule sets into a session factory.
        /// </summary>
        /// <param name="ruleSets">Rule sets to compile.</param>
        /// <returns>Session factory.</returns>
        ISessionFactory Compile(IEnumerable<IRuleSet> ruleSets);
    }

    public class RuleCompiler : IRuleCompiler
    {
        public ISessionFactory Compile(IEnumerable<IRuleSet> ruleSets)
        {
            var rules = ruleSets.SelectMany(x => x.Rules);
            return Compile(rules);
        }

        public ISessionFactory Compile(IEnumerable<IRuleDefinition> ruleDefinitions)
        {
            var rules = new List<ICompiledRule>();
            var reteBuilder = new ReteBuilder();
            foreach (var ruleDefinition in ruleDefinitions)
            {
                try
                {
                    ICompiledRule compiledRule = CompileRule(reteBuilder, ruleDefinition);
                    rules.Add(compiledRule);
                }
                catch (Exception e)
                {
                    throw new RuleCompilationException(
                        string.Format("Failed to compile rule. Rule={0}",
                            ruleDefinition.Name), ruleDefinition, e);
                }
            }

            INetwork network = reteBuilder.Build();
            var factory = new SessionFactory(network);
            return factory;
        }

        private ICompiledRule CompileRule(ReteBuilder reteBuilder, IRuleDefinition ruleDefinition)
        {
            var context = new ReteBuilderContext();
            ITerminalNode terminalNode = reteBuilder.AddRule(context, ruleDefinition);

            var rightHandSide = ruleDefinition.RightHandSide;
            var actions = new List<IRuleAction>();
            foreach (var action in rightHandSide.Actions)
            {
                var mask = context.GetTupleMask(action.Declarations);
                var ruleAction = new RuleAction(action.Expression, mask);
                actions.Add(ruleAction);
            }

            var rule = new CompiledRule(ruleDefinition, actions);
            BuildRuleNode(rule, terminalNode);
            return rule;
        }

        private void BuildRuleNode(ICompiledRule rule, ITerminalNode terminalNode)
        {
            var ruleNode = new RuleNode(rule);
            terminalNode.Attach(ruleNode);
        }
    }
}