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
            IReteBuilder reteBuilder = new ReteBuilder();
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

        private void CompileRule(IReteBuilder reteBuilder, IRuleDefinition ruleDefinition)
        {
            var transformation = new RuleTransformation();
            var transformedRule = transformation.Transform(ruleDefinition);
            var ruleDeclarations = transformedRule.LeftHandSide.Declarations.ToList();
            var ruleDependencies = transformedRule.DependencyGroup.Dependencies.Select(x => x.Declaration).ToList();

            IEnumerable<IRuleDependency> dependencies = CompileDependencies(transformedRule);
            IEnumerable<ITerminalNode> terminals = reteBuilder.AddRule(transformedRule);

            var rightHandSide = transformedRule.RightHandSide;
            var actions = new List<IRuleAction>();
            foreach (var action in rightHandSide.Actions)
            {
                var factIndexMap = IndexMap.CreateMap(action.References, ruleDeclarations);
                var dependencyIndexMap = IndexMap.CreateMap(action.References, ruleDependencies);
                var ruleAction = new RuleAction(action.Expression, factIndexMap, dependencyIndexMap);
                actions.Add(ruleAction);
            }

            var rule = new CompiledRule(ruleDefinition, ruleDeclarations, actions, dependencies);
            BuildRuleNode(rule, terminals);
        }

        private IEnumerable<IRuleDependency> CompileDependencies(IRuleDefinition ruleDefinition)
        {
            foreach (var dependency in ruleDefinition.DependencyGroup.Dependencies)
            {
                var compiledDependency = new RuleDependency(dependency.Declaration, dependency.ServiceType);
                yield return compiledDependency;
            }
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
