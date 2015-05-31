using System.Linq;
using NRules.Diagnostics;
using NRules.Fluent;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class LifecycleEventTest
    {
        [Test]
        public void Insert_Fact_RaisesFactInsertingEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var fact = new FactType1 { TestProperty = "Valid Value" };

            object factorySender = null;
            WorkingMemoryEventArgs factoryArgs = null;
            object sessionSender = null;
            WorkingMemoryEventArgs sessionArgs = null;
            factory.Events.FactInsertingEvent += (sender, args) =>
            {
                factorySender = sender;
                factoryArgs = args;
            };
            session.Events.FactInsertingEvent += (sender, args) =>
            {
                sessionSender = sender;
                sessionArgs = args;
            };

            //Act
            session.Insert(fact);

            //Assert
            Assert.AreSame(session, factorySender);
            Assert.AreSame(session, sessionSender);
            Assert.AreSame(fact, factoryArgs.Fact.Value);
            Assert.AreSame(fact, sessionArgs.Fact.Value);
        }

        [Test]
        public void Insert_Fact_RaisesFactInsertedEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var fact = new FactType1 { TestProperty = "Valid Value" };

            object factorySender = null;
            WorkingMemoryEventArgs factoryArgs = null;
            object sessionSender = null;
            WorkingMemoryEventArgs sessionArgs = null;
            factory.Events.FactInsertedEvent += (sender, args) =>
            {
                factorySender = sender;
                factoryArgs = args;
            };
            session.Events.FactInsertedEvent += (sender, args) =>
            {
                sessionSender = sender;
                sessionArgs = args;
            };

            //Act
            session.Insert(fact);

            //Assert
            Assert.AreSame(session, factorySender);
            Assert.AreSame(session, sessionSender);
            Assert.AreSame(fact, factoryArgs.Fact.Value);
            Assert.AreSame(fact, sessionArgs.Fact.Value);
        }

        [Test]
        public void Update_Fact_RaisesFactUpdatingEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var fact = new FactType1 { TestProperty = "Valid Value" };
            session.Insert(fact);

            object factorySender = null;
            WorkingMemoryEventArgs factoryArgs = null;
            object sessionSender = null;
            WorkingMemoryEventArgs sessionArgs = null;
            factory.Events.FactUpdatingEvent += (sender, args) =>
            {
                factorySender = sender;
                factoryArgs = args;
            };
            session.Events.FactUpdatingEvent += (sender, args) =>
            {
                sessionSender = sender;
                sessionArgs = args;
            };

            //Act
            session.Update(fact);

            //Assert
            Assert.AreSame(session, factorySender);
            Assert.AreSame(session, sessionSender);
            Assert.AreSame(fact, factoryArgs.Fact.Value);
            Assert.AreSame(fact, sessionArgs.Fact.Value);
        }

        [Test]
        public void Update_Fact_RaisesFactUpdatedEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var fact = new FactType1 { TestProperty = "Valid Value" };
            session.Insert(fact);

            object factorySender = null;
            WorkingMemoryEventArgs factoryArgs = null;
            object sessionSender = null;
            WorkingMemoryEventArgs sessionArgs = null;
            factory.Events.FactUpdatedEvent += (sender, args) =>
            {
                factorySender = sender;
                factoryArgs = args;
            };
            session.Events.FactUpdatedEvent += (sender, args) =>
            {
                sessionSender = sender;
                sessionArgs = args;
            };

            //Act
            session.Update(fact);

            //Assert
            Assert.AreSame(session, factorySender);
            Assert.AreSame(session, sessionSender);
            Assert.AreSame(fact, factoryArgs.Fact.Value);
            Assert.AreSame(fact, sessionArgs.Fact.Value);
        }

        [Test]
        public void Retract_Fact_RaisesFactRetractingEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var fact = new FactType1 { TestProperty = "Valid Value" };
            session.Insert(fact);

            object factorySender = null;
            WorkingMemoryEventArgs factoryArgs = null;
            object sessionSender = null;
            WorkingMemoryEventArgs sessionArgs = null;
            factory.Events.FactRetractingEvent += (sender, args) =>
            {
                factorySender = sender;
                factoryArgs = args;
            };
            session.Events.FactRetractingEvent += (sender, args) =>
            {
                sessionSender = sender;
                sessionArgs = args;
            };

            //Act
            session.Retract(fact);

            //Assert
            Assert.AreSame(session, factorySender);
            Assert.AreSame(session, sessionSender);
            Assert.AreSame(fact, factoryArgs.Fact.Value);
            Assert.AreSame(fact, sessionArgs.Fact.Value);
        }

        [Test]
        public void Retract_Fact_RaisesFactRetractedEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var fact = new FactType1 { TestProperty = "Valid Value" };
            session.Insert(fact);

            object factorySender = null;
            WorkingMemoryEventArgs factoryArgs = null;
            object sessionSender = null;
            WorkingMemoryEventArgs sessionArgs = null;
            factory.Events.FactRetractedEvent += (sender, args) =>
            {
                factorySender = sender;
                factoryArgs = args;
            };
            session.Events.FactRetractedEvent += (sender, args) =>
            {
                sessionSender = sender;
                sessionArgs = args;
            };

            //Act
            session.Retract(fact);

            //Assert
            Assert.AreSame(session, factorySender);
            Assert.AreSame(session, sessionSender);
            Assert.AreSame(fact, factoryArgs.Fact.Value);
            Assert.AreSame(fact, sessionArgs.Fact.Value);
        }

        [Test]
        public void Insert_RuleActivated_RaisesActivationCreatedEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var fact = new FactType1 { TestProperty = "Valid Value" };

            object factorySender = null;
            AgendaEventArgs factoryArgs = null;
            object sessionSender = null;
            AgendaEventArgs sessionArgs = null;
            factory.Events.ActivationCreatedEvent += (sender, args) =>
            {
                factorySender = sender;
                factoryArgs = args;
            };
            session.Events.ActivationCreatedEvent += (sender, args) =>
            {
                sessionSender = sender;
                sessionArgs = args;
            };

            //Act
            session.Insert(fact);

            //Assert
            Assert.AreSame(session, factorySender);
            Assert.AreSame(session, sessionSender);
            Assert.AreSame(fact, factoryArgs.Facts.Single().Value);
            Assert.AreSame(fact, sessionArgs.Facts.Single().Value);
            Assert.That(factoryArgs.Rule.Name.Contains("OneFactRule"));
            Assert.That(sessionArgs.Rule.Name.Contains("OneFactRule"));
        }

        [Test]
        public void Update_RuleReactivated_RaisesActivationUpatedEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var fact = new FactType1 { TestProperty = "Valid Value" };
            session.Insert(fact);

            object factorySender = null;
            AgendaEventArgs factoryArgs = null;
            object sessionSender = null;
            AgendaEventArgs sessionArgs = null;
            factory.Events.ActivationUpdatedEvent += (sender, args) =>
            {
                factorySender = sender;
                factoryArgs = args;
            };
            session.Events.ActivationUpdatedEvent += (sender, args) =>
            {
                sessionSender = sender;
                sessionArgs = args;
            };

            //Act
            session.Update(fact);

            //Assert
            Assert.AreSame(session, factorySender);
            Assert.AreSame(session, sessionSender);
            Assert.AreSame(fact, factoryArgs.Facts.Single().Value);
            Assert.AreSame(fact, sessionArgs.Facts.Single().Value);
            Assert.That(factoryArgs.Rule.Name.Contains("OneFactRule"));
            Assert.That(sessionArgs.Rule.Name.Contains("OneFactRule"));
        }

        [Test]
        public void Retract_RuleDeactivated_RaisesActivationDeletedEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var fact = new FactType1 { TestProperty = "Valid Value" };
            session.Insert(fact);

            object factorySender = null;
            AgendaEventArgs factoryArgs = null;
            object sessionSender = null;
            AgendaEventArgs sessionArgs = null;
            factory.Events.ActivationDeletedEvent += (sender, args) =>
            {
                factorySender = sender;
                factoryArgs = args;
            };
            session.Events.ActivationDeletedEvent += (sender, args) =>
            {
                sessionSender = sender;
                sessionArgs = args;
            };

            //Act
            session.Retract(fact);

            //Assert
            Assert.AreSame(session, factorySender);
            Assert.AreSame(session, sessionSender);
            Assert.AreSame(fact, factoryArgs.Facts.Single().Value);
            Assert.AreSame(fact, sessionArgs.Facts.Single().Value);
            Assert.That(factoryArgs.Rule.Name.Contains("OneFactRule"));
            Assert.That(sessionArgs.Rule.Name.Contains("OneFactRule"));
        }

        [Test]
        public void Fire_RuleFires_RaisesRuleFiringEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var fact = new FactType1 { TestProperty = "Valid Value" };
            session.Insert(fact);

            object factorySender = null;
            AgendaEventArgs factoryArgs = null;
            object sessionSender = null;
            AgendaEventArgs sessionArgs = null;
            factory.Events.RuleFiringEvent += (sender, args) =>
            {
                factorySender = sender;
                factoryArgs = args;
            };
            session.Events.RuleFiringEvent += (sender, args) =>
            {
                sessionSender = sender;
                sessionArgs = args;
            };

            //Act
            session.Fire();

            //Assert
            Assert.AreSame(session, factorySender);
            Assert.AreSame(session, sessionSender);
            Assert.AreSame(fact, factoryArgs.Facts.Single().Value);
            Assert.AreSame(fact, sessionArgs.Facts.Single().Value);
            Assert.That(factoryArgs.Rule.Name.Contains("OneFactRule"));
            Assert.That(sessionArgs.Rule.Name.Contains("OneFactRule"));
        }

        [Test]
        public void Fire_RuleFires_RaisesRuleFiredEvent()
        {
            //Arrange
            var factory = CreateTarget();
            var session = factory.CreateSession();

            var fact = new FactType1 { TestProperty = "Valid Value" };
            session.Insert(fact);

            object factorySender = null;
            AgendaEventArgs factoryArgs = null;
            object sessionSender = null;
            AgendaEventArgs sessionArgs = null;
            factory.Events.RuleFiredEvent += (sender, args) =>
            {
                factorySender = sender;
                factoryArgs = args;
            };
            session.Events.RuleFiredEvent += (sender, args) =>
            {
                sessionSender = sender;
                sessionArgs = args;
            };

            //Act
            session.Fire();

            //Assert
            Assert.AreSame(session, factorySender);
            Assert.AreSame(session, sessionSender);
            Assert.AreSame(fact, factoryArgs.Facts.Single().Value);
            Assert.AreSame(fact, sessionArgs.Facts.Single().Value);
            Assert.That(factoryArgs.Rule.Name.Contains("OneFactRule"));
            Assert.That(sessionArgs.Rule.Name.Contains("OneFactRule"));
        }

        private ISessionFactory CreateTarget()
        {
            var repository = new RuleRepository();
            repository.Load(x => x.From(typeof(OneFactRule)));
            return repository.Compile();
        }
    }
}