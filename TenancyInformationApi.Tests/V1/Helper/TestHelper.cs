using AutoFixture;
using System.Linq;
using TenancyInformationApi.V1.Infrastructure;

namespace TenancyInformationApi.Tests.V1.Helper
{
    public static class TestHelper
    {
        public static UhTenancyAgreement CreateDatabaseTenancyEntity(string tenancyReference, string agreementTypeRef,
            string propertyReference, string tenureTypeLookupId = null)
        {
            var fixture = new Fixture();
            var fp = fixture.Build<UhTenancyAgreement>()
                .Without(ta => ta.UhTenureType)
                .Without(ta => ta.UhProperty)
                .Create();
            fp.UhTenureTypeId = tenureTypeLookupId ?? fp.UhTenureTypeId;
            fp.TenancyAgreementReference = tenancyReference ?? fp.TenancyAgreementReference;
            fp.PropertyReference = propertyReference ?? fp.PropertyReference;
            fp.UhAgreementTypeId = agreementTypeRef.First();
            return fp;
        }

        public static UHProperty CreateDatabaseProperty(string propertyReference = null, string address = null, string postcode = null)
        {
            var fixture = new Fixture();
            var property = fixture.Build<UHProperty>()
                .With(p => p.PropertyReference, propertyReference)
                .Create();
            if (address != null) property.AddressLine1 = address;
            if (postcode != null) property.Postcode = postcode;
            return property;
        }

        public static UHResident CreateDatabaseResident(string houseReference = null)
        {
            var fixture = new Fixture();
            return fixture.Build<UHResident>()
                .With(r => r.HouseReference, houseReference ?? fixture.Create<string>())
                .Create();
        }

        public static UhTenureType CreateTenureTypeLookup()
        {
            var fixture = new Fixture();
            return fixture.Create<UhTenureType>();
        }

        public static UhAgreementType CreateAgreementTypeLookup()
        {
            var fixture = new Fixture();
            return fixture.Build<UhAgreementType>()
                .With(a => a.LookupType, "ZAG")
                .With(a => a.UhAgreementTypeId, fixture.Create<string>().First().ToString)
                .Create();
        }
    }
}
