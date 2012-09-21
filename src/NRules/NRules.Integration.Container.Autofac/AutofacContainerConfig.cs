using Autofac;
using NRules.Config;

namespace NRules.Integration.Container.Autofac
{
    public static class AutofacContainerConfig
    {
        public static Configuration AutofacContainer(this Configure config, ILifetimeScope scope)
        {
            return config.Container(new AutofacContainer(scope));
        }

        public static Configuration AutofacContainer(this Configure config)
        {
            return config.Container(new AutofacContainer());
        }
    }
}