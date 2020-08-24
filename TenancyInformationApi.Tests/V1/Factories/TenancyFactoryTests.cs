using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using TenancyInformationApi.V1.Domain;
using TenancyInformationApi.V1.Factories;
using TenancyInformationApi.V1.Infrastructure;

namespace TenancyInformationApi.Tests.V1.Factories
{
    [TestFixture]
    public class TenancyFactoryTests
    {
        private Fixture _fixture = new Fixture();

        [Test]
        public void EntityContainsAppropriateData()
        {
            var uhTenancy = _fixture.Create<UhTenancyAgreement>();
            var agreementTypeDescription = _fixture.Create<UhAgreementType>();
            var domainTenancy = uhTenancy.ToDomain(agreementTypeDescription);

            domainTenancy.Tenure.Should().Contain(uhTenancy.UhTenureTypeId);
            domainTenancy.Tenure.Should().Contain(uhTenancy.UhTenureType.Description);
            domainTenancy.Agreement.Should().Contain(agreementTypeDescription.UhAgreementTypeId.ToString());
            domainTenancy.Agreement.Should().Contain(agreementTypeDescription.Description);
        }

        [Test]
        public void ToDomainCorrectlyFormatsData()
        {
            var uhTenancy = _fixture.Create<UhTenancyAgreement>();
            var agreementTypeDescription = _fixture.Create<UhAgreementType>();

            var tagRef = uhTenancy.TenancyAgreementReference;
            uhTenancy.TenancyAgreementReference = $" {tagRef}   ";

            var domainTenancy = uhTenancy.ToDomain(agreementTypeDescription);
            var commencementDate = uhTenancy.CommencementOfTenancy;
            var endDate = uhTenancy.EndOfTenancy;

            domainTenancy.TenancyReference.Should().Be(tagRef);
            domainTenancy.CommencementOfTenancyDate.Should()
                .Be($"{commencementDate.Value.Year:0000}-{commencementDate.Value.Month:00}-{commencementDate.Value.Day:00}");
            domainTenancy.EndOfTenancyDate.Should()
                .Be($"{endDate.Value.Year:0000}-{endDate.Value.Month:00}-{endDate.Value.Day:00}");
        }

        [Test]
        public void ToDomainWillMapDataWithOnlyMinimalRequiredData()
        {
            var tenure = new UhTenancyAgreement
            {
                IsPresent = true,
                IsTerminated = false,
                TenancyAgreementReference = "12345/3"
            };
            var typeLookup = new UhAgreementType
            {
                UhAgreementTypeId = "M",
                Description = "describing"
            };

            tenure.ToDomain(typeLookup).Should().BeEquivalentTo(new Tenancy
            {
                Present = true,
                Terminated = false,
                TenancyReference = "12345/3",
                Agreement = "M: describing"
            });
        }
    }
}
