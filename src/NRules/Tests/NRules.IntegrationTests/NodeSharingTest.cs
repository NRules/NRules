using System.Linq;
using NRules.Diagnostics;
using NRules.IntegrationTests.TestAssets;
using NRules.IntegrationTests.TestRules;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class NodeSharingTest : BaseRuleTestFixture
    {
        [Test]
        public void Fire_AlphaSelectionNodes_OnePerIntraCondition()
        {
            //Arrange
            var snapshotProvider = (ISessionSnapshotProvider) Session;
            var snapshot = snapshotProvider.GetSnapshot();

            //Act
            var count = snapshot.Nodes.Count(x => x.NodeType == NodeType.Selection);

            //Assert
            Assert.AreEqual(5, count);
        }

        [Test]
        public void Fire_BetaJoinNodes_OnePerPattern()
        {
            //Arrange
            var snapshotProvider = (ISessionSnapshotProvider) Session;
            var snapshot = snapshotProvider.GetSnapshot();

            //Act
            var count = snapshot.Nodes.Count(x => x.NodeType == NodeType.Join);

            //Assert
            Assert.AreEqual(4, count);
        }

        [Test]
        public void Fire_AggregateNodes_CorrectCount()
        {
            //Arrange
            var snapshotProvider = (ISessionSnapshotProvider) Session;
            var snapshot = snapshotProvider.GetSnapshot();

            //Act
            var count = snapshot.Nodes.Count(x => x.NodeType == NodeType.Aggregate);

            //Assert
            Assert.AreEqual(3, count);
        }

        [Test]
        public void Fire_NotNodes_CorrectCount()
        {
            //Arrange
            var snapshotProvider = (ISessionSnapshotProvider) Session;
            var snapshot = snapshotProvider.GetSnapshot();

            //Act
            var count = snapshot.Nodes.Count(x => x.NodeType == NodeType.Not);

            //Assert
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Fire_ExistsNodes_CorrectCount()
        {
            //Arrange
            var snapshotProvider = (ISessionSnapshotProvider) Session;
            var snapshot = snapshotProvider.GetSnapshot();

            //Act
            var count = snapshot.Nodes.Count(x => x.NodeType == NodeType.Exists);

            //Assert
            Assert.AreEqual(1, count);
        }

        protected override void SetUpRules()
        {
            SetUpRule<TwinRuleOne>();
            SetUpRule<TwinRuleTwo>();
        }
    }
}