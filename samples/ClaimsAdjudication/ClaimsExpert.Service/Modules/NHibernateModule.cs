using System.Configuration;
using Autofac;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NRules.Samples.ClaimsExpert.Domain;
using NRules.Samples.ClaimsExpert.Domain.Modules;

namespace NRules.Samples.ClaimsExpert.Service.Modules
{
    public class NHibernateModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => CreateSessionFactory())
                .As<NHibernate.ISessionFactory>().SingleInstance()
                .AutoActivate();
            builder.Register(c => c.Resolve<NHibernate.ISessionFactory>().OpenSession())
                .As<NHibernate.ISession>().InstancePerDependency();
        }

        private NHibernate.ISessionFactory CreateSessionFactory()
        {
            var databaseFile = ConfigurationManager.AppSettings["databaseFile"];
            var configuration = Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.UsingFile(databaseFile))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<DomainModule>());
            var sessionFactory = configuration.BuildSessionFactory();
            return sessionFactory;
        }
    }
}