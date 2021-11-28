using System;
using System.Collections.Generic;
using NRules.Json.Tests.TestAssets;
using Xunit;

namespace NRules.Json.Tests
{
    public class TypeResolverTest
    {
        [Fact]
        public void Roundtrip_SimpleSystemType_TypeNameWithoutAssemblyName()
        {
            TestRoundtrip(
                typeof(string),
                typeof(string).FullName);
        }

        [Fact]
        public void Roundtrip_SimpleCustomType_TypeNameWithAssemblyName()
        {
            TestRoundtrip(
                typeof(FactType1),
                typeof(FactType1).AssemblyQualifiedName);
        }
        
        [Fact]
        public void Roundtrip_SimpleTypeWithAlias_Alias()
        {
            TestRoundtrip(new()
                {
                    { "string", typeof(string) }
                },
                typeof(string),
                "string");
        }
        
        [Fact]
        public void Roundtrip_ArrayTypeWithAlias_Alias()
        {
            TestRoundtrip(new()
                {
                    { "string", typeof(string) }
                },
                typeof(string[]),
                "string[]");
        }
        
        [Fact]
        public void Roundtrip_2DArrayTypeWithAlias_Alias()
        {
            TestRoundtrip(new()
                {
                    { "string", typeof(string) }
                },
                typeof(string[,]),
                "string[,]");
        }
        
        [Fact]
        public void Roundtrip_JaggedArrayTypeWithAlias_Alias()
        {
            TestRoundtrip(new()
                {
                    { "string", typeof(string) }
                },
                typeof(string[][]),
                "string[][]");
        }

        [Fact]
        public void Roundtrip_GenericSystemTypeWithSystemTypeArgument_TypeNamesWithoutAssemblyNames()
        {
            TestRoundtrip(
                typeof(List<string>),
                $"{typeof(List<>).FullName}[[{typeof(string).FullName}]]");
        }

        [Fact]
        public void Roundtrip_GenericSystemTypeWithAliasWithSystemTypeArgumentWithAlias_GenericAliasWithTypeArgumentAlias()
        {
            TestRoundtrip(new()
                {
                    { "list", typeof(List<>) },
                    { "string", typeof(string) }
                },
                typeof(List<string>),
                "list[[string]]");
        }

        [Fact]
        public void Roundtrip_ConstructedGenericSystemTypeWithAlias_Alias()
        {
            TestRoundtrip(new()
                {
                    { "list_s", typeof(List<string>) }
                },
                typeof(List<string>),
                "list_s");
        }

        [Fact]
        public void Roundtrip_NestedGenericSystemTypeWithSystemTypeArgument_TypeNamesWithoutAssemblyNames()
        {
            TestRoundtrip(
                typeof(Dictionary<int, List<string>>),
                $"{typeof(Dictionary<,>).FullName}[[{typeof(int).FullName}],[{typeof(List<>).FullName}[[{typeof(string).FullName}]]]]");
        }

        [Fact]
        public void Roundtrip_GenericSystemTypeWithCustomTypeArgument_GenericTypeNameWithoutAssemblyNameTypeArgumentWithAssemblyName()
        {
            TestRoundtrip(
                typeof(List<FactType1>),
                $"{typeof(List<>).FullName}[[{typeof(FactType1).AssemblyQualifiedName}]]");
        }

        [Fact]
        public void Roundtrip_GenericCustomTypeWithSystemTypeArgument_GenericTypeNameWithAssemblyNameTypeArgumentWithoutAssemblyName()
        {
            TestRoundtrip(
                typeof(Container<string>),
                $"{typeof(Container<>).FullName}[[{typeof(string).FullName}]], {typeof(Container<>).Assembly.FullName}");
        }

        [Fact]
        public void Roundtrip_GenericCustomTypeWithAliasWithSystemTypeArgument_GenericTypeAliasTypeArgumentWithoutAssemblyName()
        {
            TestRoundtrip(new()
                {
                    { "container", typeof(Container<>) }
                },
                typeof(Container<string>),
                $"container[[{typeof(string).FullName}]]");
        }

        [Fact]
        public void Roundtrip_CustomNestedClassType_OuterTypePlusInnerType()
        {
            TestRoundtrip(
                typeof(Outer.Inner),
                $"{typeof(Outer).FullName}+{nameof(Outer.Inner)}, {typeof(Container<>).Assembly.FullName}");
        }

        [Fact]
        public void Roundtrip_CustomNestedClassTypeWithAlias_Alias()
        {
            TestRoundtrip(new()
                {
                    { "inner", typeof(Outer.Inner) }
                },
                typeof(Outer.Inner),
                "inner");
        }

        [Fact]
        public void GetTypeFromName_InvalidName_Throws()
        {
            //Arrange
            var target = CreateTarget();

            //Act - Assert
            var ex = Assert.Throws<ArgumentException>(() => target.GetTypeFromName("BadTypeName")); 
            Assert.Contains("BadTypeName", ex.Message);
        }

        private void TestRoundtrip(Type originalType, string expectedTypeName)
        {
            TestRoundtrip(new (), originalType, expectedTypeName);
        }

        private void TestRoundtrip(Dictionary<string, Type> aliases, Type originalType, string expectedTypeName)
        {
            var typeResolver = CreateTarget();
            foreach (var alias in aliases)
            {
                typeResolver.RegisterAlias(alias.Key, alias.Value);
            }

            var serializedTypeName = typeResolver.GetTypeName(originalType);
            var deserializedType = typeResolver.GetTypeFromName(serializedTypeName);

            Assert.Equal(expectedTypeName, serializedTypeName);
            Assert.Equal(originalType, deserializedType);
        }

        private static TypeResolver CreateTarget()
        {
            return new TypeResolver();
        }
    }
}
