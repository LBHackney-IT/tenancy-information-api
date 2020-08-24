using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenancyInformationApi.V1.Infrastructure;

namespace TenancyInformationApi.Tests.V1.Helper
{
    public static class TestHelper
    {
        public static UhTenancyAgreement CreateDatabaseTenancyEntity(string tenancyReference, string agreementTypeRef, string tenureTypeLookupId = null)
        {
            var fixture = new Fixture();
            var fp = fixture.Build<UhTenancyAgreement>()
                .Without(ta => ta.UhTenureType)
                .Create();
            fp.UhTenureTypeId = tenureTypeLookupId ?? fp.UhTenureTypeId;
            fp.TenancyAgreementReference = tenancyReference ?? fp.TenancyAgreementReference;
            fp.UhAgreementTypeId = Convert.ToChar(agreementTypeRef);
            return fp;
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
                .With(a => a.UhAgreementTypeId, fixture.Create<char>().ToString)
                .Create();
        }
    }
}
