using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Utilities;
using Xunit;

namespace NRules.Tests.Utilities
{
    public class ExpressionComparerTest
    {
        [Fact]
        public void AreEqual_BothNull_True()
        {
            AssertEqual(null, null);
        }

        [Fact]
        public void AreEqual_EquivalentNewArray_True()
        {
            // Arrange
            Expression<Func<IEnumerable<string>>> first = () => new[] {"string1", "string2"};
            Expression<Func<IEnumerable<string>>> second = () => new[] {"string1", "string2"};

            // Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentNewArray_False()
        {
            // Arrange
            Expression<Func<IEnumerable<string>>> first = () => new[] {"string1", "string2", "string3"};
            Expression<Func<IEnumerable<string>>> second = () => new[] {"string1", "string2"};

            // Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentConditionalArray_True()
        {
            // Arrange
            var strings = new[] {"string1", "string2"};
            Expression<Func<IEnumerable<string>>> first = () => strings.Length > 1 ? new string[0] : strings;
            Expression<Func<IEnumerable<string>>> second = () => strings.Length > 1 ? new string[0] : strings;

            // Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentConditionalArray_False()
        {
            // Arrange
            var strings = new[] {"string1", "string2"};
            Expression<Func<IEnumerable<string>>> first = () => strings.Length > 1 ? new string[0] : strings;
            Expression<Func<IEnumerable<string>>> second = () => strings.Length > 1 ? strings : new string[0];

            // Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentConditional_True()
        {
            //Arrange
            Expression<Func<int, int>> first = i1 => i1 == 1 ? 1 : 0;
            Expression<Func<int, int>> second = i2 => i2 == 1 ? 1 : 0;

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentConditional_Test_False()
        {
            //Arrange
            Expression<Func<int, int>> first = i1 => i1 == 1 ? 1 : 0;
            Expression<Func<int, int>> second = i2 => i2 == 2 ? 1 : 0;

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentConditional_IfTrue_False()
        {
            //Arrange
            Expression<Func<int, int>> first = i1 => i1 == 1 ? 1 : 0;
            Expression<Func<int, int>> second = i2 => i2 == 1 ? 2 : 0;

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentConditional_IfFalse_False()
        {
            //Arrange
            Expression<Func<int, int>> first = i1 => i1 == 1 ? 1 : 0;
            Expression<Func<int, int>> second = i2 => i2 == 1 ? 1 : 2;

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentBinary_True()
        {
            //Arrange
            Expression<Func<int, int, bool>> first = (i1, i2) => i1 == i2;
            Expression<Func<int, int, bool>> second = (ii1, ii2) => ii1 == ii2;

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_TwoNonEquivalentBinary_False()
        {
            //Arrange
            Expression<Func<int, int, bool>> first = (i1, i2) => i1 == i2;
            Expression<Func<int, int, bool>> second = (ii1, ii2) => ii1 != ii2;

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentUnary_True()
        {
            //Arrange
            Expression<Func<int, int>> first = i => -i;
            Expression<Func<int, int>> second = i => -i;

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentMember_True()
        {
            //Arrange
            Expression<Func<DateTime, DayOfWeek>> first = d => d.DayOfWeek;
            Expression<Func<DateTime, DayOfWeek>> second = d => d.DayOfWeek;

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentMethodCall_True()
        {
            //Arrange
            Expression<Func<DateTime, DateTime>> first = d => d.ToLocalTime();
            Expression<Func<DateTime, DateTime>> second = d => d.ToLocalTime();

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentStaticField_True()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Value == StaticField;
            Expression<Func<SomeFact, bool>> second = f => f.Value == StaticField;

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_StaticFieldVsCapturedVariable_False()
        {
            //Arrange
            var variable = StaticField;
            Expression<Func<SomeFact, bool>> first = f => f.Value == variable;
            Expression<Func<SomeFact, bool>> second = f => f.Value == StaticField;

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_CapturedVariablesPointingToSameValue_True()
        {
            //Arrange
            var variable1 = 1;
            var variable2 = 1;
            Expression<Func<SomeFact, bool>> first = f => f.Value == variable1;
            Expression<Func<SomeFact, bool>> second = f => f.Value == variable2;

            //Act - Assert
            AssertEqual(second, first);
        }

        [Fact]
        public void AreEqual_CapturedVariablesPointingToDifferentValues_True()
        {
            //Arrange
            var variable1 = 1;
            var variable2 = 2;
            Expression<Func<SomeFact, bool>> first = f => f.Value == variable1;
            Expression<Func<SomeFact, bool>> second = f => f.Value == variable2;

            //Act - Assert
            AssertNotEqual(second, first);
        }

        [Fact]
        public void AreEqual_EquivalentStaticProperty_True()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Value == StaticProperty;
            Expression<Func<SomeFact, bool>> second = f => f.Value == StaticProperty;

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentStaticMethod_True()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Value == StaticMethod();
            Expression<Func<SomeFact, bool>> second = f => f.Value == StaticMethod();

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentMethodWithArguments_True()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Value == StaticMethod("one");
            Expression<Func<SomeFact, bool>> second = f => f.Value == StaticMethod("one");

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentMethodWithArguments_False()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Value == StaticMethod("one");
            Expression<Func<SomeFact, bool>> second = f => f.Value == StaticMethod("two");

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentMethods_False()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Value == StaticMethod("one");
            Expression<Func<SomeFact, bool>> second = f => f.Value == OtherStaticMethod("one");

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentMemberAccessExtension_True()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Child.Values.Contains("sdlkjf");
            Expression<Func<SomeFact, bool>> second = f => f.Child.Values.Contains("sdlkjf");

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentMemberAccessExtension_False()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Child.Values.Contains("abcdef");
            Expression<Func<SomeFact, bool>> second = f => f.Child.Values.Contains("sdlkjf");

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentMemberAccess_True()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Child.Values.GetLength(0) == 0;
            Expression<Func<SomeFact, bool>> second = f => f.Child.Values.GetLength(0) == 0;

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentMemberAccess_False()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Child.Values.GetLength(0) == 0;
            Expression<Func<SomeFact, bool>> second = f => f.Child.Values.GetLength(1) == 0;

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentMemberAccessConvert_True()
        {
            //Arrange
            int value = 1;

            Expression<Func<SomeOtherFact, bool>> first = f => ((ISomeFact)f).Value.Equals(value);
            Expression<Func<SomeOtherFact, bool>> second = f => ((ISomeFact)f).Value.Equals(value);

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentMemberAccessConvert_False()
        {
            //Arrange
            int value1 = 1;
            int value2 = 2;

            Expression<Func<SomeOtherFact, bool>> first = f => ((ISomeFact)f).Value.Equals(value1);
            Expression<Func<SomeOtherFact, bool>> second = f => ((ISomeFact)f).Value.Equals(value2);

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentMemberAccessIndexer_True()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Child.Values[0] == "1";
            Expression<Func<SomeFact, bool>> second = f => f.Child.Values[0] == "1";

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentMemberAccessIndexer_False()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Child.Values[0] == "1";
            Expression<Func<SomeFact, bool>> second = f => f.Child.Values[1] == "1";

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentNew_True()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => new Tuple<int>(1).Item1 == f.Value;
            Expression<Func<SomeFact, bool>> second = f => new Tuple<int>(1).Item1 == f.Value;

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentNew_False()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => new Tuple<int>(1).Item1 == f.Value;
            Expression<Func<SomeFact, bool>> second = f => new Tuple<int>(2).Item1 == f.Value;

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentMemberCall_True()
        {
            //Arrange
            Expression<Func<SomeClass, bool>> first = x => x.NestedValue1().Values == null;
            Expression<Func<SomeClass, bool>> second = y => y.NestedValue1().Values == null;

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentMemberCall_False()
        {
            //Arrange
            Expression<Func<SomeClass, bool>> first = x => x.NestedValue1().Values == null;
            Expression<Func<SomeClass, bool>> second = x => x.NestedValue2().Values == null;

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentTypeBinaryExpression_True()
        {
            //Arrange
            Expression<Func<SomeClass, bool>> first = x => x is SomeClass;
            Expression<Func<SomeClass, bool>> second = x => x is SomeClass;

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentTypeBinaryExpression_False()
        {
            //Arrange
            Expression<Func<SomeClass, bool>> first = x => x is SomeClass;
            Expression<Func<SomeClass, bool>> second = x => x is object;

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentInvocationExpression_True()
        {
            //Arrange
            var methodInfo = GetType().GetMethods()
                .First(info => info.IsStatic && info.Name == "StaticMethod"
                               && info.GetParameters().Length == 1);

            var staticMethodDelegate = (Func<string, int>) methodInfo.CreateDelegate(typeof(Func<string, int>));

            Expression<Func<string, int>> first = data => staticMethodDelegate(data);
            Expression<Func<string, int>> second = data => staticMethodDelegate(data);

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentInvocationExpression_False()
        {
            //Arrange
            var methodInfos = GetType().GetMethods();

            var methodInfoWithArg = methodInfos
                .First(info => info.IsStatic && info.Name == "StaticMethod"
                               && info.GetParameters().Length == 1);

            var methodInfoWithoutArg = methodInfos
                .First(info => info.IsStatic && info.Name == "StaticMethod"
                               && !info.GetParameters().Any());

            var staticMethodWithArgDelegate =
                (Func<string, int>) methodInfoWithArg.CreateDelegate(typeof(Func<string, int>));
            var staticMethodWithoutArgDelegate = (Func<int>) methodInfoWithoutArg.CreateDelegate(typeof(Func<int>));

            Expression<Func<string, int>> first = data => staticMethodWithArgDelegate(data);
            Expression<Func<int>> second = () => staticMethodWithoutArgDelegate();

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentInvocationExpressionWithSimilarSignature_False()
        {
            //Arrange
            var methodInfos = GetType().GetMethods();

            var methodInfoWithArg = methodInfos
                .First(info => info.IsStatic && info.Name == "StaticMethod"
                               && info.GetParameters().Length == 1);

            var otherMethodInfoWithArg = methodInfos
                .First(info => info.IsStatic && info.Name == "OtherStaticMethod"
                               && info.GetParameters().Length == 1);

            var staticMethodWithArgDelegate =
                (Func<string, int>) methodInfoWithArg.CreateDelegate(typeof(Func<string, int>));
            var otherStaticMethodWithArgDelegate =
                (Func<string, int>) otherMethodInfoWithArg.CreateDelegate(typeof(Func<string, int>));

            Expression<Func<string, int>> first = data => staticMethodWithArgDelegate(data);
            Expression<Func<string, int>> second = data => otherStaticMethodWithArgDelegate(data);

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentMemberAssignment_True()
        {
            //Arrange
            Expression<Func<SomeClassWithProperty>> first = () => new SomeClassWithProperty { Value = "A" };
            Expression<Func<SomeClassWithProperty>> second = () => new SomeClassWithProperty { Value = "A" };

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentMemberAssignment_False()
        {
            //Arrange
            Expression<Func<SomeClassWithProperty>> first = () => new SomeClassWithProperty { Value = "A" };
            Expression<Func<SomeClassWithProperty>> second = () => new SomeClassWithProperty { Value = "B" };

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentListInitExpression_True()
        {
            //Arrange
            Expression<Func<IEnumerable<string>>> first = () => new List<string> { "A" };
            Expression<Func<IEnumerable<string>>> second = () => new List<string> { "A" };

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentListInitExpression_False()
        {
            //Arrange
            Expression<Func<IEnumerable<string>>> first = () => new List<string> { "A" };
            Expression<Func<IEnumerable<string>>> second = () => new List<string> { "B" };

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentMemberListBinding_True()
        {
            //Arrange
            Expression<Func<SomeClassWithListProperty>> first = () => new SomeClassWithListProperty() { List = { "A" } };
            Expression<Func<SomeClassWithListProperty>> second = () => new SomeClassWithListProperty { List = { "A" } };

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentMemberListBinding_False()
        {
            //Arrange
            Expression<Func<SomeClassWithListProperty>> first = () => new SomeClassWithListProperty() { List = { "A" } };
            Expression<Func<SomeClassWithListProperty>> second = () => new SomeClassWithListProperty { List = { "B" } };

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentMemberMemberBinding_True()
        {
            //Arrange
            Expression<Func<SomeClassWithComplexProperty>> first = () => new SomeClassWithComplexProperty { Value = { Value = "A" } };
            Expression<Func<SomeClassWithComplexProperty>> second = () => new SomeClassWithComplexProperty { Value = { Value = "A" } };

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentMemberMemberBinding_False()
        {
            //Arrange
            Expression<Func<SomeClassWithComplexProperty>> first = () => new SomeClassWithComplexProperty { Value = { Value = "A" } };
            Expression<Func<SomeClassWithComplexProperty>> second = () => new SomeClassWithComplexProperty { Value = { Value = "B" } };

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentSubtractMemberBinding_True()
        {
            //Arrange
            Expression<Func<ISomeFact, int>> first = x => (x.Value1 - x.Value2).Day;
            Expression<Func<ISomeFact, int>> second = x => (x.Value1 - x.Value2).Day;

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentSubtractMemberBinding_False()
        {
            //Arrange
            Expression<Func<ISomeFact, int>> first = x => (x.Value1 - x.Value2).Day;
            Expression<Func<ISomeFact, int>> second = x => (x.Value1 - x.Value2).Year;

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Fact]
        public void AreEqual_EquivalentAddMemberBinding_True()
        {
            //Arrange
            Expression<Func<ISomeFact, int>> first = x => (x.Value1 + x.Value2).Day;
            Expression<Func<ISomeFact, int>> second = x => (x.Value1 + x.Value2).Day;

            //Act - Assert
            AssertEqual(first, second);
        }

        [Fact]
        public void AreEqual_NonEquivalentAddMemberBinding_False()
        {
            //Arrange
            Expression<Func<ISomeFact, int>> first = x => (x.Value1 + x.Value2).Day;
            Expression<Func<ISomeFact, int>> second = x => (x.Value1 + x.Value2).Year;

            //Act - Assert
            AssertNotEqual(first, second);
        }

        private static void AssertEqual(Expression first, Expression second)
        {
            //Act
            bool result = ExpressionComparer.AreEqual(first, second);

            //Assert
            Assert.True(result);
        }

        private static void AssertNotEqual(Expression first, Expression second)
        {
            //Act
            bool result = ExpressionComparer.AreEqual(first, second);

            //Assert
            Assert.False(result);
        }

        public static readonly int StaticField = 1;

        public static int StaticProperty => 1;

        public static int StaticMethod()
        {
            return 1;
        }

        public static int StaticMethod(string param1)
        {
            return param1.GetHashCode();
        }

        public static int OtherStaticMethod(string param1)
        {
            return param1.GetHashCode();
        }

        public class SomeFact
        {
            public int Value = 1;

            public readonly SomeClass Child = new SomeClass();
        }

        public interface ISomeFact
        {
            int Value { get; set; }
            DateTime Value1 { get; set; }
            TimeSpan Value2 { get; set; }
        }

        public class SomeOtherFact : ISomeFact
        {
            int ISomeFact.Value { get; set; }
            public DateTime Value1 { get; set; }
            public TimeSpan Value2 { get; set; }
        }

        public class SomeClass
        {
            public readonly string[] Values = {"blop"};

            public SomeClass NestedValue1()
            {
                return new SomeClass();
            }

            public SomeClass NestedValue2()
            {
                return new SomeClass();
            }
        }

        public class SomeClassWithProperty
        {
            public string Value { get; set; }
        }

        public class SomeClassWithListProperty
        {
            public List<string> List { get; set; } = new List<string>();
        }

        public class SomeClassWithComplexProperty
        {
            public SomeClassWithProperty Value { get; set; } = new SomeClassWithProperty();
        }
    }
}