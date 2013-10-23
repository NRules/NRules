using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rete;
using NRules.Rule;

namespace NRules.Core
{
    public interface IRuleCompiler
    {
        ISessionFactory Compile(IEnumerable<IRuleDefinition> ruleDefinitions);
    }

    internal class RuleCompiler : IRuleCompiler
    {
        public ISessionFactory Compile(IEnumerable<IRuleDefinition> ruleDefinitions)
        {
            var rules = new List<ICompiledRule>();
            var reteBuilder = new ReteBuilder();
            foreach (var ruleDefinition in ruleDefinitions)
            {
                var context = new ReteBuilderContext();
                ITerminalNode terminalNode = reteBuilder.AddRule(context, ruleDefinition);
                ICompiledRule compiledRule = CompileRule(context, ruleDefinition);
                BuildRuleNode(compiledRule, terminalNode);
                rules.Add(compiledRule);
            }

            INetwork network = reteBuilder.GetNetwork();
            var factory = new SessionFactory(rules, network);
            return factory;
        }

        private void BuildRuleNode(ICompiledRule rule, ITerminalNode terminalNode)
        {
            var ruleNode = new RuleNode(rule.Handle, rule.Definition.Priority);
            terminalNode.Attach(ruleNode);
        }

        private ICompiledRule CompileRule(ReteBuilderContext context, IRuleDefinition ruleDefinition)
        {
            var rightHandSide = ruleDefinition.RightHandSide;
            var actions = new List<IRuleAction>();
            foreach (var action in rightHandSide.Actions)
            {
                var mask = context.GetTupleMask(action.Declarations);
                var ruleAction = new RuleAction(action.Expression, mask.ToArray());
                actions.Add(ruleAction);
            }
            var rule = new CompiledRule(ruleDefinition, actions);
            return rule;
        }
    }
}