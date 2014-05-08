using System;
using System.Reflection;
using NRules.Fluent;

namespace NRules.Debug.Visualizer.Tests
{
    public class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var repository = new RuleRepository();
            repository.Load("Test", x => x.From(Assembly.GetExecutingAssembly()));

            ISessionFactory factory = repository.Compile();
            ISession session = factory.CreateSession();

            VisualizerHost.Visualize(session);
        }
    }
}