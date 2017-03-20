using System.Collections.Generic;
using Moq;
using NRules.Diagnostics;
using NRules.Proxy;
using NRules.Rete;
using NUnit.Framework;

namespace NRules.Tests
{
    [TestFixture]
    public class SessionTest
    {
        private Mock<IAgenda> _agenda;
        private Mock<INetwork> _network;
        private Mock<IWorkingMemory> _workingMemory;
        private Mock<IEventAggregator> _eventAggregator;
        private Mock<IDependencyResolver> _dependencyResolver;
        private Mock<IActionInterceptor> _actionInterceptor;
            
        [SetUp]
        public void Setup()
        {
            _agenda = new Mock<IAgenda>();
            _network = new Mock<INetwork>();
            _workingMemory = new Mock<IWorkingMemory>();
            _eventAggregator = new Mock<IEventAggregator>();
            _dependencyResolver = new Mock<IDependencyResolver>();
            _actionInterceptor = new Mock<IActionInterceptor>();
        }

        [Test]
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

        [Test]
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

        [Test]
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

        private Session CreateTarget()
        {
            return new Session(_network.Object, _agenda.Object, _workingMemory.Object, _eventAggregator.Object, _dependencyResolver.Object, _actionInterceptor.Object);
        }

        private FactResult Succeeded()
        {
            return new FactResult(new List<object>());
        }
    }
}