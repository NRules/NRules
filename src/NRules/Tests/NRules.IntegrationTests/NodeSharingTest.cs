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
        public void Fire_AlphaSelectionNodes_OnePerIntaCondition()
        {
            //Arrange
            var snapshotProvider = (ISessionSnapshotProvider) Session;
            var snapshot = snapshotProvider.GetSnapshot();

            //Act
            var alphaNodesCount = snapshot.Nodes.Count(x => x.NodeType == NodeType.Selection);

            //Assert
            Assert.AreEqual(2, alphaNodesCount);
        }

        [Test]
        public void Fire_BetaJoinNodes_OnePerPattern()
        {
            //Arrange
            var snapshotProvider = (ISessionSnapshotProvider) Session;
            var snapshot = snapshotProvider.GetSnapshot();

            //Act
            var betaJoinNodesCount = snapshot.Nodes.Count(x => x.NodeType == NodeType.Join);

            //Assert
            Assert.AreEqual(2, betaJoinNodesCount);
        }

        protected override void SetUpRules()
        {
            SetUpRule<TwinRuleOne>();
            SetUpRule<TwinRuleTwo>();
        }
    }
}