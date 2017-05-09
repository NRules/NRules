using System.Collections.Generic;
using Moq;
using NRules.Diagnostics;
using NRules.Extensibility;
using NRules.Rete;
using Xunit;

namespace NRules.Tests
{
    public class SessionTest
    {
        private Mock<IAgendaInternal> _agenda;
        private Mock<INetwork> _network;
        private Mock<IWorkingMemory> _workingMemory;
        private Mock<IEventAggregator> _eventAggregator;
        private Mock<IActionExecutor> _actionExecutor;
        private Mock<IDependencyResolver> _dependencyResolver;
        private Mock<IActionInterceptor> _actionInterceptor;
            
        public SessionTest()
        {
            _agenda = new Mock<IAgendaInternal>();
            _network = new Mock<INetwork>();
            _workingMemory = new Mock<IWorkingMemory>();
            _eventAggregator = new Mock<IEventAggregator>();
            _actionExecutor = new Mock<IActionExecutor>();
            _dependencyResolver = new Mock<IDependencyResolver>();
            _actionInterceptor = new Mock<IActionInterceptor>();
        }

        [Fact]
        public void Insert_Called_PropagatesAssert()
        {
            // Arrange
            var fact = new object();
            var target = CreateTarget();
            _network.Setup(x => x.PropagateAssert(It.IsAny<IExecutionContext>(), new [] {fact})).Returns(Succeeded());

            // Act
            target.Insert(fact);

            // Assert
            _network.Verify(x => x.PropagateAssert(It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object), new[] { fact }), Times.Exactly(1));
        }

        [Fact]
        public void InsertAll_Called_PropagatesAssert()
        {
            // Arrange
            var facts = new[] {new object(), new object()};
            var target = CreateTarget();
            _network.Setup(x => x.PropagateAssert(It.IsAny<IExecutionContext>(), facts)).Returns(Succeeded());

            // Act
            target.InsertAll(facts);

            // Assert
            _network.Verify(x => x.PropagateAssert(It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object), facts), Times.Exactly(1));
        }

        [Fact]
        public void Update_Called_PropagatesUpdate()
        {
            // Arrange
            var fact = new object();
            var target = CreateTarget();
            _network.Setup(x => x.PropagateUpdate(It.IsAny<IExecutionContext>(), new[] { fact })).Returns(Succeeded());

            // Act
            target.Update(fact);

            // Assert
            _network.Verify(x => x.PropagateUpdate(It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object), new[] { fact }), Times.Exactly(1));
        }

        [Fact]
        public void UpdateAll_Called_PropagatesUpdate()
        {
            // Arrange
            var facts = new[] {new object(), new object()};
            var target = CreateTarget();
            _network.Setup(x => x.PropagateUpdate(It.IsAny<IExecutionContext>(), facts)).Returns(Succeeded());

            // Act
            target.UpdateAll(facts);

            // Assert
            _network.Verify(x => x.PropagateUpdate(It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object), facts), Times.Exactly(1));
        }

        [Fact]
        public void Retract_Called_PropagatesRetract()
        {
            // Arrange
            var fact = new object();
            var target = CreateTarget();
            _network.Setup(x => x.PropagateRetract(It.IsAny<IExecutionContext>(), new[] { fact })).Returns(Succeeded());

            // Act
            target.Retract(fact);

            // Assert
            _network.Verify(x => x.PropagateRetract(It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object), new[] { fact }), Times.Exactly(1));
        }

        [Fact]
        public void RetractAll_Called_PropagatesRetract()
        {
            // Arrange
            var facts = new[] {new object(), new object()};
            var target = CreateTarget();
            _network.Setup(x => x.PropagateRetract(It.IsAny<IExecutionContext>(), facts)).Returns(Succeeded());

            // Act
            target.RetractAll(facts);

            // Assert
            _network.Verify(x => x.PropagateRetract(It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object), facts), Times.Exactly(1));
        }

        [Fact]
        public void Fire_NoActiveRules_ReturnsZero()
        {
            // Arrange
            var target = CreateTarget();
            _agenda.Setup(x => x.IsEmpty()).Returns(true);

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
            _agenda.SetupSequence(x => x.IsEmpty())
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
            _agenda.SetupSequence(x => x.IsEmpty())
                .Returns(false).Returns(false).Returns(true);

            // Act
            var actual = target.Fire(1);

            // Assert
            Assert.Equal(1, actual);
        }

        private Session CreateTarget()
        {
            return new Session(_network.Object, _agenda.Object, _workingMemory.Object, _eventAggregator.Object, _actionExecutor.Object, _dependencyResolver.Object, _actionInterceptor.Object);
        }

        private FactResult Succeeded()
        {
            return new FactResult(new List<object>());
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