using Moq;
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

        [SetUp]
        public void Setup()
        {
            _agenda = new Mock<IAgenda>();
            _network = new Mock<INetwork>();
            _workingMemory = new Mock<IWorkingMemory>();
        }

        [Test]
        public void Insert_Always_PropagatesAssert()
        {
            // Arrange
            var fact = new object();
            var target = CreateTarget();

            // Act
            target.Insert(fact);

            // Assert
            _network.Verify(x => x.PropagateAssert(_workingMemory.Object, fact), Times.Exactly(1));
        }

        private Session CreateTarget()
        {
            return new Session(_network.Object, _agenda.Object, _workingMemory.Object);
        }
    }
}