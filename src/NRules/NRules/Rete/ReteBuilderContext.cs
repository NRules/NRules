using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal class ReteBuilderContext
    {
        private int _factOffset = 0;
        private readonly Dictionary<Declaration, int> _offsetMap;

        public ReteBuilderContext()
        {
            _offsetMap = new Dictionary<Declaration, int>();
        }

        public ReteBuilderContext(ReteBuilderContext context)
        {
            BetaSource = context.BetaSource;
            _factOffset = context._factOffset;
            _offsetMap = new Dictionary<Declaration, int>(context._offsetMap);
        }

        public IObjectSource AlphaSource { get; set; }
        public ITupleSource BetaSource { get; set; }
        public bool HasSubnet { get; set; }

        public void RegisterDeclaration(Declaration declaration)
        {
            _offsetMap[declaration] = _factOffset;
            _factOffset++;
        }

        public IEnumerable<int> GetTupleMask(IEnumerable<Declaration> declarations)
        {
            return declarations.Select(d => _offsetMap[d]);
        }

        public void ResetAlphaSource()
        {
            AlphaSource = null;
            HasSubnet = false;
        }
    }
}