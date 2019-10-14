using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel
{
    /// <summary>
    /// Base class for rule elements.
    /// </summary>
    public abstract class RuleElement
    {
        private readonly HashSet<Declaration> _exports;
        private readonly HashSet<Declaration> _imports;

        /// <summary>
        /// Element type of this rule element.
        /// </summary>
        public abstract ElementType ElementType { get; }

        /// <summary>
        /// Rule element declarations exported to the outer scope.
        /// </summary>
        public IEnumerable<Declaration> Exports => _exports;

        /// <summary>
        /// Rule element declarations imported from the outer scope.
        /// </summary>
        public IEnumerable<Declaration> Imports => _imports;

        internal RuleElement()
        {
            _exports = new HashSet<Declaration>();
            _imports = new HashSet<Declaration>();
        }

        internal void AddImports(IEnumerable<Declaration> declarations)
        {
            var imports = declarations.Except(_exports);
            _imports.UnionWith(imports);
        }

        internal void AddImports(RuleElement element)
        {
            if (element != null)
                AddImports(new[] {element});
        }

        internal void AddImports(IEnumerable<RuleElement> elements)
        {
            var imports = elements.SelectMany(x => x.Imports);
            AddImports(imports);
        }

        internal void AddExports(IEnumerable<Declaration> declarations)
        {
            _exports.UnionWith(declarations);
        }

        internal void AddExports(IEnumerable<RuleElement> elements)
        {
            var exports = elements.SelectMany(x => x.Exports);
            AddExports(exports);
        }

        internal void AddExport(Declaration declaration)
        {
            AddExports(new[] {declaration});
        }

        internal abstract void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor);
    }
}