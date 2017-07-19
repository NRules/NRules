using System;
using NRules.Extensibility;
using NRules.RuleModel;

namespace NRules
{
    internal interface IRuleDependency
    {
        Declaration Declaration { get; }
        Func<IDependencyResolver, IResolutionContext, object> Factory { get; }
    }

    internal class RuleDependency : IRuleDependency
    {
        public RuleDependency(Declaration declaration, Type serviceType)
        {
            Declaration = declaration;
            Factory = (r, c) => r.Resolve(c, serviceType);
        }

        public Declaration Declaration { get; }
        public Func<IDependencyResolver, IResolutionContext, object> Factory { get; }
    }
}