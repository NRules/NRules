using System;
using System.Collections.Generic;
using System.Reflection;
using NRules.Core.IntegrationTests.Domain;
using NRules.Core.IntegrationTests.Rules;
using NRules.Core.IntegrationTests.Tests.Helpers;
using NUnit.Framework;

namespace NRules.Core.IntegrationTests.Tests
{
    [TestFixture]
    public class SimplePersonalFinancesRuleTests
    {
        private List<InsuranceApplicant> _qualifiers;
        private ISession _session;
        private IContainer _container;
        private readonly InsuranceHistory _insuranceHistory = new InsuranceHistory(new List<InsurancePolicy>(), 0, 0, 0);

        [SetUp]
        public void Setup()
        {
            _qualifiers = new List<InsuranceApplicant>();
            _container = DependencyFactory.GetContainer(QualificationHandler);

            var repository = new RuleRepository(_container);
            repository.AddRuleSet(Assembly.GetAssembly(typeof(SimplePersonalFinancesRule)));

            var factory = new SessionFactory(repository);
            _session = factory.CreateSession();
        }

        private void QualificationHandler(object sender, EventArgs data)
        {
            var args = data as QualificationEventArgs;
            _qualifiers.Add(args.Applicant);
        }

        [Test]
        public void SimplePersonalFinancesRule_ApplicantTooYoung_RuleIsNotSatisfied()
        {
            // Arrange
            PersonalFinances poorFinances = PersonalFinancesFactory.PoorAndNotInDebt();
            PersonalFinances richFinances = PersonalFinancesFactory.RichAndInDebt();
            Person poorPerson = PersonFactory.YoungPerson();
            Person richPerson = PersonFactory.YoungPerson();
            var poorApplicant = new InsuranceApplicant(poorPerson, _insuranceHistory, poorFinances);
            var richApplicant = new InsuranceApplicant(richPerson, _insuranceHistory, richFinances);
            _session.Insert(poorApplicant);
            _session.Insert(richApplicant);
            _session.Insert(poorFinances);
            _session.Insert(richFinances);

            // Act
            _session.Fire();

            // Assert
            Assert.AreEqual(0, _qualifiers.Count);
        }

        [Test]
        public void SimplePersonalFinancesRule_ApplicantOfAgeAndMeans_RuleIsSatisfied()
        {
            // Arrange
            PersonalFinances richFinances = PersonalFinancesFactory.RichAndInDebt();
            Person middledAgedPerson = PersonFactory.MiddledAgedPerson();
            var richApplicant = new InsuranceApplicant(middledAgedPerson, _insuranceHistory, richFinances);
            _session.Insert(richApplicant);
            _session.Insert(richFinances);

            // Act
            _session.Fire();

            // Assert
            Assert.AreEqual(1, _qualifiers.Count);
            Assert.True(_qualifiers.Contains(richApplicant));
        }

        [Test]
        public void SimplePersonalFinancesRule_ApplicantOfAgeButNotMeans_RuleIsNotSatisfied()
        {
            // Arrange
            PersonalFinances poorFinances = PersonalFinancesFactory.PoorAndInDebt();
            Person middledAgedPerson = PersonFactory.MiddledAgedPerson();
            var poorApplicant = new InsuranceApplicant(middledAgedPerson, _insuranceHistory, poorFinances);
            _session.Insert(poorApplicant);
            _session.Insert(poorFinances);

            // Act
            _session.Fire();

            // Assert
            Assert.AreEqual(0, _qualifiers.Count);
        }
    }
}
