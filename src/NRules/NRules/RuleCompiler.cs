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
        /// <seealso cref="IRuleRepository"/>
        public ISessionFactory Compile(IEnumerable<IRuleDefinition> ruleDefinitions)
        {
            var reteBuilder = new ReteBuilder();
            foreach (var ruleDefinition in ruleDefinitions)
            {
                try
                {
                    CompileRule(reteBuilder, ruleDefinition);
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

        private void CompileRule(ReteBuilder reteBuilder, IRuleDefinition ruleDefinition)
        {
            var transformation = new RuleTransformation();
            var transformedRule = transformation.Transform(ruleDefinition);
            var ruleDeclarations = transformedRule.LeftHandSide.Declarations.ToList();
            var ruleDependencies = transformedRule.DependencyGroup.Dependencies.Select(x => x.Declaration).ToList();

            IRulePriority priority = CompilePriority(ruleDefinition.Priority, ruleDeclarations, ruleDependencies);
            IEnumerable<IRuleAction> actions = CompileActions(transformedRule, ruleDeclarations, ruleDependencies);
            IEnumerable<IRuleDependency> dependencies = CompileDependencies(transformedRule);

            var rule = new CompiledRule(ruleDefinition, priority, actions, dependencies);

            IEnumerable<ITerminalNode> terminals = reteBuilder.AddRule(transformedRule);
            BuildRuleNode(rule, terminals);
        }

        private static IRulePriority CompilePriority(PriorityElement priority, List<Declaration> ruleDeclarations, List<Declaration> ruleDependencies)
        {
            var factIndexMap = IndexMap.CreateMap(priority.References, ruleDeclarations);
            var dependencyIndexMap = IndexMap.CreateMap(priority.References, ruleDependencies);
            var rulePriority = new RulePriority(priority.Expression, factIndexMap, dependencyIndexMap);
            return rulePriority;
        }

        private static IEnumerable<IRuleAction> CompileActions(IRuleDefinition transformedRule, List<Declaration> ruleDeclarations, List<Declaration> ruleDependencies)
        {
            var rightHandSide = transformedRule.RightHandSide;
            foreach (var action in rightHandSide.Actions)
            {
                var factIndexMap = IndexMap.CreateMap(action.References, ruleDeclarations);
                var dependencyIndexMap = IndexMap.CreateMap(action.References, ruleDependencies);
                var ruleAction = new RuleAction(action.Expression, factIndexMap, dependencyIndexMap);
                yield return ruleAction;
            }
        }

        private static IEnumerable<IRuleDependency> CompileDependencies(IRuleDefinition ruleDefinition)
        {
            foreach (var dependency in ruleDefinition.DependencyGroup.Dependencies)
            {
                var compiledDependency = new RuleDependency(dependency.Declaration, dependency.ServiceType);
                yield return compiledDependency;
            }
        }

        private static void BuildRuleNode(ICompiledRule rule, IEnumerable<ITerminalNode> terminalNodes)
        {
            var ruleNode = new RuleNode(rule);
            foreach (var terminalNode in terminalNodes)
            {
                terminalNode.Attach(ruleNode);
            }
        }
    }
}