using System;
using System.Collections.Generic;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class FilterCustomEqualityFilterOnValueKeyComparerTest : BaseRuleTestFixture
    {
        [Fact]
        public void FilterOnChange_UsingCustomEqualityComparer_FiresBasedOnCustomComparer_FiresOnChange()
        {
            // Arrange
            var fact = new FactIdEqual {Id = 1, Value = 1};

            Session.Insert(fact);

            // Act
            Session.Fire();

            // Assert
            AssertFiredOnce();

            // Arrange
            var newFact = new FactIdEqual {Id = 1, Value = 2};

            Session.Update(newFact);

            // Act
            Session.Fire();

            // Assert
            AssertFiredTwice();
        }

        [Fact]
        public void FilterOnChange_UsingCustomEqualityComparer_FiresBasedOnCustomComparer_DoesNotFireWhenNoChange()
        {
            // Arrange
            var fact = new FactIdEqual {Id = 1, Value = 1};

            Session.Insert(fact);

            // Act
            Session.Fire();

            // Assert
            AssertFiredOnce();

            // Arrange
            var newFact = new FactIdEqual {Id = 1, Value = 1};

            Session.Update(newFact);

            // Act
            Session.Fire();

            // Assert
            AssertFiredOnce();
        }

        // implements id equality
        public class FactIdEqual : IEquatable<FactIdEqual>
        {
            public int Id { get; set; }
            public int Value { get; set; }

            public bool Equals(FactIdEqual other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Id == other.Id;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((FactIdEqual) obj);
            }

            public override int GetHashCode() { return Id; }
        }

        public class FactIdValueEqualityComparer : IEqualityComparer<FactIdEqual>
        {
            public bool Equals(FactIdEqual x, FactIdEqual y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Id == y.Id && x.Value == y.Value;
            }

            public int GetHashCode(FactIdEqual obj)
            {
                unchecked
                {
                    return (obj.Id * 397) ^ obj.Value;
                }
            }

            public static IEqualityComparer<FactIdEqual> Instance { get; } = new FactIdValueEqualityComparer();
        }

        public class FilterOnIdValueKeyRule : Rule
        {
            public override void Define()
            {
                FactIdEqual factIdEqual = null;

                When()
                    .Match(() => factIdEqual);

                Filter()
                    .OnChange(() => factIdEqual, () => FactIdValueEqualityComparer.Instance);
            }
        }

        protected override void SetUpRules() { SetUpRule<FilterOnIdValueKeyRule>(); }
    }
}