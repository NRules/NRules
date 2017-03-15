using System.Linq;
using NRules.Fluent;
using NRules.IntegrationTests.TestRules;
using NRules.RuleModel;
using NUnit.Framework;

namespace NRules.IntegrationTests
{
    [TestFixture]
    public class RuleMetadataTest
    {
        private RuleRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new RuleRepository();
        }

        [Test]
        public void Name_NameAttributePresent_CustomValue()
        {
            //Arrange
            _repository.Load(x => x.From(typeof(RuleWithMetadata)));
            IRuleDefinition rule = _repository.GetRules().Single();

            //Act
            string actual = rule.Name;

            //Assert
            Assert.AreEqual("Rule with metadata", actual);
        }

        [Test]
        public void Name_NameAttributeOverriden_CustomValue()
        {
            //Arrange
            _repository.Load(x => x.From(typeof(RuleWithMetadataAndOverride)));
            IRuleDefinition rule = _repository.GetRules().Single();

            //Act
            string actual = rule.Name;

            //Assert
            Assert.AreEqual("Programmatic name", actual);
        }

        [Test]
        public void Description_DescriptionAttributePresent_CustomValue()
        {
            //Arrange
            _repository.Load(x => x.From(typeof(RuleWithMetadata)));
            IRuleDefinition rule = _repository.GetRules().Single();

            //Act
            string actual = rule.Description;

            //Assert
            Assert.AreEqual("Rule description", actual);
        }

        [Test]
        public void Tags_TagAttributesPresent_CustomValues()
        {
            //Arrange
            _repository.Load(x => x.From(typeof(RuleWithMetadata)));
            IRuleDefinition rule = _repository.GetRules().Single();

            //Act
            string[] actual = rule.Tags.ToArray();

            //Assert
            Assert.AreEqual(2, actual.Length);
            Assert.Contains("Test", actual);
            Assert.Contains("Metadata", actual);
        }

        [Test]
        public void Priority_PriorityAttributePresent_CustomValue()
        {
            //Arrange
            _repository.Load(x => x.From(typeof(RuleWithMetadata)));
            IRuleDefinition rule = _repository.GetRules().Single();

            //Act
            int actual = rule.Priority;

            //Assert
            Assert.AreEqual(100, actual);
        }

        [Test]
        public void Tags_TagAttributesAndParentAttributesPresent_CustomValues()
        {
            //Arrange
            _repository.Load(x => x.From(typeof(RuleWithMetadataAndParentMetadata)));
            IRuleDefinition rule = _repository.GetRules().Single();

            //Act
            string[] actual = rule.Tags.ToArray();

            //Assert
            Assert.AreEqual(4, actual.Length);
            Assert.Contains("ChildTag", actual);
            Assert.Contains("ChildMetadata", actual);
            Assert.Contains("ParentTag", actual);
            Assert.Contains("ParentMetadata", actual);
        }

        [Test]
        public void Priority_PriorityParentAttributePresent_CustomValue()
        {
            //Arrange
            _repository.Load(x => x.From(typeof(RuleWithMetadataAndParentMetadata)));
            IRuleDefinition rule = _repository.GetRules().Single();

            //Act
            int actual = rule.Priority;

            //Assert
            Assert.AreEqual(200, actual);
        }

        [Test]
        public void Priority_PriorityAttributeOverriden_CustomValue()
        {
            //Arrange
            _repository.Load(x => x.From(typeof(RuleWithMetadataAndParentMetadataAndOverrides)));
            IRuleDefinition rule = _repository.GetRules().Single();

            //Act
            int actual = rule.Priority;

            //Assert
            Assert.AreEqual(500, actual);
        }

        [Test]
        public void Priority_PriorityAttributeOverridenProgrammatically_CustomValue()
        {
            //Arrange
            _repository.Load(x => x.From(typeof(RuleWithMetadataAndOverride)));
            IRuleDefinition rule = _repository.GetRules().Single();

            //Act
            int actual = rule.Priority;

            //Assert
            Assert.AreEqual(1000, actual);
        }

        [Test]
        public void Name_NoAttributes_TypeName()
        {
            //Arrange
            _repository.Load(x => x.From(typeof(RuleWithoutMetadata)));
            IRuleDefinition rule = _repository.GetRules().Single();

            //Act
            string actual = rule.Name;

            //Assert
            Assert.AreEqual(typeof(RuleWithoutMetadata).FullName, actual);
        }

        [Test]
        public void Description_NoAttributes_Empty()
        {
            //Arrange
            _repository.Load(x => x.From(typeof(RuleWithoutMetadata)));
            IRuleDefinition rule = _repository.GetRules().Single();

            //Act
            string actual = rule.Description;

            //Assert
            Assert.AreEqual(string.Empty, actual);
        }

        [Test]
        public void Tags_NoAttributes_Empty()
        {
            //Arrange
            _repository.Load(x => x.From(typeof(RuleWithoutMetadata)));
            IRuleDefinition rule = _repository.GetRules().Single();

            //Act
            string[] actual = rule.Tags.ToArray();

            //Assert
            Assert.AreEqual(0, actual.Length);
        }

        [Test]
        public void Priority_NoAttribute_Default()
        {
            //Arrange
            _repository.Load(x => x.From(typeof(RuleWithoutMetadata)));
            IRuleDefinition rule = _repository.GetRules().Single();

            //Act
            int actual = rule.Priority;

            //Assert
            Assert.AreEqual(0, actual);
        }
    }
}