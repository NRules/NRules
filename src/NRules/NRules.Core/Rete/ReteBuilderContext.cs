using System.Collections.Generic;
using NRules.Rule;

namespace NRules.Core.Rete
{
    internal class ReteBuilderContext
    {
        public ReteBuilderContext()
        {
            Declarations = new List<Declaration>();
        }

        public ReteBuilderContext(ReteBuilderContext context)
        {
            BetaSource = context.BetaSource;
            Declarations = new List<Declaration>(context.Declarations);
        }

        public List<Declaration> Declarations { get; private set; } 
        public IObjectSource AlphaSource { get; set; }
        public ITupleSource BetaSource { get; set; }
    }
}