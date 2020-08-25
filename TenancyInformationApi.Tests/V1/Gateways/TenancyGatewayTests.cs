using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using TenancyInformationApi.V1.Gateways;
using FluentAssertions;
using NUnit.Framework;
using TenancyInformationApi.Tests.V1.Helper;
using TenancyInformationApi.V1.Domain;
using TenancyInformationApi.V1.Factories;
using TenancyInformationApi.V1.Infrastructure;

namespace TenancyInformationApi.Tests.V1.Gateways
{
    [TestFixture]
    public class TenancyGatewayTests : UhTests
    {
        private readonly Fixture _fixture = new Fixture();
        private TenancyGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new TenancyGateway(UhContext);
        }

        [Test]
        public void GetByIdReturnsNullIfEntityDoesntExist()
        {
            var response = _classUnderTest.GetById("123/2");

            response.Should().BeNull();
        }

        [Test]
        public void GetByIdReturnsTheEntityIfItExists()
        {
            var tagRef = _fixture.Create<string>().Substring(0, 11);
            var (uhTenancy, _, agreementTypeLookup) = SaveTenancyAndLookups(tagRef);

            var expected = uhTenancy.ToDomain(agreementTypeLookup);
            var response = _classUnderTest.GetById(uhTenancy.TenancyAgreementReference);

            response.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void GetByIdWillOnlyGetLookupValuesForZAGTypeLookups()
        {
            var tagRef = _fixture.Create<string>().Substring(0, 11);
            var (uhTenancy, _, agreementTypeLookup) = SaveTenancyAndLookups(tagRef);

            var nonZagAgreementTypeLookup = _fixture.Build<UhAgreementType>()
                .With(a => a.LookupType, "TAG")
                .With(a => a.UhAgreementTypeId, agreementTypeLookup.UhAgreementTypeId)
                .Create();
            UhContext.UhTenancyAgreementsType.Add(nonZagAgreementTypeLookup);
            UhContext.SaveChanges();

            var expected = uhTenancy.ToDomain(agreementTypeLookup);
            var response = _classUnderTest.GetById(uhTenancy.TenancyAgreementReference);

            response.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ListTenanciesWithNoSavedTenanciesReturnsAnEmptyList()
        {
            _classUnderTest.ListTenancies().Should().BeEmpty();
        }

        [Test]
        public void ListTenanciesGivenNoQueryParametersWillReturnAllSavedTenancies()
        {
            var savedEntities = new List<(UhTenancyAgreement uhTenancy, UhTenureType tenureTypeLookup, UhAgreementType agreementTypeLookup)>
            {
                SaveTenancyAndLookups(),
                SaveTenancyAndLookups(),
                SaveTenancyAndLookups()
            };

            var expectedResponses = savedEntities.Select(x => x.uhTenancy.ToDomain(x.agreementTypeLookup));
            _classUnderTest.ListTenancies().Should().BeEquivalentTo(expectedResponses);
        }

        private (UhTenancyAgreement uhTenancy, UhTenureType tenureTypeLookup, UhAgreementType agreementTypeLookup) SaveTenancyAndLookups(string tagRef = null)
        {
            tagRef ??= _fixture.Create<string>().Substring(0, 11);
            var tenureTypeLookup = AddTenureTypeLookupToDatabase();
            var agreementTypeLookup = AddAgreementTypeLookupToDatabase();
            var uhTenancy = AddTenancyAgreementToDatabase(tagRef, agreementTypeLookup, tenureTypeLookup);
            return (uhTenancy, tenureTypeLookup, agreementTypeLookup);
        }

        private UhTenancyAgreement AddTenancyAgreementToDatabase(string tagRef, UhAgreementType agreementTypeLookup,
            UhTenureType tenureTypeLookup)
        {
            var uhTenancy = TestHelper.CreateDatabaseTenancyEntity(tagRef, agreementTypeLookup.UhAgreementTypeId,
                tenureTypeLookup.UhTenureTypeId);

            UhContext.UhTenancyAgreements.Add(uhTenancy);
            UhContext.SaveChanges();
            return uhTenancy;
        }

        private UhAgreementType AddAgreementTypeLookupToDatabase()
        {
            var agreementTypeLookup = TestHelper.CreateAgreementTypeLookup();
            UhContext.UhTenancyAgreementsType.Add(agreementTypeLookup);
            UhContext.SaveChanges();
            return agreementTypeLookup;
        }

        private UhTenureType AddTenureTypeLookupToDatabase()
        {
            var tenureTypeLookup = _fixture.Create<UhTenureType>();
            UhContext.UhTenure.Add(tenureTypeLookup);
            UhContext.SaveChanges();
            return tenureTypeLookup;
        }
    }
}
