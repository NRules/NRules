using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Core.IntegrationTests.Domain;
using NRules.Core.IntegrationTests.Rules;
using NRules.Core.IntegrationTests.Tests.Helpers;
using NUnit.Framework;

namespace NRules.Core.IntegrationTests.Tests
{
    internal class SimpleJoinRuleTests
    {
        private List<InsuranceApplicant> _qualifiers;
        private ISession _session;
        private IContainer _container;

        [SetUp]
        public void Setup()
        {
            _qualifiers = new List<InsuranceApplicant>();
            _container = DependencyFactory.GetContainer(QualificationHandler);

            var repository = new RuleRepository(_container);
            repository.AddRuleSet(typeof (SelfBeneficialPolicyRule));

            var factory = new SessionFactory(repository);
            _session = factory.CreateSession();
        }

        private void QualificationHandler(object sender, EventArgs data)
        {
            var args = data as QualificationEventArgs;
            _qualifiers.Add(args.Applicant);
        }

        [Test]
        public void SimpleJoinRule_SelfBeneficialPolicy_RuleIsSatisfied()
        {
            // Arrange
            Person person = PersonFactory.MiddledAgedPerson();
            InsuranceHistory history = InsuranceHistoryFactory.StandardInsurancePolicies(person);
            var applicant = new InsuranceApplicant(person, history, null);
            _session.Insert(applicant);
            _session.Insert(person);
            _session.Insert(history);
            _session.Insert(history.Policies.First());
            _session.Insert(history.Policies.First().Beneficiary);

            // Act
            _session.Fire();

            // Assert
            Assert.AreEqual(1, _qualifiers.Count);
        }

        [Test]
        public void SimpleJoinRule_NotSelfBeneficialPolicy_RuleIsNotSatisfied()
        {
            // Arrange
            Person person1 = PersonFactory.MiddledAgedPerson();
            Person person2 = PersonFactory.MiddledAgedPerson();
            InsuranceHistory history = InsuranceHistoryFactory.StandardInsurancePolicies(person1);
            var applicant = new InsuranceApplicant(person2, history, null);
            _session.Insert(applicant);

            // Act
            _session.Fire();

            // Assert
            Assert.AreEqual(0, _qualifiers.Count);
        }
    }
}