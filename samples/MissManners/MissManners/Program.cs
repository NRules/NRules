using System;
using System.Linq;
using MissManners.Domain;
using MissManners.Rules;
using NRules;
using NRules.Events;
using NRules.Fluent;

namespace MissManners
{
    public class Program
    {
        static void Main(string[] args)
        {
            var repository = new RuleRepository();
            repository.Load("Test", x => x.From(typeof(AssignFirstSeat).Assembly));

            var factory = repository.Compile();
            var session = factory.CreateSession();
            session.EventProvider.FactInsertedEvent += EventProviderOnFactInsertedEvent;
            session.EventProvider.FactUpdatedEvent += EventProviderOnFactUpdatedEvent;
            session.EventProvider.FactRetractedEvent += EventProviderOnFactRetractedEvent;
            session.EventProvider.ActivationCreatedEvent += EventProviderOnActivationCreatedEvent;
            session.EventProvider.ActivationDeletedEvent += EventProviderOnActivationDeletedEvent;
            session.EventProvider.BeforeRuleFiredEvent += EventProviderOnBeforeRuleFiredEvent;

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

        private static void EventProviderOnFactInsertedEvent(object sender, WorkingMemoryEventArgs e)
        {
            Console.WriteLine("Insert: {0}", e.Fact);
        }

        private static void EventProviderOnFactUpdatedEvent(object sender, WorkingMemoryEventArgs e)
        {
            Console.WriteLine("Update: {0}", e.Fact);
        }

        private static void EventProviderOnFactRetractedEvent(object sender, WorkingMemoryEventArgs e)
        {
            Console.WriteLine("Retract: {0}", e.Fact);
        }

        private static void EventProviderOnBeforeRuleFiredEvent(object sender, AgendaEventArgs e)
        {
            Console.WriteLine("Fire({0}): {1}", e.Rule.Name, string.Join(",", e.Facts.ToArray()));
        }

        private static void EventProviderOnActivationDeletedEvent(object sender, AgendaEventArgs e)
        {
            Console.WriteLine("-A({0}): {1}", e.Rule.Name, string.Join(",", e.Facts.ToArray()));
        }

        private static void EventProviderOnActivationCreatedEvent(object sender, AgendaEventArgs e)
        {
            Console.WriteLine("+A({0}): {1}", e.Rule.Name, string.Join(",", e.Facts.ToArray()));
        }
    }
}
