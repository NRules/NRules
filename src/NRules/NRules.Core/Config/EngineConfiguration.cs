using NRules.Config;
using NRules.Core.Rete;

namespace NRules.Core.Config
{
    public class EngineConfiguration : Configuration
    {
        internal EngineConfiguration(Configuration config) : base(config)
        {
            Container.Configure<RuleRepository>(DependencyLifecycle.SingleInstance);
            Container.Configure<SessionFactory>(DependencyLifecycle.InstancePerCall);
            Container.Configure<ReteBuilder>(DependencyLifecycle.InstancePerCall);
        }

        public IRuleRepository CreateRepository()
        {
            return Container.Build<IRuleRepository>();
        }
    }
}