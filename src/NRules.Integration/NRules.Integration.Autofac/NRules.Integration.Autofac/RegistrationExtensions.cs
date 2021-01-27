using System;
using Autofac;
using Autofac.Builder;
using NRules.Extensibility;
using NRules.Fluent;
using NRules.RuleModel;

namespace NRules.Integration.Autofac
{
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Registers fluent rule types with the container, registers <see cref="RuleRepository"/> with the container
        /// and loads registered rules into the repository.
        /// By default repository is registered as a single instance and is wired with a <see cref="IRuleActivator"/>.
        /// </summary>
        /// <param name="builder">Container builder.</param>
        /// <param name="scanAction">Configuration action on the rule type scanner.</param>
        /// <returns>Registration builder for <see cref="RuleRepository"/> to specify additional registration configuration.</returns>
        public static IRegistrationBuilder<RuleRepository, ConcreteReflectionActivatorData, SingleRegistrationStyle> 
            RegisterRuleRepository(this ContainerBuilder builder, Action<IRuleTypeScanner> scanAction)
        {
            var scanner = new RuleTypeScanner();
            scanAction(scanner);
            var ruleTypes = scanner.GetRuleTypes();
            builder.RegisterTypes(ruleTypes)
                .AsSelf()
                .InstancePerDependency();

            builder.RegisterType<AutofacRuleActivator>()
                .As<IRuleActivator>();

           return builder.RegisterType<RuleRepository>()
                .As<IRuleRepository>()
                .SingleInstance()
                .OnActivating(e =>
                {
                    e.Instance.Activator = e.Context.Resolve<IRuleActivator>();
                    e.Instance.Load(s => s.From(scanAction));
                });
        }

        /// <summary>
        /// Registers <see cref="ISessionFactory"/> with the container.
        /// Requires that <see cref="IRuleRepository"/> is registered with the container.
        /// By default session factory is registered as a single instance and is wired with a <see cref="IDependencyResolver"/>.
        /// </summary>
        /// <param name="builder">Container builder.</param>
        /// <returns>Registration builder for <see cref="ISessionFactory"/> to specify additional registration configuration.</returns>
        public static IRegistrationBuilder<ISessionFactory, SimpleActivatorData, SingleRegistrationStyle> 
            RegisterSessionFactory(this ContainerBuilder builder)
        {
            return builder.RegisterSessionFactory(c => c.Resolve<IRuleRepository>().Compile());
        }

        /// <summary>
        /// Registers <see cref="ISessionFactory"/> with the container.
        /// By default session factory is registered as a single instance and is wired with a <see cref="IDependencyResolver"/>.
        /// </summary>
        /// <param name="builder">Container builder.</param>
        /// <param name="compileFunc">Compile function that creates an instance of <see cref="ISessionFactory"/>.</param>
        /// <returns>Registration builder for <see cref="ISessionFactory"/> to specify additional registration configuration.</returns>
        public static IRegistrationBuilder<ISessionFactory, SimpleActivatorData, SingleRegistrationStyle> 
            RegisterSessionFactory(this ContainerBuilder builder, Func<IComponentContext, ISessionFactory> compileFunc)
        {
            builder.RegisterType<AutofacDependencyResolver>()
                .As<IDependencyResolver>();

            return builder.Register(compileFunc)
                .As<ISessionFactory>()
                .SingleInstance()
                .OnActivating(e => e.Instance.DependencyResolver = e.Context.Resolve<IDependencyResolver>());
        }

        /// <summary>
        /// Registers <see cref="ISession"/> with the container.
        /// By default session is registered as an instance per lifetime scope.
        /// </summary>
        /// <param name="builder">Container builder.</param>
        /// <returns>Registration builder for <see cref="ISession"/> to specify additional registration configuration.</returns>
        public static IRegistrationBuilder<ISession, SimpleActivatorData, SingleRegistrationStyle> 
            RegisterSession(this ContainerBuilder builder)
        {
            return builder.RegisterSession(c => c.Resolve<ISessionFactory>().CreateSession());
        }

        /// <summary>
        /// Registers <see cref="ISession"/> with the container.
        /// By default session is registered as an instance per lifetime scope.
        /// </summary>
        /// <param name="builder">Container builder.</param>
        /// <param name="factoryFunc">Factory function that creates an instance of <see cref="ISession"/>.</param>
        /// <returns>Registration builder for <see cref="ISession"/> to specify additional registration configuration.</returns>
        public static IRegistrationBuilder<ISession, SimpleActivatorData, SingleRegistrationStyle> 
            RegisterSession(this ContainerBuilder builder, Func<IComponentContext, ISession> factoryFunc)
        {
            return builder.Register(factoryFunc)
                .As<ISession>()
                .InstancePerLifetimeScope();
        }
    }
}