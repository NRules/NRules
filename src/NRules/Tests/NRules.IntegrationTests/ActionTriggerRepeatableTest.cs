using System;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests
{
    public class ActionTriggerRepeatableTest : BaseRuleTestFixture
    {
        public ActionTriggerRepeatableTest()
        {
            _matchActionCount = 0;
            _rematchActionCount = 0;
            _unmatchActionCount = 0;
        }

        private static int _matchActionCount;
        private static int _rematchActionCount;
        private static int _unmatchActionCount;

        private static readonly Action OnMatchAction = () => { _matchActionCount++; };
        private static readonly Action OnReMatchAction = () => { _rematchActionCount++; };
        private static readonly Action OnUnMatchAction = () => { _unmatchActionCount++; };

        [Fact]
        public void Fire_InsertThenFire_FiresOnMatch()
        {
            //Arrange
            var fact = new FactType();
            
            //Act
            Session.Insert(fact);
            Session.Fire();

            //Assert
            Assert.Equal(1, _matchActionCount);
            Assert.Equal(0, _rematchActionCount);
            Assert.Equal(0, _unmatchActionCount);
        }

        [Fact]
        public void Fire_FilterOffInsertThenFire_DoesNotFire()
        {
            //Arrange
            var fact = new FactType();
            
            //Act
            fact.AcceptFilter = false;
            Session.Insert(fact);
            Session.Fire();

            //Assert
            Assert.Equal(0, _matchActionCount);
            Assert.Equal(0, _rematchActionCount);
            Assert.Equal(0, _unmatchActionCount);
        }

        [Fact]
        public void Fire_InsertThenUpdateThenFire_FiresOnMatch()
        {
            //Arrange
            var fact = new FactType();
            
            //Act
            Session.Insert(fact);
            Session.Update(fact);
            Session.Fire();

            //Assert
            Assert.Equal(1, _matchActionCount);
            Assert.Equal(0, _rematchActionCount);
            Assert.Equal(0, _unmatchActionCount);
        }

        [Fact]
        public void Fire_InsertThenFilterOffThenUpdateThenFire_DoesNotFire()
        {
            //Arrange
            var fact = new FactType();
            
            //Act
            Session.Insert(fact);
            fact.AcceptFilter = false;
            Session.Update(fact);
            Session.Fire();

            //Assert
            Assert.Equal(0, _matchActionCount);
            Assert.Equal(0, _rematchActionCount);
            Assert.Equal(0, _unmatchActionCount);
        }

        [Fact]
        public void Fire_FilterOffInsertThenFilterOnThenUpdateThenFire_FiresOnMatch()
        {
            //Arrange
            var fact = new FactType();
            
            //Act
            fact.AcceptFilter = false;
            Session.Insert(fact);
            fact.AcceptFilter = true;
            Session.Update(fact);
            Session.Fire();

            //Assert
            Assert.Equal(1, _matchActionCount);
            Assert.Equal(0, _rematchActionCount);
            Assert.Equal(0, _unmatchActionCount);
        }

        [Fact]
        public void Fire_InsertThenFireThenUpdateThenFire_FiresOnMatchAndOnRematch()
        {
            //Arrange
            var fact = new FactType();

            //Act
            Session.Insert(fact);
            Session.Fire();
            Session.Update(fact);
            Session.Fire();

            //Assert
            Assert.Equal(1, _matchActionCount);
            Assert.Equal(1, _rematchActionCount);
            Assert.Equal(0, _unmatchActionCount);
        }

        [Fact]
        public void Fire_InsertThenFireThenTwoUpdatesThenFire_FiresOnMatchAndOnRematch()
        {
            //Arrange
            var fact = new FactType();

            //Act
            Session.Insert(fact);
            Session.Fire();
            Session.Update(fact);
            Session.Update(fact);
            Session.Fire();

            //Assert
            Assert.Equal(1, _matchActionCount);
            Assert.Equal(1, _rematchActionCount);
            Assert.Equal(0, _unmatchActionCount);
        }

        [Fact]
        public void Fire_InsertThenFireThenFilterOffThenUpdateThenFilterOnThenUpdateThenFire_FiresOnMatchAndOnRematch()
        {
            //Arrange
            var fact = new FactType();

            //Act
            Session.Insert(fact);
            Session.Fire();
            fact.AcceptFilter = false;
            Session.Update(fact);
            fact.AcceptFilter = true;
            Session.Update(fact);
            Session.Fire();

            //Assert
            Assert.Equal(1, _matchActionCount);
            Assert.Equal(1, _rematchActionCount);
            Assert.Equal(0, _unmatchActionCount);
        }

        [Fact]
        public void Fire_InsertThenFireThenUpdateThenFilterOffThenUpdateThenFire_FiresOnMatch()
        {
            //Arrange
            var fact = new FactType();

            //Act
            Session.Insert(fact);
            Session.Fire();
            Session.Update(fact);
            fact.AcceptFilter = false;
            Session.Update(fact);
            Session.Fire();

            //Assert
            Assert.Equal(1, _matchActionCount);
            Assert.Equal(0, _rematchActionCount);
            Assert.Equal(0, _unmatchActionCount);
        }

        [Fact]
        public void Fire_InsertThenFireThenUpdateThenFireThenUpdateThenFire_FiresOnMatchAndOnRematchTwice()
        {
            //Arrange
            var fact = new FactType();

            //Act
            Session.Insert(fact);
            Session.Fire();
            Session.Update(fact);
            Session.Fire();
            Session.Update(fact);
            Session.Fire();

            //Assert
            Assert.Equal(1, _matchActionCount);
            Assert.Equal(2, _rematchActionCount);
            Assert.Equal(0, _unmatchActionCount);
        }

        [Fact]
        public void Fire_InsertThenRetractThenFire_DoesNotFire()
        {
            //Arrange
            var fact = new FactType();

            //Act
            Session.Insert(fact);
            Session.Retract(fact);
            Session.Fire();

            //Assert
            Assert.Equal(0, _matchActionCount);
            Assert.Equal(0, _rematchActionCount);
            Assert.Equal(0, _unmatchActionCount);
        }

        [Fact]
        public void Fire_InsertThenFireThenRetractThenFire_FiresOnMatchAndOnUnmatch()
        {
            //Arrange
            var fact = new FactType();

            //Act
            Session.Insert(fact);
            Session.Fire();
            Session.Retract(fact);
            Session.Fire();

            //Assert
            Assert.Equal(1, _matchActionCount);
            Assert.Equal(0, _rematchActionCount);
            Assert.Equal(1, _unmatchActionCount);
        }

        [Fact]
        public void Fire_InsertThenFireThenFilterOffThenRetractThenFire_FiresOnMatchAndOnUnmatch()
        {
            //Arrange
            var fact = new FactType();

            //Act
            Session.Insert(fact);
            Session.Fire();
            fact.AcceptFilter = false;
            Session.Retract(fact);
            Session.Fire();

            //Assert
            Assert.Equal(1, _matchActionCount);
            Assert.Equal(0, _rematchActionCount);
            Assert.Equal(1, _unmatchActionCount);
        }
        
        [Fact]
        public void Fire_InsertThenFireThenUpdateThenFireThenRetractThenFire_FiresOnMatchAndOnRematchAndOnUnmatch()
        {
            //Arrange
            var fact = new FactType();

            //Act
            Session.Insert(fact);
            Session.Fire();
            Session.Update(fact);
            Session.Fire();
            Session.Retract(fact);
            Session.Fire();

            //Assert
            Assert.Equal(1, _matchActionCount);
            Assert.Equal(1, _rematchActionCount);
            Assert.Equal(1, _unmatchActionCount);
        }

        [Fact]
        public void Fire_InsertThenFireThenFilterOffThenUpdateThenFireThenRetractThenFire_FiresOnMatchAndOnUnmatch()
        {
            //Arrange
            var fact = new FactType();

            //Act
            Session.Insert(fact);
            Session.Fire();
            fact.AcceptFilter = false;
            Session.Update(fact);
            Session.Fire();
            Session.Retract(fact);
            Session.Fire();

            //Assert
            Assert.Equal(1, _matchActionCount);
            Assert.Equal(0, _rematchActionCount);
            Assert.Equal(1, _unmatchActionCount);
        }

        [Fact]
        public void Fire_InsertThenFireThenUpdateThenRetractThenFire_FiresOnMatchAndOnUnmatch()
        {
            //Arrange
            var fact = new FactType();

            //Act
            Session.Insert(fact);
            Session.Fire();
            Session.Update(fact);
            Session.Retract(fact);
            Session.Fire();

            //Assert
            Assert.Equal(1, _matchActionCount);
            Assert.Equal(0, _rematchActionCount);
            Assert.Equal(1, _unmatchActionCount);
        }

        [Fact]
        public void Fire_InsertThenFireThenFilterOffThenUpdateThenRetractThenFire_FiresOnMatchAndOnUnmatch()
        {
            //Arrange
            var fact = new FactType();

            //Act
            Session.Insert(fact);
            Session.Fire();
            fact.AcceptFilter = false;
            Session.Update(fact);
            Session.Retract(fact);
            Session.Fire();

            //Assert
            Assert.Equal(1, _matchActionCount);
            Assert.Equal(0, _rematchActionCount);
            Assert.Equal(1, _unmatchActionCount);
        }

        [Fact]
        public void Fire_InsertThenFireThenRetractThenFireThenInsertThenFire_FiresOnMatchTwiceAndOnUnmatch()
        {
            //Arrange
            var fact = new FactType();

            //Act
            Session.Insert(fact);
            Session.Fire();
            Session.Retract(fact);
            Session.Fire();
            Session.Insert(fact);
            Session.Fire();

            //Assert
            Assert.Equal(2, _matchActionCount);
            Assert.Equal(0, _rematchActionCount);
            Assert.Equal(1, _unmatchActionCount);
        }

        [Fact]
        public void Fire_InsertThenFireThenRetractThenInsertThenFire_FiresOnMatchTwiceAndOnUnmatch()
        {
            //Arrange
            var fact = new FactType();

            //Act
            Session.Insert(fact);
            Session.Fire();
            Session.Retract(fact);
            Session.Insert(fact);
            Session.Fire();

            //Assert
            Assert.Equal(2, _matchActionCount);
            Assert.Equal(0, _rematchActionCount);
            Assert.Equal(1, _unmatchActionCount);
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType
        {
            public bool AcceptFilter { get; set; } = true;
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                FactType fact = null;

                When()
                    .Match(() => fact);

                Filter()
                    .Where(() => fact.AcceptFilter);

                Then()
                    .Action(ctx => OnMatchAction(), ActionTrigger.Activated)
                    .Action(ctx => OnReMatchAction(), ActionTrigger.Reactivated)
                    .Action(ctx => OnUnMatchAction(), ActionTrigger.Deactivated);
            }
        }
    }
}