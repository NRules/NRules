using System;
using System.Collections.Generic;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Builder to compose a group of rule dependencies.
    /// </summary>
    public class DependencyGroupBuilder : RuleElementBuilder, IBuilder<DependencyGroupElement>
    {
        private readonly List<DependencyElement> _dependencies = new List<DependencyElement>();

        internal DependencyGroupBuilder(SymbolTable scope)
            : base(scope)
        {
        }

        /// <summary>
        /// Adds a dependency to the group.
        /// </summary>
        /// <param name="type">Dependency .NET type.</param>
        /// <param name="name">Dependency name.</param>
        public void Dependency(Type type, string name)
        {
            Declaration declaration = Scope.Declare(type, name);
            var dependency = new DependencyElement(declaration, Scope.Declarations, type);
            declaration.Target = dependency;
            _dependencies.Add(dependency);
        }

        DependencyGroupElement IBuilder<DependencyGroupElement>.Build()
        {
            var actionGroup = new DependencyGroupElement(Scope.VisibleDeclarations, _dependencies);
            return actionGroup;
        }
    }
}
