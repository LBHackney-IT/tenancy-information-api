using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Bogus;
using TenancyInformationApi.V1.Gateways;
using FluentAssertions;
using FluentAssertions.Equivalency;
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
            var (uhTenancy, _, agreementTypeLookup, property) = SaveTenancyPropertyAndLookups(tagRef);

            var expected = uhTenancy.ToDomain(agreementTypeLookup);
            var response = _classUnderTest.GetById(uhTenancy.TenancyAgreementReference);

            response.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void GetByIdWillOnlyGetLookupValuesForZAGTypeLookups()
        {
            var tagRef = _fixture.Create<string>().Substring(0, 11);
            var (uhTenancy, _, agreementTypeLookup, property) = SaveTenancyPropertyAndLookups(tagRef);

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
            _classUnderTest.ListTenancies(20, 0).Should().BeEmpty();
        }

        [Test]
        public void ListTenanciesGivenNoQueryParametersWillReturnAllSavedTenancies()
        {
            var savedEntities = new List<(UhTenancyAgreement uhTenancy, UhTenureType tenureTypeLookup, UhAgreementType agreementTypeLookup, UHProperty property)>
            {
                SaveTenancyPropertyAndLookups(agreementLookupId: "a"),
                SaveTenancyPropertyAndLookups(agreementLookupId: "b"),
                SaveTenancyPropertyAndLookups(agreementLookupId: "c")
            };

            var expectedResponses = savedEntities.Select(x =>
                x.uhTenancy.ToDomain(x.agreementTypeLookup, x.property));
            _classUnderTest.ListTenancies(20, 0).Should().BeEquivalentTo(expectedResponses, _ignoreResidents);
        }

        [Test]
        public void ListTenanciesReturnsAListOfResidentsForATenancy()
        {
            var savedEntities = SaveTenancyPropertyAndLookups();

            var residents = new List<UHResident>
            {
                AddResidentToTheDatabase(savedEntities.uhTenancy.HouseholdReference, 1),
                AddResidentToTheDatabase(savedEntities.uhTenancy.HouseholdReference, 2)
            };

            _classUnderTest.ListTenancies(20, 0).First().Residents.Should().BeEquivalentTo(residents.ToDomain());
        }

        [Test]
        public void ListTenanciesOnlyReturnsResidentsForTheCorrectHouseReference()
        {
            var savedEntities = SaveTenancyPropertyAndLookups();

            var residentsOne = AddResidentToTheDatabase("diff house", 1);
            var residentTwo = AddResidentToTheDatabase(savedEntities.uhTenancy.HouseholdReference, 2);

            var listTenanciesResponse = _classUnderTest.ListTenancies(20, 0);
            listTenanciesResponse.First().Residents.Count.Should().Be(1);
            listTenanciesResponse.First().Residents.First().Should().BeEquivalentTo(residentTwo.ToDomain());
        }

        [Test]
        public void ListTenanciesWillReturnLimitNumberOfRecordsReturned()
        {
            SaveTenancyPropertyAndLookups(agreementLookupId: "a");
            SaveTenancyPropertyAndLookups(agreementLookupId: "b");
            SaveTenancyPropertyAndLookups(agreementLookupId: "c");

            var response = _classUnderTest.ListTenancies(2, 0);
            response.Count.Should().Be(2);
        }

        [Test]
        public void ListTenanciesWillReturnRecordsOrderedByTagRef()
        {
            var house2Tenancy2 = SaveTenancyPropertyAndLookups(tagRef: "45627/2", agreementLookupId: "a");
            var house2Tenancy1 = SaveTenancyPropertyAndLookups(tagRef: "45627/1", agreementLookupId: "b");
            var house1Tenancy1 = SaveTenancyPropertyAndLookups(tagRef: "12345/1", agreementLookupId: "c");

            var response = _classUnderTest.ListTenancies(2, 0);
            response.Count.Should().Be(2);
            response.First()
                .Should().BeEquivalentTo(ExpectedDomain(house1Tenancy1), _ignoreResidents);
            response.Last()
                .Should().BeEquivalentTo(ExpectedDomain(house2Tenancy1), _ignoreResidents);
        }

        [Test]
        public void ListTenanciesWillOffsetByGivenCursor()
        {
            var house2Tenancy2 = SaveTenancyPropertyAndLookups(tagRef: "45627/2", agreementLookupId: "a");
            var house2Tenancy1 = SaveTenancyPropertyAndLookups(tagRef: "45627/1", agreementLookupId: "b");
            var house1Tenancy1 = SaveTenancyPropertyAndLookups(tagRef: "12345/1", agreementLookupId: "c");
            var house3Tenancy1 = SaveTenancyPropertyAndLookups(tagRef: "77726/1", agreementLookupId: "e");

            var response = _classUnderTest.ListTenancies(2, 456271);
            response.Count.Should().Be(2);
            response.First().Should().BeEquivalentTo(ExpectedDomain(house2Tenancy2), _ignoreResidents);
            response.Last().Should().BeEquivalentTo(ExpectedDomain(house3Tenancy1), _ignoreResidents);
        }

        private static readonly Func<EquivalencyAssertionOptions<Tenancy>, EquivalencyAssertionOptions<Tenancy>> _ignoreResidents =
            options => options.Excluding(t => t.Residents);

        private static Tenancy ExpectedDomain((UhTenancyAgreement uhTenancy, UhTenureType tenureTypeLookup, UhAgreementType agreementTypeLookup, UHProperty property) tenancyEntities)
        {
            return tenancyEntities.uhTenancy.ToDomain(tenancyEntities.agreementTypeLookup, tenancyEntities.property);
        }

        private UHResident AddResidentToTheDatabase(string houseReference, int personNumber)
        {
            var residentOne = _fixture.Build<UHResident>()
                .With(r => r.HouseReference, houseReference)
                .With(r => r.PersonNumber, personNumber)
                .Create();
            UhContext.UhResidents.Add(residentOne);
            UhContext.SaveChanges();
            return residentOne;
        }

        private (UhTenancyAgreement uhTenancy, UhTenureType tenureTypeLookup, UhAgreementType agreementTypeLookup, UHProperty property) SaveTenancyPropertyAndLookups(string tagRef = null, string agreementLookupId = null)
        {
            var faker = new Faker();
            tagRef ??= $"{faker.Random.Int(0, 99999)}/{faker.Random.Int(0, 99)}";
            var tenureTypeLookup = AddTenureTypeLookupToDatabase();
            var agreementTypeLookup = AddAgreementTypeLookupToDatabase(agreementLookupId);
            var uhTenancy = AddTenancyAgreementToDatabase(tagRef, agreementTypeLookup, tenureTypeLookup);
            var property = AddPropertyToDatabase(uhTenancy.PropertyReference);
            return (uhTenancy, tenureTypeLookup, agreementTypeLookup, property);
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

        private UhAgreementType AddAgreementTypeLookupToDatabase(string agreementLookupId)
        {
            var agreementTypeLookup = TestHelper.CreateAgreementTypeLookup();
            if (agreementLookupId != null)
            {
                agreementTypeLookup.UhAgreementTypeId = agreementLookupId;
            }
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

        private UHProperty AddPropertyToDatabase(string propertyReference)
        {
            var property = TestHelper.CreateDatabaseProperty(propertyReference);
            UhContext.UhProperties.Add(property);
            UhContext.SaveChanges();
            return property;
        }
    }
}
