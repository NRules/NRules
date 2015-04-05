using System;
using System.Linq;
using System.Linq.Expressions;
using NRules.Utilities;
using NUnit.Framework;

namespace NRules.Tests.Utilities
{
    [TestFixture]
    public class ExpressionComparerTest
    {
        [Test]
        public void AreEqual_BothNull_True()
        {
            AssertEqual(null, null);
        }

        [Test]
        public void AreEqual_EquivalentBinary_True()
        {
            //Arrange
            Expression<Func<int, int, bool>> first = (i1, i2) => i1 == i2;
            Expression<Func<int, int, bool>> second = (ii1, ii2) => ii1 == ii2;

            //Act - Assert
            AssertEqual(first, second);
        }
        
        [Test]
        public void AreEqual_TwoNonEquivalentBinary_False()
        {
            //Arrange
            Expression<Func<int, int, bool>> first = (i1, i2) => i1 == i2;
            Expression<Func<int, int, bool>> second = (ii1, ii2) => ii1 != ii2;

            //Act - Assert
            AssertNotEqual(first, second);
        }
        
        [Test]
        public void AreEqual_EquivalentUnary_True()
        {
            //Arrange
            Expression<Func<int, int>> first = i => -i;
            Expression<Func<int, int>> second = i => -i;

            //Act - Assert
            AssertEqual(first, second);
        }
        
        [Test]
        public void AreEqual_EquivalentMember_True()
        {
            //Arrange
            Expression<Func<DateTime, DayOfWeek>> first = d => d.DayOfWeek;
            Expression<Func<DateTime, DayOfWeek>> second = d => d.DayOfWeek;

            //Act - Assert
            AssertEqual(first, second);
        }

        [Test]
        public void AreEqual_EquivalentMethodCall_True()
        {
            //Arrange
            Expression<Func<DateTime, DateTime>> first = d => d.ToLocalTime();
            Expression<Func<DateTime, DateTime>> second = d => d.ToLocalTime();

            //Act - Assert
            AssertEqual(first, second);
        }

        [Test]
        public void AreEqual_EquivalentStaticField_True()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Value == StaticField;
            Expression<Func<SomeFact, bool>> second = f => f.Value == StaticField;

            //Act - Assert
            AssertEqual(first, second);
        }

        [Test]
        public void AreEqual_EquivalentStaticProperty_True()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Value == StaticProperty;
            Expression<Func<SomeFact, bool>> second = f => f.Value == StaticProperty;

            //Act - Assert
            AssertEqual(first, second);
        }

        [Test]
        public void AreEqual_EquivalentStaticMethod_True()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Value == StaticMethod();
            Expression<Func<SomeFact, bool>> second = f => f.Value == StaticMethod();

            //Act - Assert
            AssertEqual(first, second);
        }

        [Test]
        public void AreEqual_EquivalentMethodWithArguments_True()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Value == StaticMethod("one");
            Expression<Func<SomeFact, bool>> second = f => f.Value == StaticMethod("one");

            //Act - Assert
            AssertEqual(first, second);
        }

        [Test]
        public void AreEqual_NonEquivalentMethodWithArguments_False()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Value == StaticMethod("one");
            Expression<Func<SomeFact, bool>> second = f => f.Value == StaticMethod("two");

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Test]
        public void AreEqual_EquivalentMemberAccessExtension_True()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Child.Values.Contains("sdlkjf");
            Expression<Func<SomeFact, bool>> second = f => f.Child.Values.Contains("sdlkjf");

            //Act - Assert
            AssertEqual(first, second);
        }

        [Test]
        public void AreEqual_EquivalentMemberAccess_True()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Child.Values.GetLength(0) == 0;
            Expression<Func<SomeFact, bool>> second = f => f.Child.Values.GetLength(0) == 0;

            //Act - Assert
            AssertEqual(first, second);
        }

        [Test]
        public void AreEqual_EquivalentMemberAccessIndexer_True()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Child.Values[0] == "1";
            Expression<Func<SomeFact, bool>> second = f => f.Child.Values[0] == "1";

            //Act - Assert
            AssertEqual(first, second);
        }

        [Test]
        public void AreEqual_NonEquivalentMemberAccessIndexer_False()
        {
            //Arrange
            Expression<Func<SomeFact, bool>> first = f => f.Child.Values[0] == "1";
            Expression<Func<SomeFact, bool>> second = f => f.Child.Values[1] == "1";

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Test]
        public void AreEqual_EquivalentInvocationExpression_True()
        {
            //Arrange
            var methodInfo = GetType().GetMethods()
                .First(info => info.IsStatic && info.Name == "StaticMethod" 
                    && info.GetParameters().Length == 1);

            var staticMethodDelegate = (Func<string, int>)Delegate.CreateDelegate(typeof(Func<string, int>), methodInfo);

            Expression<Func<string, int>> first = data => staticMethodDelegate(data);
            Expression<Func<string, int>> second = data => staticMethodDelegate(data);

            //Act - Assert
            AssertEqual(first, second);
        }

        [Test]
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

            var staticMethodWithArgDelegate = (Func<string, int>)Delegate.CreateDelegate(typeof(Func<string, int>), methodInfoWithArg);
            var staticMethodWithoutArgDelegate = (Func<int>)Delegate.CreateDelegate(typeof(Func<int>), methodInfoWithoutArg);

            Expression<Func<string, int>> first = data => staticMethodWithArgDelegate(data);
            Expression<Func<int>> second = () => staticMethodWithoutArgDelegate();

            //Act - Assert
            AssertNotEqual(first, second);
        }

        [Test]
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

            var staticMethodWithArgDelegate = (Func<string, int>)Delegate.CreateDelegate(typeof(Func<string, int>), methodInfoWithArg);
            var otherStaticMethodWithArgDelegate = (Func<string, int>)Delegate.CreateDelegate(typeof(Func<string, int>), otherMethodInfoWithArg);

            Expression<Func<string, int>> first = data => staticMethodWithArgDelegate(data);
            Expression<Func<string, int>> second = data => otherStaticMethodWithArgDelegate(data);

            //Act - Assert
            AssertNotEqual(first, second);
        }

        private static void AssertEqual(Expression first, Expression second)
        {
            //Act
            bool result = ExpressionComparer.AreEqual(first, second);

            //Assert
            Assert.That(result, Is.True);
        }

        private static void AssertNotEqual(Expression first, Expression second)
        {
            //Act
            bool result = ExpressionComparer.AreEqual(first, second);

            //Assert
            Assert.That(result, Is.False);
        }

        public static int StaticField = 1;

        public static int StaticProperty
        {
            get { return 1; }
        }

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

            public SomeClass Child = new SomeClass();
        }

        public class SomeClass
        {
            public string[] Values = { "blop" };
        }
    }
}