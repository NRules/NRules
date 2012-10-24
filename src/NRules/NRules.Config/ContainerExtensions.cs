using System.Collections.Generic;
using System.Linq;

namespace NRules.Config
{
    public static class ContainerExtensions
    {
        public static T Build<T>(this IContainer container)
        {
            return (T) container.Build(typeof (T));
        }

        public static IEnumerable<T> BuildAll<T>(this IContainer container)
        {
            return container.BuildAll(typeof (T)).Cast<T>();
        }

        public static ComponentConfig<T> Configure<T>(this IContainer container, DependencyLifecycle lifecycle)
        {
            container.Configure(typeof (T), lifecycle);
            return new ComponentConfig<T>(container);
        }

        public static void Register<T>(this IContainer container, object instance)
        {
            container.Register(typeof (T), instance);
        }
    }
}