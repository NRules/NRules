using System;
using System.Collections.Generic;
using System.Threading;
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
        public void InsertAll_SomeFactsExist_Throws()
        {
            // Arrange
            var facts = new[] { new object(), new object() };
            var factWrappers = new Dictionary<object, Fact>
            {
                {facts[0], new Fact(facts[0])},
                {facts[1], null}
            };
            var target = CreateTarget();
            _workingMemory.Setup(x => x.GetFact(It.IsAny<object>())).Returns<object>(x => factWrappers[x]);

            // Act - Assert
            Assert.Throws<ArgumentException>(() => target.InsertAll(facts));
        }

        [Fact]
        public void TryInsertAll_SomeFactsExistOptionsAllOrNothing_DoesNotPropagateAssert()
        {
            // Arrange
            var facts = new[] { new object(), new object() };
            var factWrappers = new Dictionary<object, Fact>
            {
                {facts[0], new Fact(facts[0])},
                {facts[1], null}
            };
            var target = CreateTarget();
            _workingMemory.Setup(x => x.GetFact(It.IsAny<object>())).Returns<object>(x => factWrappers[x]);

            // Act
            var result = target.TryInsertAll(facts, BatchOptions.AllOrNothing);

            // Assert
            Assert.Equal(1, result.FailedCount);
            _network.Verify(x => x.PropagateAssert(
                    It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object),
                    It.IsAny<List<Fact>>()),
                Times.Never());
        }

        [Fact]
        public void TryInsertAll_SomeFactsExistOptionsSkipFailed_PropagatesAssertForNonFailed()
        {
            // Arrange
            var facts = new[] { new object(), new object() };
            var factWrappers = new Dictionary<object, Fact>
            {
                {facts[0], new Fact(facts[0])},
                {facts[1], null}
            };
            var target = CreateTarget();
            _workingMemory.Setup(x => x.GetFact(It.IsAny<object>())).Returns<object>(x => factWrappers[x]);

            // Act
            var result = target.TryInsertAll(facts, BatchOptions.SkipFailed);

            // Assert
            Assert.Equal(1, result.FailedCount);
            _network.Verify(x => x.PropagateAssert(
                    It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object),
                    It.Is<List<Fact>>(p => p.Count == 1 && p[0].Object == facts[1])),
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
                    It.Is<List<Fact>>(p => p.Count == 1 && p[0].Object == fact)),
                Times.Exactly(1));
        }

        [Fact]
        public void UpdateAll_FactsExist_PropagatesUpdate()
        {
            // Arrange
            var facts = new[] { new object(), new object() };
            var factWrappers = new Dictionary<object, Fact>
            {
                {facts[0], new Fact(facts[0])},
                {facts[1], new Fact(facts[1])}
            };
            var target = CreateTarget();
            _workingMemory.Setup(x => x.GetFact(It.IsAny<object>())).Returns<object>(x => factWrappers[x]);

            // Act
            target.UpdateAll(facts);

            // Assert
            _network.Verify(x => x.PropagateUpdate(
                    It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object),
                    It.Is<List<Fact>>(p => p.Count == 2 && p[0].Object == facts[0] && p[1].Object == facts[1])),
                Times.Exactly(1));
        }

        [Fact]
        public void UpdateAll_SomeFactsDoNotExist_Throws()
        {
            // Arrange
            var facts = new[] {new object(), new object()};
            var factWrappers = new Dictionary<object, Fact>
            {
                {facts[0], new Fact(facts[0])},
                {facts[1], null}
            };
            var target = CreateTarget();
            _workingMemory.Setup(x => x.GetFact(It.IsAny<object>())).Returns<object>(x => factWrappers[x]);

            // Act - Assert
            Assert.Throws<ArgumentException>(() => target.UpdateAll(facts));
        }

        [Fact]
        public void TryUpdateAll_SomeFactsDoNotExistOptionsAllOrNothing_DoesNotPropagateUpdate()
        {
            // Arrange
            var facts = new[] { new object(), new object() };
            var factWrappers = new Dictionary<object, Fact>
            {
                {facts[0], new Fact(facts[0])},
                {facts[1], null}
            };
            var target = CreateTarget();
            _workingMemory.Setup(x => x.GetFact(It.IsAny<object>())).Returns<object>(x => factWrappers[x]);

            // Act
            var result = target.TryUpdateAll(facts, BatchOptions.AllOrNothing);

            // Assert
            Assert.Equal(1, result.FailedCount);
            _network.Verify(x => x.PropagateUpdate(
                    It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object),
                    It.IsAny<List<Fact>>()),
                Times.Never());
        }

        [Fact]
        public void TryUpdateAll_SomeFactsDoNotExistOptionsSkipFailed_PropagateUpdateNonFailed()
        {
            // Arrange
            var facts = new[] { new object(), new object() };
            var factWrappers = new Dictionary<object, Fact>
            {
                {facts[0], new Fact(facts[0])},
                {facts[1], null}
            };
            var target = CreateTarget();
            _workingMemory.Setup(x => x.GetFact(It.IsAny<object>())).Returns<object>(x => factWrappers[x]);

            // Act
            var result = target.TryUpdateAll(facts, BatchOptions.SkipFailed);

            // Assert
            Assert.Equal(1, result.FailedCount);
            _network.Verify(x => x.PropagateUpdate(
                    It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object),
                    It.Is<List<Fact>>(p => p.Count == 1 && p[0].Object == facts[0])),
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
        public void RetractAll_FactsExist_PropagatesRetract()
        {
            // Arrange
            var facts = new[] { new object(), new object() };
            var factWrappers = new Dictionary<object, Fact>
            {
                {facts[0], new Fact(facts[0])},
                {facts[1], new Fact(facts[1])}
            };
            var target = CreateTarget();
            _workingMemory.Setup(x => x.GetFact(It.IsAny<object>())).Returns<object>(x => factWrappers[x]);

            // Act
            target.RetractAll(facts);

            // Assert
            _network.Verify(x => x.PropagateRetract(
                    It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object),
                    It.Is<List<Fact>>(p => p.Count == 2 && p[0].Object == facts[0] && p[1].Object == facts[1])),
                Times.Exactly(1));
        }

        [Fact]
        public void RetractAll_SomeFactsDoNotExist_Throws()
        {
            // Arrange
            var facts = new[] { new object(), new object() };
            var factWrappers = new Dictionary<object, Fact>
            {
                {facts[0], new Fact(facts[0])},
                {facts[1], null}
            };
            var target = CreateTarget();
            _workingMemory.Setup(x => x.GetFact(It.IsAny<object>())).Returns<object>(x => factWrappers[x]);

            // Act - Assert
            Assert.Throws<ArgumentException>(() => target.RetractAll(facts));
        }


        [Fact]
        public void TryRetractAll_SomeFactsDoNotExistOptionsAllOrNothing_DoesNotPropagateRetract()
        {
            // Arrange
            var facts = new[] { new object(), new object() };
            var factWrappers = new Dictionary<object, Fact>
            {
                {facts[0], new Fact(facts[0])},
                {facts[1], null}
            };
            var target = CreateTarget();
            _workingMemory.Setup(x => x.GetFact(It.IsAny<object>())).Returns<object>(x => factWrappers[x]);

            // Act
            var result = target.TryRetractAll(facts, BatchOptions.AllOrNothing);

            // Assert
            Assert.Equal(1, result.FailedCount);
            _network.Verify(x => x.PropagateRetract(
                    It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object),
                    It.IsAny<List<Fact>>()),
                Times.Never());
        }

        [Fact]
        public void TryRetractAll_SomeFactsDoNotExistOptionsSkipFailed_PropagateRetractNonFailed()
        {
            // Arrange
            var facts = new[] { new object(), new object() };
            var factWrappers = new Dictionary<object, Fact>
            {
                {facts[0], new Fact(facts[0])},
                {facts[1], null}
            };
            var target = CreateTarget();
            _workingMemory.Setup(x => x.GetFact(It.IsAny<object>())).Returns<object>(x => factWrappers[x]);

            // Act
            var result = target.TryRetractAll(facts, BatchOptions.SkipFailed);

            // Assert
            Assert.Equal(1, result.FailedCount);
            _network.Verify(x => x.PropagateRetract(
                    It.Is<IExecutionContext>(p => p.WorkingMemory == _workingMemory.Object),
                    It.Is<List<Fact>>(p => p.Count == 1 && p[0].Object == facts[0])),
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

        [Fact]
        public void Fire_CancellationRequested()
        {
            using (var cancellationSource = new CancellationTokenSource())
            {
                // Arrange
                var hitCount = 0;
                var target = CreateTarget();
                _agenda.Setup(x => x.Pop()).Returns(StubActivation());
                _agenda
                    .Setup(x => x.IsEmpty)
                    .Returns(() =>
                    {
                        if (++hitCount == 2)
                        {
                            cancellationSource.Cancel();
                        }

                        return hitCount < 5 ? false : true;
                    });

                // Act
                var actual = target.Fire(cancellationSource.Token);

                // Assert
                Assert.Equal(2, actual);
                _actionExecutor.Verify(ae => ae.Execute(It.IsAny<IExecutionContext>(), It.Is<IActionContext>(ac => ac.CancellationToken == cancellationSource.Token)));
            }
        }

        [Fact]
        public void Fire_CancellationRequested_WithMaxRules()
        {
            using (var cancellationSource = new CancellationTokenSource())
            {
                // Arrange
                var hitCount = 0;
                var target = CreateTarget();
                _agenda.Setup(x => x.Pop()).Returns(StubActivation());
                _agenda
                    .Setup(x => x.IsEmpty)
                    .Returns(() =>
                    {
                        if (++hitCount == 2)
                        {
                            cancellationSource.Cancel();
                        }

                        return hitCount < 5 ? false : true;
                    });

                // Act
                var actual = target.Fire(5, cancellationSource.Token);

                // Assert
                Assert.Equal(2, actual);
                _actionExecutor.Verify(ae => ae.Execute(It.IsAny<IExecutionContext>(), It.Is<IActionContext>(ac => ac.CancellationToken == cancellationSource.Token)));
            }
        }

        [Fact]
        public void Fire_PassesCancellationTokenToActionContext()
        {
            using (var cancellationSource = new CancellationTokenSource())
            {
                // Arrange
                var target = CreateTarget();
                _agenda.Setup(x => x.Pop()).Returns(StubActivation());
                _agenda.SetupSequence(x => x.IsEmpty)
                    .Returns(false).Returns(true);

                // Act
                var actual = target.Fire(cancellationSource.Token);

                // Assert
                Assert.Equal(1, actual);
                _actionExecutor.Verify(ae => ae.Execute(It.IsAny<IExecutionContext>(), It.Is<IActionContext>(ac => ac.CancellationToken == cancellationSource.Token)));
            }
        }

        [Fact]
        public void Fire_PassesCancellationTokenToActionContext_WithMaxRules()
        {
            using (var cancellationSource = new CancellationTokenSource())
            {
                // Arrange
                var target = CreateTarget();
                _agenda.Setup(x => x.Pop()).Returns(StubActivation());
                _agenda.SetupSequence(x => x.IsEmpty)
                    .Returns(false).Returns(true);

                // Act
                var actual = target.Fire(2, cancellationSource.Token);

                // Assert
                Assert.Equal(1, actual);
                _actionExecutor.Verify(ae => ae.Execute(It.IsAny<IExecutionContext>(), It.Is<IActionContext>(ac => ac.CancellationToken == cancellationSource.Token)));
            }
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
            var activation = new Activation(rule.Object, null);
            return activation;
        }
    }
}