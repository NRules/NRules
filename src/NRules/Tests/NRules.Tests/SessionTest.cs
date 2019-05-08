using System.Collections.Generic;
using System.Linq;
using Moq;
using NRules.Diagnostics;
using NRules.Extensibility;
using NRules.Rete;
using Xunit;

namespace NRules.Tests
{
    public class SessionTest
    {
        private readonly Mock<IAgendaInternal> _agenda;
        private readonly Mock<INetwork> _network;
        private readonly Mock<IWorkingMemory> _workingMemory;
        private readonly Mock<IEventAggregator> _eventAggregator;
        private readonly Mock<IActionExecutor> _actionExecutor;
        private readonly Mock<IIdGenerator> _idGenerator;
        private readonly Mock<IDependencyResolver> _dependencyResolver;
        private readonly Mock<IActionInterceptor> _actionInterceptor;
            
        public SessionTest()
        {
            _agenda = new Mock<IAgendaInternal>();
            _network = new Mock<INetwork>();
            _workingMemory = new Mock<IWorkingMemory>();
            _eventAggregator = new Mock<IEventAggregator>();
            _actionExecutor = new Mock<IActionExecutor>();
            _idGenerator = new Mock<IIdGenerator>();
            _dependencyResolver = new Mock<IDependencyResolver>();
            _actionInterceptor = new Mock<IActionInterceptor>();
        }

        [Fact]
        public void Insert_FactDoesNotExist_PropagatesAssert()
        {
            // Arrange
            var fact = new object();
            var target = CreateTarget();
            _workingMemory.Setup(x => x.GetFact(fact)).Returns(() => null);

            // Act
            target.Insert(fact);

            // Assert
            _network.Verify(x => x.PropagateAssert(
                    It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object),
                    It.Is<List<Fact>>(p => p.Count == 1 && p[0].Object == fact)),
                Times.Exactly(1));
        }

        [Fact]
        public void InsertAll_FactsDoNotExist_PropagatesAssert()
        {
            // Arrange
            var facts = new[] { new object(), new object() };
            var target = CreateTarget();
            _workingMemory.Setup(x => x.GetFact(It.IsAny<object>())).Returns(() => null);

            // Act
            target.InsertAll(facts);

            // Assert
            _network.Verify(x => x.PropagateAssert(
                    It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object),
                    It.Is<List<Fact>>(p => p.Count == 2 && p[0].Object == facts[0] && p[1].Object == facts[1])),
                Times.Exactly(1));
        }

        [Fact]
        public void Update_FactExists_PropagatesUpdate()
        {
            // Arrange
            var fact = new object();
            var factWrapper = new Fact(fact);
            var target = CreateTarget();
            _workingMemory.Setup(x => x.GetFact(fact)).Returns(factWrapper);

            // Act
            target.Update(fact);

            // Assert
            _network.Verify(x => x.PropagateUpdate(
                    It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object),
                    It.Is<List<Fact>>(p => p.Count == 1 && p[0] == factWrapper)),
                Times.Exactly(1));
        }

        [Fact]
        public void UpdateAll_Called_PropagatesUpdate()
        {
            // Arrange
            var facts = new[] {new object(), new object()};
            var factWrappers = new[] {new Fact(facts[0]), new Fact(facts[1])};
            var target = CreateTarget();
            var factLookup = factWrappers.ToDictionary(x => x.Object, x => x);
            _workingMemory.Setup(x => x.GetFact(It.IsAny<object>())).Returns<object>(x => factLookup[x]);

            // Act
            target.UpdateAll(facts);

            // Assert
            _network.Verify(x => x.PropagateUpdate(
                    It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object),
                    It.Is<List<Fact>>(p => p.Count == 2 && p[0] == factWrappers[0] && p[1] == factWrappers[1])),
                Times.Exactly(1));
        }

        [Fact]
        public void Retract_FactExists_PropagatesRetract()
        {
            // Arrange
            var fact = new object();
            var factWrapper = new Fact(fact);
            var target = CreateTarget();
            _workingMemory.Setup(x => x.GetFact(fact)).Returns(factWrapper);

            // Act
            target.Retract(fact);

            // Assert
            _network.Verify(x => x.PropagateRetract(
                    It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object),
                    It.Is<List<Fact>>(p => p.Count == 1 && p[0] == factWrapper)),
                Times.Exactly(1));
        }

        [Fact]
        public void RetractAll_Called_PropagatesRetract()
        {
            // Arrange
            var facts = new[] { new object(), new object() };
            var factWrappers = new[] { new Fact(facts[0]), new Fact(facts[1]) };
            var target = CreateTarget();
            var factLookup = factWrappers.ToDictionary(x => x.Object, x => x);
            _workingMemory.Setup(x => x.GetFact(It.IsAny<object>())).Returns<object>(x => factLookup[x]);

            // Act
            target.RetractAll(facts);

            // Assert
            _network.Verify(x => x.PropagateRetract(
                    It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object),
                    It.Is<List<Fact>>(p => p.Count == 2 && p[0] == factWrappers[0] && p[1] == factWrappers[1])),
                Times.Exactly(1));
        }

        [Fact]
        public void Fire_NoActiveRules_ReturnsZero()
        {
            // Arrange
            var target = CreateTarget();
            _agenda.Setup(x => x.IsEmpty).Returns(true);

            // Act
            var actual = target.Fire();

            // Assert
            Assert.Equal(0, actual);
        }

        [Fact]
        public void Fire_ActiveRules_ReturnsNumberOfRulesFired()
        {
            // Arrange
            var target = CreateTarget();
            _agenda.Setup(x => x.Pop()).Returns(StubActivation());
            _agenda.SetupSequence(x => x.IsEmpty)
                .Returns(false).Returns(false).Returns(true);

            // Act
            var actual = target.Fire();

            // Assert
            Assert.Equal(2, actual);
        }

        [Fact]
        public void Fire_ActiveRulesMoreThanMax_FiresMaxRules()
        {
            // Arrange
            var target = CreateTarget();
            _agenda.Setup(x => x.Pop()).Returns(StubActivation());
            _agenda.SetupSequence(x => x.IsEmpty)
                .Returns(false).Returns(false).Returns(true);

            // Act
            var actual = target.Fire(1);

            // Assert
            Assert.Equal(1, actual);
        }

        private Session CreateTarget()
        {
            var session = new Session(_network.Object, _agenda.Object, _workingMemory.Object, _eventAggregator.Object, 
                _actionExecutor.Object, _idGenerator.Object, _dependencyResolver.Object, _actionInterceptor.Object);
            return session;
        }

        private static Activation StubActivation()
        {
            var rule = new Mock<ICompiledRule>();
            rule.Setup(x => x.Actions).Returns(new IRuleAction[0]);
            var activation = new Activation(rule.Object, null, null);
            return activation;
        }
    }
}