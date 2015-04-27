using System;
using NRules.RuleModel;

namespace NRules
{
    internal interface IRuleDependency
    {
        Declaration Declaration { get; }
        Func<IDependencyResolver, object> Factory { get; }
    }

    internal class RuleDependency : IRuleDependency
    {
        private readonly Declaration _declaration;
        private readonly Type _serviceType;
        private readonly Func<IDependencyResolver, object> _factory;

        public RuleDependency(Declaration declaration, Type serviceType)
        {
            _declaration = declaration;
            _serviceType = serviceType;
            _factory = x => x.Resolve(_serviceType);
        }

        public Declaration Declaration { get { return _declaration; } }
        public Func<IDependencyResolver, object> Factory { get { return _factory; } }
    }
}