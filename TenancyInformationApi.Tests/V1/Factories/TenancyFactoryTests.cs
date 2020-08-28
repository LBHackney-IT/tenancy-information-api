using System;
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
        private readonly Fixture _fixture = new Fixture();

        [Test]
        public void EntityContainsAppropriateData()
        {
            var uhTenancy = _fixture.Create<UhTenancyAgreement>();
            var agreementTypeDescription = _fixture.Create<UhAgreementType>();
            var property = _fixture.Create<UHProperty>();
            var domainTenancy = uhTenancy.ToDomain(agreementTypeDescription, property);

            domainTenancy.Tenure.Should().Contain(uhTenancy.UhTenureType.UhTenureTypeId);
            domainTenancy.Tenure.Should().Contain(uhTenancy.UhTenureType.Description);
            domainTenancy.Agreement.Should().Contain(agreementTypeDescription.UhAgreementTypeId.ToString());
            domainTenancy.Agreement.Should().Contain(agreementTypeDescription.Description);
        }

        [Test]
        public void ToDomainCorrectlyFormatsData()
        {
            var uhTenancy = _fixture.Create<UhTenancyAgreement>();
            var property = _fixture.Create<UHProperty>();
            var agreementTypeDescription = _fixture.Create<UhAgreementType>();

            var tagRef = uhTenancy.TenancyAgreementReference;
            uhTenancy.TenancyAgreementReference = $" {tagRef}   ";

            var address = property.AddressLine1;
            property.AddressLine1 = $"   {address}  ";

            var domainTenancy = uhTenancy.ToDomain(agreementTypeDescription, property);
            var commencementDate = uhTenancy.CommencementOfTenancy;
            var endDate = uhTenancy.EndOfTenancy;

            domainTenancy.TenancyAgreementReference.Should().Be(tagRef);
            domainTenancy.CommencementOfTenancyDate.Should()
                .Be($"{commencementDate.Value.Year:0000}-{commencementDate.Value.Month:00}-{commencementDate.Value.Day:00}");
            domainTenancy.EndOfTenancyDate.Should()
                .Be($"{endDate.Value.Year:0000}-{endDate.Value.Month:00}-{endDate.Value.Day:00}");
            domainTenancy.Address.Should().BeEquivalentTo(address);
        }

        [Test]
        public void ToDomainWillMapAddressDataFromTheProperty()
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
            var property = new UHProperty
            {
                PropertyReference = "12333",
                AddressLine1 = "1 Hillman Road",
                Postcode = "E8 1JJ"
            };
            var domain = tenure.ToDomain(typeLookup, property);
            domain.Address.Should().Be("1 Hillman Road");
            domain.Postcode.Should().Be("E8 1JJ");
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
                TenancyAgreementReference = "12345/3",
                Agreement = "M: describing",
            });
        }

        [Test]
        public void ToDomainWillMapAResident()
        {
            var dbResident = new UHResident
            {
                FirstName = "first name",
                LastName = "last name",
                DateOfBirth = new DateTime(1980, 12, 28)
            };
            dbResident.ToDomain().Should().BeEquivalentTo(new Resident
            {
                FirstName = "first name",
                LastName = "last name",
                DateOfBirth = new DateTime(1980, 12, 28)
            });
        }

        [Test]
        public void ChangeDatesToNullWhereAppropriate()
        {
            var fixture = new Fixture();
            var nullDate = new DateTime(1900, 1, 1);
            var dbResident = fixture.Build<UHResident>()
                .With(r => r.DateOfBirth, nullDate)
                .Create();
            var dbTenancy = fixture.Build<UhTenancyAgreement>()
                .With(t => t.CommencementOfTenancy, nullDate)
                .With(t => t.EndOfTenancy, nullDate)
                .Create();
            var agreementTypeDescription = _fixture.Create<UhAgreementType>();
            var property = _fixture.Create<UHProperty>();

            dbResident.ToDomain().DateOfBirth.Should().BeNull();
            dbTenancy.ToDomain(agreementTypeDescription, property).CommencementOfTenancyDate.Should().BeNull();
            dbTenancy.ToDomain(agreementTypeDescription, property).EndOfTenancyDate.Should().BeNull();
        }
    }
}
