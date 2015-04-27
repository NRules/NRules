using System;

namespace NRules
{
    public interface IDependencyResolver
    {
        object Resolve(Type serviceType);
    }

    internal class DependencyResolver : IDependencyResolver
    {
        public object Resolve(Type serviceType)
        {
            throw new InvalidOperationException("Dependency resolver not provided");
        }
    }
}