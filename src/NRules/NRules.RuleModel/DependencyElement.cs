using System;

namespace NRules.RuleModel
{
    /// <summary>
    /// Dependency that the rule uses when its actions run.
    /// </summary>
    public class DependencyElement : RuleElement
    {
        internal DependencyElement(Declaration declaration, Type serviceType)
        {
            Declaration = declaration;
            ServiceType = serviceType;

            AddExport(declaration);
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