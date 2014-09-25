using System;
using System.Reflection;
using NRules.Debugger.Visualizer.Tests.TestAssets;
using NRules.Fluent;

namespace NRules.Debugger.Visualizer.Tests
{
    public class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var repository = new RuleRepository();
            repository.Load(x => x.From(Assembly.GetExecutingAssembly()));

            ISessionFactory factory = repository.Compile();
            ISession session = factory.CreateSession();

            session.Insert(new Fact1 { Value = "TestValue" });

            VisualizerHost.Visualize(session);
        }
    }
}