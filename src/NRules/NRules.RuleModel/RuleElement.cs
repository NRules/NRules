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

        public IEnumerable<Declaration> Exports => _exports;
        public IEnumerable<Declaration> Imports => _imports;

        internal RuleElement()
        {
            _exports = new HashSet<Declaration>();
            _imports = new HashSet<Declaration>();
        }

        protected void AddImport(Declaration declaration)
        {
            AddImports(new[] {declaration});
        }

        protected void AddImports(IEnumerable<Declaration> declarations)
        {
            var imports = declarations.Except(_exports);
            _imports.UnionWith(imports);
        }

        protected void AddImports(RuleElement element)
        {
            if (element != null)
                AddImports(new[] {element});
        }

        protected void AddImports(IEnumerable<RuleElement> elements)
        {
            var imports = elements.SelectMany(x => x.Imports);
            AddImports(imports);
        }

        protected void AddExport(Declaration declaration)
        {
            AddExports(new[] {declaration});
        }

        protected void AddExports(IEnumerable<Declaration> declarations)
        {
            _exports.UnionWith(declarations);
        }

        protected void AddExports(RuleElement element)
        {
            if (element != null)
                AddExports(new[] {element});
        }

        protected void AddExports(IEnumerable<RuleElement> elements)
        {
            var exports = elements.SelectMany(x => x.Exports);
            AddExports(exports);
        }

        internal abstract void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor);
    }
}