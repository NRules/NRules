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

        public IEnumerable<int> GetTupleMask(IEnumerable<Declaration> declarations)
        {
            foreach (var declaration in declarations)
            {
                int selectionIndex = Declarations.FindIndex(0, d => Equals(declaration, d));
                yield return selectionIndex;
            }
        }
    }
}