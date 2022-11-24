﻿using System.Collections.Generic;
using System.Linq;
using NRules.Diagnostics;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests;

public class NodeSharingTest : BaseRulesTestFixture
{
    [Fact]
    public void Session_AlphaSelectionNodes_OnePerIntraCondition()
    {
        //Arrange
        var schema = Session.GetSchema();

        //Act
        var count = schema.Nodes.Count(x => x.NodeType == NodeType.Selection);

        //Assert
        Assert.Equal(5, count);
    }

    [Fact]
    public void Session_BetaJoinNodes_CorrectCount()
    {
        //Arrange
        var schema = Session.GetSchema();

        //Act
        var count = schema.Nodes.Count(x => x.NodeType == NodeType.Join);

        //Assert
        Assert.Equal(4, count);
    }

    [Fact]
    public void Session_AggregateNodes_CorrectCount()
    {
        //Arrange
        var schema = Session.GetSchema();

        //Act
        var count = schema.Nodes.Count(x => x.NodeType == NodeType.Aggregate);

        //Assert
        Assert.Equal(3, count);
    }

    [Fact]
    public void Session_NotNodes_CorrectCount()
    {
        //Arrange
        var schema = Session.GetSchema();

        //Act
        var count = schema.Nodes.Count(x => x.NodeType == NodeType.Not);

        //Assert
        Assert.Equal(1, count);
    }

    [Fact]
    public void Session_ExistsNodes_CorrectCount()
    {
        //Arrange
        var schema = Session.GetSchema();

        //Act
        var count = schema.Nodes.Count(x => x.NodeType == NodeType.Exists);

        //Assert
        Assert.Equal(1, count);
    }

    [Fact]
    public void Session_BindingNodes_CorrectCount()
    {
        //Arrange
        var schema = Session.GetSchema();

        //Act
        var count = schema.Nodes.Count(x => x.NodeType == NodeType.Binding);

        //Assert
        Assert.Equal(1, count);
    }

    protected override void SetUpRules(Testing.IRepositorySetup setup)
    {
        setup.Rule<TwinRuleOne>();
        setup.Rule<TwinRuleTwo>();
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
            string joinValue = null;
            FactType2 fact2 = null;
            IEnumerable<FactType4> group = null;

            When()
                .Match(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Let(() => joinValue, () => fact1.TestProperty)
                .Match(() => fact2, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == joinValue)
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
            string joinValue = null;
            FactType2 fact2 = null;
            IEnumerable<FactType4> group = null;

            When()
                .Match(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Let(() => joinValue, () => fact1.TestProperty)
                .Match(() => fact2, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == joinValue)
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