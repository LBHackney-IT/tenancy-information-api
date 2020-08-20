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
        public static UhTenancyAgreement CreateDatabasePersonEntity( string tenancyReference )
        {
            var faker = new Fixture();
            var fp = faker.Create<UhTenancyAgreement>();
            fp.TenancyAgreementReference = tenancyReference;
            return fp;
        }





    }
}
