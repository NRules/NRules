using System;
using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Dependency that the rule uses when its actions runs.
    /// </summary>
    public class DependencyElement : RuleElement
    {
        private readonly Declaration _declaration;
        private readonly Type _serviceType;

        internal DependencyElement(Declaration declaration, IEnumerable<Declaration> declarations, Type serviceType)
            : base(declarations)
        {
            _declaration = declaration;
            _serviceType = serviceType;
        }

        /// <summary>
        /// Declaration that references the dependency.
        /// </summary>
        public Declaration Declaration
        {
            get { return _declaration; }
        }

        /// <summary>
        /// Type of service that this dependency configures.
        /// </summary>
        public Type ServiceType
        {
            get { return _serviceType; }
        }

        internal override void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor)
        {
            visitor.VisitDependency(context, this);
        }
    }
}