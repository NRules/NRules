using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using NRules.Config;
using IContainer = NRules.Config.IContainer;

namespace NRules.Integration.Container.Autofac
{
    internal class AutofacContainer : IContainer
    {
        private readonly ILifetimeScope _container;

        public AutofacContainer() : this(null)
        {
        }

        public AutofacContainer(ILifetimeScope container)
        {
            _container = container;
            if (container == null)
            {
                var builder = new ContainerBuilder();
                _container = builder.Build();
            }
        }

        public object Build(Type typeToBuild)
        {
            return _container.Resolve(typeToBuild);
        }

        public IEnumerable<object> BuildAll(Type typeToBuild)
        {
            return _container.Resolve(typeof (IEnumerable<>).MakeGenericType(typeToBuild)) as IEnumerable<object>;
        }

        public void Configure(Type component, DependencyLifecycle lifecycle)
        {
            var registration = GetComponentRegistration(component);

            if (registration != null)
                return;

            var builder = new ContainerBuilder();
            var registrationBuilder =
                builder.RegisterType(component).AsImplementedInterfaces().AsSelf().PropertiesAutowired();

            switch (lifecycle)
            {
                case DependencyLifecycle.InstancePerCall:
                    registrationBuilder.InstancePerDependency();
                    break;
                case DependencyLifecycle.SingleInstance:
                    registrationBuilder.SingleInstance();
                    break;
                default:
                    throw new ArgumentException(string.Format("Invalid component lifecycle. Lifecycle={0}",
                                                              lifecycle));
            }

            builder.Update(_container.ComponentRegistry);
        }

        public void Register(Type component, object instance)
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(instance).As(component).ExternallyOwned().PropertiesAutowired();
            builder.Update(_container.ComponentRegistry);
        }

        public void ConfigureProperty(Type component, string property, object value)
        {
            var registration = GetComponentRegistration(component);

            if (registration == null)
            {
                throw new InvalidOperationException(string.Format(
                    "Failed to configue component property. Confgure component first. Component={0}", component.FullName));
            }

            registration.Activating += (sender, e) => SetPropertyValue(e.Instance, property, value);
        }

        private static void SetPropertyValue(object instance, string propertyName, object value)
        {
            var property = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            property.SetValue(instance, value, null);
        }

        private IComponentRegistration GetComponentRegistration(Type concreteComponent)
        {
            return
                _container.ComponentRegistry.Registrations.FirstOrDefault(
                    x => x.Activator.LimitType == concreteComponent);
        }
    }
}