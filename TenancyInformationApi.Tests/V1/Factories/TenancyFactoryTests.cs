using System;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using TenancyInformationApi.V1.Factories;
using TenancyInformationApi.V1.Infrastructure;

namespace TenancyInformationApi.Tests.V1.Factories
{
    [TestFixture]
    public class TenancyFactoryTests
    {
        private Fixture _fixture = new Fixture();

        //TODO: Test for presence of Agreement/Tenure data in Domain object.
        [Test]
        public void EntityContainsAppropriateData()
        {
            var uhTenancy = _fixture.Create<UhTenancyAgreement>();

            var domainTenancy = uhTenancy.ToDomain();

            domainTenancy.Tenure.Should().Contain(uhTenancy.UhTenureTypeId);
            domainTenancy.Tenure.Should().Contain(uhTenancy.UhTenureType.Description);
            domainTenancy.Agreement.Should().Contain(uhTenancy.UhAgreementTypeId);
            domainTenancy.Agreement.Should().Contain(uhTenancy.UhAgreementType.Description);
        }
    }
}
