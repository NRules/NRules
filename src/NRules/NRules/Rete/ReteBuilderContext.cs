using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal class ReteBuilderContext
    {
        private readonly List<Declaration> _declarations;

        public ReteBuilderContext(IRuleDefinition rule, DummyNode dummyNode)
        {
            Rule = rule;
            _declarations = new List<Declaration>();
            BetaSource = dummyNode;
        }

        public ReteBuilderContext(ReteBuilderContext context)
        {
            Rule = context.Rule;
            BetaSource = context.BetaSource;
            _declarations = new List<Declaration>(context._declarations);
        }

        public IRuleDefinition Rule { get; }
        public IEnumerable<Declaration> Declarations => _declarations;

        public AlphaNode CurrentAlphaNode { get; set; }
        public IAlphaMemoryNode AlphaSource { get; set; }
        public IBetaMemoryNode BetaSource { get; set; }
        public bool HasSubnet { get; set; }

        public void RegisterDeclaration(Declaration declaration)
        {
            _declarations.Add(declaration);
        }

        public void ResetAlphaSource()
        {
            AlphaSource = null;
            HasSubnet = false;
        }
    }
}