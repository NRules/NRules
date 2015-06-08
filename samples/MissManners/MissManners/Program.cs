using System;
using System.Linq;
using NRules.Diagnostics;
using NRules.Fluent;
using NRules.Samples.MissManners.Domain;
using NRules.Samples.MissManners.Rules;

namespace NRules.Samples.MissManners
{
    public class Program
    {
        static void Main(string[] args)
        {
            var repository = new RuleRepository();
            repository.Load(x => x.From(typeof(AssignFirstSeat).Assembly));

            var factory = repository.Compile();
            var session = factory.CreateSession();
            session.Events.FactInsertingEvent += EventProviderOnFactInsertingEvent;
            session.Events.FactUpdatingEvent += EventProviderOnFactUpdatingEvent;
            session.Events.FactRetractingEvent += EventProviderOnFactRetractingEvent;
            session.Events.ActivationCreatedEvent += EventProviderOnActivationCreatedEvent;
            session.Events.ActivationUpdatedEvent += EventProviderOnActivationUpdatedEvent;
            session.Events.ActivationDeletedEvent += EventProviderOnActivationDeletedEvent;
            session.Events.RuleFiringEvent += EventProviderOnRuleFiringEvent;

            session.Insert(new Guest("N1", Gender.Male, new Hobby("H1")));
            session.Insert(new Guest("N2", Gender.Female, new Hobby("H1")));
            session.Insert(new Guest("N2", Gender.Female, new Hobby("H3")));
            session.Insert(new Guest("N3", Gender.Male, new Hobby("H3")));
            session.Insert(new Guest("N4", Gender.Male, new Hobby("H1")));
            session.Insert(new Guest("N4", Gender.Female, new Hobby("H2")));
            session.Insert(new Guest("N4", Gender.Female, new Hobby("H3")));
            session.Insert(new Guest("N5", Gender.Female, new Hobby("H2")));
            session.Insert(new Guest("N5", Gender.Female, new Hobby("H1")));
            session.Insert(new Count(1));
            session.Insert(new LastSeat(5));

            var context = new Context();
            session.Insert(context);

            session.Fire();
        }

        private static void EventProviderOnFactInsertingEvent(object sender, WorkingMemoryEventArgs e)
        {
            Console.WriteLine("Insert: {0}", e.Fact.Value);
        }

        private static void EventProviderOnFactUpdatingEvent(object sender, WorkingMemoryEventArgs e)
        {
            Console.WriteLine("Update: {0}", e.Fact.Value);
        }

        private static void EventProviderOnFactRetractingEvent(object sender, WorkingMemoryEventArgs e)
        {
            Console.WriteLine("Retract: {0}", e.Fact.Value);
        }

        private static void EventProviderOnRuleFiringEvent(object sender, AgendaEventArgs e)
        {
            Console.WriteLine("Fire({0}): {1}", e.Rule.Name, string.Join(",", e.Facts.Select(f => f.Value).ToArray()));
        }

        private static void EventProviderOnActivationDeletedEvent(object sender, AgendaEventArgs e)
        {
            Console.WriteLine("-A({0}): {1}", e.Rule.Name, string.Join(",", e.Facts.Select(f => f.Value).ToArray()));
        }

        private static void EventProviderOnActivationUpdatedEvent(object sender, AgendaEventArgs e)
        {
            Console.WriteLine("=A({0}): {1}", e.Rule.Name, string.Join(",", e.Facts.Select(f => f.Value).ToArray()));
        }
        
        private static void EventProviderOnActivationCreatedEvent(object sender, AgendaEventArgs e)
        {
            Console.WriteLine("+A({0}): {1}", e.Rule.Name, string.Join(",", e.Facts.Select(f => f.Value).ToArray()));
        }
    }
}
