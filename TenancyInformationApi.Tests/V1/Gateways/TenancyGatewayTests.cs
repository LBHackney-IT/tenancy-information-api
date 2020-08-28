using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Bogus;
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
            CallGatewayWithArgs().Should().BeEmpty();
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

            var expectedResponses = savedEntities.Select(ExpectedDomain);

            CallGatewayWithArgs().Should().BeEquivalentTo(expectedResponses);
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

            CallGatewayWithArgs().First().Residents.Should().BeEquivalentTo(residents.ToDomain());
        }

        [Test]
        public void ListTenanciesOnlyReturnsResidentsForTheCorrectHouseReference()
        {
            var savedEntities = SaveTenancyPropertyAndLookups();

            var residentsOne = AddResidentToTheDatabase("diff house", 1);
            var residentTwo = AddResidentToTheDatabase(savedEntities.uhTenancy.HouseholdReference, 2);

            var listTenanciesResponse = CallGatewayWithArgs();
            listTenanciesResponse.First().Residents.Count.Should().Be(1);
            listTenanciesResponse.First().Residents.First().Should().BeEquivalentTo(residentTwo.ToDomain());
        }

        [Test]
        public void ListTenanciesWillReturnLimitNumberOfRecordsReturned()
        {
            SaveTenancyPropertyAndLookups(agreementLookupId: "a");
            SaveTenancyPropertyAndLookups(agreementLookupId: "b");
            SaveTenancyPropertyAndLookups(agreementLookupId: "c");

            var response = CallGatewayWithArgs(limit: 2);
            response.Count.Should().Be(2);
        }

        [Test]
        public void ListTenanciesWillReturnRecordsOrderedByTagRef()
        {
            var house2Tenancy2 = SaveTenancyPropertyAndLookups(tagRef: "45627/2", agreementLookupId: "a");
            var house2Tenancy1 = SaveTenancyPropertyAndLookups(tagRef: "45627/1", agreementLookupId: "b");
            var house1Tenancy1 = SaveTenancyPropertyAndLookups(tagRef: "12345/1", agreementLookupId: "c");

            var response = CallGatewayWithArgs(limit: 2);
            response.Count.Should().Be(2);
            response.First()
                .Should().BeEquivalentTo(ExpectedDomain(house1Tenancy1));
            response.Last()
                .Should().BeEquivalentTo(ExpectedDomain(house2Tenancy1));
        }

        [Test]
        public void ListTenanciesWillOffsetByGivenCursor()
        {
            var house2Tenancy2 = SaveTenancyPropertyAndLookups(tagRef: "45627/2", agreementLookupId: "a");
            var house2Tenancy1 = SaveTenancyPropertyAndLookups(tagRef: "45627/1", agreementLookupId: "b");
            var house1Tenancy1 = SaveTenancyPropertyAndLookups(tagRef: "12345/1", agreementLookupId: "c");
            var house3Tenancy1 = SaveTenancyPropertyAndLookups(tagRef: "77726/1", agreementLookupId: "e");

            var response = CallGatewayWithArgs(limit: 2, cursor: 456271);
            response.Count.Should().Be(2);
            response.First().Should().BeEquivalentTo(ExpectedDomain(house2Tenancy2));
            response.Last().Should().BeEquivalentTo(ExpectedDomain(house3Tenancy1));
        }

        [Test]
        public void ListTenanciesWhenSearchingAddressesWillReturnRecordsWithAMatchingAddress()
        {
            var addressQuery = "23 Lowman Drive";
            var tenancyAtAddress = SaveTenancyPropertyAndLookups(address: addressQuery, agreementLookupId: "a");
            var anotherTenancy = SaveTenancyPropertyAndLookups(agreementLookupId: "b");

            var response = CallGatewayWithArgs(addressQuery: addressQuery);
            response.Count.Should().Be(1);
            response.First().Should().BeEquivalentTo(ExpectedDomain(tenancyAtAddress));
        }

        [Test]
        public void ListTenanciesWhenSearchingAddressesWillIgnoreCasingAndFormatting()
        {
            var addressQuery = "23 Lowman Drive";
            var tenancyAtAddress = SaveTenancyPropertyAndLookups(address: "23lowmanDrive", agreementLookupId: "a");
            var anotherTenancy = SaveTenancyPropertyAndLookups(agreementLookupId: "b");

            var response = CallGatewayWithArgs(addressQuery: addressQuery);
            response.Count.Should().Be(1);
            response.First().Should().BeEquivalentTo(ExpectedDomain(tenancyAtAddress));
        }

        [Test]
        public void ListTenanciesWhenSearchingAddressesWillReturnRecordsWithAPartiallyMatchingAddress()
        {
            var addressQuery = "lowman drive";
            var tenancyAtAddress = SaveTenancyPropertyAndLookups(address: "23 Lowman Drive", agreementLookupId: "a");
            var anotherTenancy = SaveTenancyPropertyAndLookups(agreementLookupId: "b");

            var response = CallGatewayWithArgs(addressQuery: addressQuery);
            response.Count.Should().Be(1);
            response.First().Should().BeEquivalentTo(ExpectedDomain(tenancyAtAddress));
        }

        [Test]
        public void ListTenanciesWhenSearchingAddressesWillReturnRecordsWithAMatchingPostcode()
        {
            var addressQuery = "e3 gt6";
            var tenancyAtAddress = SaveTenancyPropertyAndLookups(postcode: "E3 Gt6", agreementLookupId: "a");
            var anotherTenancy = SaveTenancyPropertyAndLookups(agreementLookupId: "b");

            var response = CallGatewayWithArgs(addressQuery: addressQuery);
            response.Count.Should().Be(1);
            response.First().Should().BeEquivalentTo(ExpectedDomain(tenancyAtAddress));
        }

        [Test]
        public void ListTenanciesWhenSearchingByPostcodeWillIgnoreCasingAndFormatting()
        {
            var postcode = "E3 GT6";
            var tenancyAtAddress = SaveTenancyPropertyAndLookups(postcode: "e3  gT6", agreementLookupId: "a");
            var anotherTenancy = SaveTenancyPropertyAndLookups(agreementLookupId: "b");

            var response = CallGatewayWithArgs(postcodeQuery: postcode);
            response.Count.Should().Be(1);
            response.First().Should().BeEquivalentTo(ExpectedDomain(tenancyAtAddress));
        }

        [Test]
        public void ListTenanciesWhenSearchingByPostcodeWillReturnRecordsWithAnExactlyMatchingPostcode()
        {
            var postcode = "W1 YU2";
            var tenancyAtAddress = SaveTenancyPropertyAndLookups(postcode: postcode, agreementLookupId: "a");
            var anotherTenancy = SaveTenancyPropertyAndLookups(agreementLookupId: "b");

            var response = CallGatewayWithArgs(postcodeQuery: postcode);
            response.Count.Should().Be(1);
            response.First().Should().BeEquivalentTo(ExpectedDomain(tenancyAtAddress));
        }

        [Test]
        public void ListTenanciesWhenAskingForOnlyLeaseholdsOnlyLeaseholdsAreReturned()
        {
            var leaseholdTenancy = SaveTenancyPropertyAndLookups(tenureTypeId: "LEA", agreementLookupId: "a");
            var anotherTenancy = SaveTenancyPropertyAndLookups(tenureTypeId: "PVG", agreementLookupId: "b");

            var response = CallGatewayWithArgs(leaseholdsOnly: true);
            response.Count.Should().Be(1);
            response.First().Should().BeEquivalentTo(ExpectedDomain(leaseholdTenancy));
        }

        [Test]
        public void ListTenanciesWhenAskingForOnlyFreeholdsOnlyFreeholdsAreReturned()
        {
            var freeholdTenancy = SaveTenancyPropertyAndLookups(tagRef: "12345/2", tenureTypeId: "FRE", agreementLookupId: "a");
            var secondFreeholdTenancy = SaveTenancyPropertyAndLookups(tagRef: "52345/2", tenureTypeId: "FRS", agreementLookupId: "b");
            var anotherTenancy = SaveTenancyPropertyAndLookups(tenureTypeId: "PVG", agreementLookupId: "c");

            var response = CallGatewayWithArgs(freeholdsOnly: true);
            response.Count.Should().Be(2);
            response.First().Should().BeEquivalentTo(ExpectedDomain(freeholdTenancy));
            response.Last().Should().BeEquivalentTo(ExpectedDomain(secondFreeholdTenancy));
        }

        [Test]
        public void ListTenanciesWillIgnoreNotRealTagRefs()
        {
            var fakeTagRefs = new List<string>
            {
                "SUSP/LEGLEA",
                "SUSP/LEGRNT",
                "SSSSSS",
                "YYYYYY",
                "ZZZZZZ",
                "DUMMY/Z001",
                "DUMMY/Z536"
            };
            fakeTagRefs.ForEach(t => SaveTenancyPropertyAndLookups(tagRef: t, agreementLookupId: t.Last().ToString()));
            var legitimateTenancy = SaveTenancyPropertyAndLookups("12367/2", "z");

            var response = CallGatewayWithArgs();
            response.Count.Should().Be(1);
            response.First().Should().BeEquivalentTo(ExpectedDomain(legitimateTenancy));
        }

        [Test]
        public void ListTenanciesWillRemoveTheZsFromTagRefsFromThePurposesOfPagination()
        {
            var house2Tenancy2 = SaveTenancyPropertyAndLookups(tagRef: "45627/Z002", agreementLookupId: "a");
            var house2Tenancy1 = SaveTenancyPropertyAndLookups(tagRef: "45627/Z001", agreementLookupId: "b");
            var house1Tenancy1 = SaveTenancyPropertyAndLookups(tagRef: "12345/Z001", agreementLookupId: "c");

            var response = CallGatewayWithArgs(limit: 2);
            response.Count.Should().Be(2);
            response.First()
                .Should().BeEquivalentTo(ExpectedDomain(house1Tenancy1));
            response.Last()
                .Should().BeEquivalentTo(ExpectedDomain(house2Tenancy1));
        }

        private static Tenancy ExpectedDomain((UhTenancyAgreement uhTenancy, UhTenureType tenureTypeLookup, UhAgreementType agreementTypeLookup, UHProperty property) tenancyEntities)
        {
            var expectedDomain = tenancyEntities.uhTenancy.ToDomain(tenancyEntities.agreementTypeLookup, tenancyEntities.property);
            expectedDomain.Residents = new List<Resident>();
            return expectedDomain;
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

        private (UhTenancyAgreement uhTenancy, UhTenureType tenureTypeLookup, UhAgreementType agreementTypeLookup,
            UHProperty property) SaveTenancyPropertyAndLookups(string tagRef = null, string agreementLookupId = null, string tenureTypeId = null,
                string address = null, string postcode = null)
        {
            var faker = new Faker();
            tagRef ??= $"{faker.Random.Int(0, 99999)}/{faker.Random.Int(0, 99)}";
            var tenureTypeLookup = AddTenureTypeLookupToDatabase(tenureTypeId);
            var agreementTypeLookup = AddAgreementTypeLookupToDatabase(agreementLookupId);
            var uhTenancy = AddTenancyAgreementToDatabase(tagRef, agreementTypeLookup, tenureTypeLookup);
            var property = AddPropertyToDatabase(uhTenancy.PropertyReference, address, postcode);
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

        private UhTenureType AddTenureTypeLookupToDatabase(string tenureTypeId)
        {
            var tenureTypeLookup = _fixture.Create<UhTenureType>();
            if (tenureTypeId != null) tenureTypeLookup.UhTenureTypeId = tenureTypeId;
            UhContext.UhTenure.Add(tenureTypeLookup);
            UhContext.SaveChanges();
            return tenureTypeLookup;
        }

        private UHProperty AddPropertyToDatabase(string propertyReference, string address, string postcode)
        {
            var property = TestHelper.CreateDatabaseProperty(propertyReference, address, postcode);

            UhContext.UhProperties.Add(property);
            UhContext.SaveChanges();
            return property;
        }

        private List<Tenancy> CallGatewayWithArgs(int limit = 20, int cursor = 0, string addressQuery = null, string postcodeQuery = null, bool leaseholdsOnly = false, bool freeholdsOnly = false)
        {
            return _classUnderTest.ListTenancies(limit, cursor, addressQuery, postcodeQuery, leaseholdsOnly, freeholdsOnly);
        }
    }
}
