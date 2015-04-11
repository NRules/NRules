using System;
using System.Diagnostics;
using System.Globalization;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using NRules.Samples.ClaimsCenter.Applications.Controllers;
using NRules.Samples.ClaimsCenter.Applications.Modules;
using NRules.Samples.ClaimsCenter.Presentation.Modules;

namespace NRules.Samples.ClaimsCenter.Presentation
{
    public partial class App
    {
        public IContainer Container { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += AppDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterAssemblyModules(typeof(ApplicationModule).Assembly);
            containerBuilder.RegisterAssemblyModules(typeof(PresentationModule).Assembly);

            Container = containerBuilder.Build();
            var controller = Container.Resolve<IApplicationController>();
            controller.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Container.Dispose();
            base.OnExit(e);
        }

        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception, false);
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception, e.IsTerminating);
        }

        private static void HandleException(Exception e, bool isTerminating)
        {
            if (e == null) return;

            Trace.TraceError(e.ToString());

            if (!isTerminating)
            {
                MessageBox.Show(string.Format(CultureInfo.CurrentCulture, "Unknown error: {0}", e), 
                    ApplicationInfo.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
