using System;

namespace NRules.Extensibility
{
    /// <summary>
    /// Defines a mechanism to resolve rule dependencies at runtime.
    /// An instance of <c>IDependencyResolver</c> can be assigned to <see cref="ISessionFactory.DependencyResolver"/> or
    /// <see cref="ISession.DependencyResolver"/>, so that all requests for rule dependencies resolution are fulfiled by that resolver.
    /// </summary>
    /// <remarks>If dependency resolver is not configured, any attempt to resolve rule dependencies will result in exception.</remarks>
    public interface IDependencyResolver
    {
        /// <summary>
        /// Resolves a registered service (normally via an IoC container).
        /// </summary>
        /// <param name="context">Information about the context at which the resolution call is made.</param>
        /// <param name="serviceType">The type of requested service.</param>
        /// <returns>Requested service.</returns>
        object Resolve(IResolutionContext context, Type serviceType);
    }

    internal class DependencyResolver : IDependencyResolver
    {
        public object Resolve(IResolutionContext context, Type serviceType)
        {
            const string message = "Dependency resolver not provided. " 
                + "To use rule dependencies set a dependency resolver on the rules session.";
            throw new InvalidOperationException(message);
        }
    }
}