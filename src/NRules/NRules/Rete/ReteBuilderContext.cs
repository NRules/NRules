using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Rete
{
    internal class ReteBuilderContext
    {
        private readonly List<Declaration> _declarationOrder;

        public ReteBuilderContext()
        {
            _declarationOrder = new List<Declaration>();
        }

        public ReteBuilderContext(ReteBuilderContext context)
        {
            BetaSource = context.BetaSource;
            _declarationOrder = new List<Declaration>(context._declarationOrder);
        }

        public AlphaNode CurrentAlphaNode { get; set; }
        public IAlphaMemoryNode AlphaSource { get; set; }
        public IBetaMemoryNode BetaSource { get; set; }
        public bool HasSubnet { get; set; }

        public void RegisterDeclaration(Declaration declaration)
        {
            _declarationOrder.Add(declaration);
        }

        public TupleMask GetTupleMask(IEnumerable<Declaration> declarations)
        {
            var positionMap = declarations.ToIndexMap();
            var mask = _declarationOrder
                .Select(x => positionMap.IndexOrDefault(x, -1)).ToArray();
            return new TupleMask(mask);
        }

        public void ResetAlphaSource()
        {
            AlphaSource = null;
            HasSubnet = false;
        }
    }
}