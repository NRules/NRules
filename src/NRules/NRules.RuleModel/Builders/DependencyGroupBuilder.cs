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

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyGroupBuilder"/>.
        /// </summary>
        public DependencyGroupBuilder()
        {
        }

        /// <summary>
        /// Adds a dependency element to the group element.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public void Dependency(DependencyElement element)
        {
            _dependencies.Add(element);
        }

        /// <summary>
        /// Adds a dependency to the group element.
        /// </summary>
        /// <param name="type">Dependency CLR type.</param>
        /// <param name="name">Dependency name.</param>
        /// <returns>Dependency declaration.</returns>
        public Declaration Dependency(Type type, string name)
        {
            var dependency = Element.Dependency(type, DeclarationName(name));
            _dependencies.Add(dependency);
            return dependency.Declaration;
        }

        DependencyGroupElement IBuilder<DependencyGroupElement>.Build()
        {
            var dependencyGroup = Element.DependencyGroup(_dependencies);
            return dependencyGroup;
        }
    }
}
