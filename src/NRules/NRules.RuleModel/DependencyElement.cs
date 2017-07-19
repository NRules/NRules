using System;
using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Dependency that the rule uses when its actions runs.
    /// </summary>
    public class DependencyElement : RuleElement
    {
        internal DependencyElement(Declaration declaration, IEnumerable<Declaration> declarations, Type serviceType)
            : base(declarations)
        {
            Declaration = declaration;
            ServiceType = serviceType;
        }

        /// <summary>
        /// Declaration that references the dependency.
        /// </summary>
        public Declaration Declaration { get; }

        /// <summary>
        /// Type of service that this dependency configures.
        /// </summary>
        public Type ServiceType { get; }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitDependency(context, this);
        }
    }
}