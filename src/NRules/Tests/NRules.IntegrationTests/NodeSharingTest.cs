using System.Collections.Generic;
using System.Linq;
using NRules.Diagnostics;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class NodeSharingTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_AlphaSelectionNodes_OnePerIntraCondition()
        {
            //Arrange
            var snapshotProvider = (ISessionSnapshotProvider) Session;
            var snapshot = snapshotProvider.GetSnapshot();

            //Act
            var count = snapshot.Nodes.Count(x => x.NodeType == NodeType.Selection);

            //Assert
            Assert.Equal(5, count);
        }

        [Fact]
        public void Fire_BetaJoinNodes_OnePerPattern()
        {
            //Arrange
            var snapshotProvider = (ISessionSnapshotProvider) Session;
            var snapshot = snapshotProvider.GetSnapshot();

            //Act
            var count = snapshot.Nodes.Count(x => x.NodeType == NodeType.Join);

            //Assert
            Assert.Equal(4, count);
        }

        [Fact]
        public void Fire_AggregateNodes_CorrectCount()
        {
            //Arrange
            var snapshotProvider = (ISessionSnapshotProvider) Session;
            var snapshot = snapshotProvider.GetSnapshot();

            //Act
            var count = snapshot.Nodes.Count(x => x.NodeType == NodeType.Aggregate);

            //Assert
            Assert.Equal(3, count);
        }

        [Fact]
        public void Fire_NotNodes_CorrectCount()
        {
            //Arrange
            var snapshotProvider = (ISessionSnapshotProvider) Session;
            var snapshot = snapshotProvider.GetSnapshot();

            //Act
            var count = snapshot.Nodes.Count(x => x.NodeType == NodeType.Not);

            //Assert
            Assert.Equal(1, count);
        }

        [Fact]
        public void Fire_ExistsNodes_CorrectCount()
        {
            //Arrange
            var snapshotProvider = (ISessionSnapshotProvider) Session;
            var snapshot = snapshotProvider.GetSnapshot();

            //Act
            var count = snapshot.Nodes.Count(x => x.NodeType == NodeType.Exists);

            //Assert
            Assert.Equal(1, count);
        }

        protected override void SetUpRules()
        {
            SetUpRule<TwinRuleOne>();
            SetUpRule<TwinRuleTwo>();
        }

        public class FactType1
        {
            public string TestProperty { get; set; }
        }

        public class FactType2
        {
            public string TestProperty { get; set; }
            public string JoinProperty { get; set; }
        }

        public class FactType3
        {
            public string TestProperty { get; set; }
        }

        public class FactType4
        {
            public string TestProperty { get; set; }
        }

        public class TwinRuleOne : Rule
        {
            public override void Define()
            {
                FactType1 fact1 = null;
                FactType2 fact2 = null;
                IEnumerable<FactType4> group = null;

                When()
                    .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                    .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact1.TestProperty)
                    .Not<FactType3>(f => f.TestProperty.StartsWith("Invalid"))
                    .Exists<FactType3>(f => f.TestProperty.StartsWith("Valid"))
                    .Query(() => group, q => q
                        .Match<FactType4>()
                        .Where(f => f.TestProperty.StartsWith("Valid"))
                        .GroupBy(f => f.TestProperty)
                        .SelectMany(x => x)
                        .Collect()
                        .Where(c => c.Any()));

                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }

        public class TwinRuleTwo : Rule
        {
            public override void Define()
            {
                FactType1 fact1 = null;
                FactType2 fact2 = null;
                IEnumerable<FactType4> group = null;

                When()
                    .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                    .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact1.TestProperty)
                    .Not<FactType3>(f => f.TestProperty.StartsWith("Invalid"))
                    .Exists<FactType3>(f => f.TestProperty.StartsWith("Valid"))
                    .Query(() => group, q => q
                        .Match<FactType4>()
                        .Where(f => f.TestProperty.StartsWith("Valid"))
                        .GroupBy(f => f.TestProperty)
                        .SelectMany(x => x)
                        .Collect()
                        .Where(c => c.Any()));

                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}