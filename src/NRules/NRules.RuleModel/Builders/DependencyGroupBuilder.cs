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

        internal DependencyGroupBuilder()
        {
        }

        /// <summary>
        /// Adds a dependency to the group.
        /// </summary>
        /// <param name="type">Dependency CLR type.</param>
        /// <param name="name">Dependency name.</param>
        /// <returns>Dependency declaration.</returns>
        public Declaration Dependency(Type type, string name)
        {
            var declaration = new Declaration(type, DeclarationName(name));
            var dependency = new DependencyElement(declaration, type);
            declaration.Target = dependency;
            _dependencies.Add(dependency);
            return declaration;
        }

        DependencyGroupElement IBuilder<DependencyGroupElement>.Build()
        {
            var dependencyGroup = new DependencyGroupElement(_dependencies);
            Validate(dependencyGroup);
            return dependencyGroup;
        }

        private static void Validate(DependencyGroupElement element)
        {
            ValidationHelper.AssertUniqueDeclarations(element.Dependencies);
        }
    }
}
