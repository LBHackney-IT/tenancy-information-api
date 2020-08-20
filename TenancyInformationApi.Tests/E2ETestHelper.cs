using AutoFixture;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TenancyInformationApi.Tests.V1.Helper;
using TenancyInformationApi.V1.Boundary.Response;
using TenancyInformationApi.V1.Factories;
using TenancyInformationApi.V1.Infrastructure;

namespace TenancyInformationApi.Tests
{
    public static class E2ETestHelper
    {

        public static TenancyInformationResponse AddPersonWithRelatedEntitiestoDb(UhContext context, string tenancyReference)
        {
            var fixture = new Fixture();
            var resident = TestHelper.CreateDatabasePersonEntity(tenancyReference);
            context.UhTenancyAgreements.Add(resident);
            context.SaveChanges();

            //var tenancyAgreement = new Fixture().Create<UhTenancyAgreement>();

            //tenancyAgreement.TenancyAgreementReference = tenancyReference;

            //var tenureType = new UhTenureType { UhTenureTypeId = fixture.Create<string>(), Description = fixture.Create<string>()};
            //var agreementType = new UhAgreementType { UhAgreementTypeId = fixture.Create<string>(), Description = fixture.Create<string>() };
            //context.UhTenancyAgreements.Add(tenancyAgreement);
            //context.SaveChanges();

            //return resident.ToDomain().ToResponse();
            //var tenAgreement = tenancyAgreement.ToDomain();

            return new TenancyInformationResponse
            {
                TenancyAgreementReference = resident.TenancyAgreementReference,
                HouseholdReference = resident.HouseholdReference,
                PropertyReference = resident.PropertyReference,
                PaymentReference = resident.PaymentReference,
                CommencementOfTenancyDate = resident?.CommencementOfTenancy.ToString(CultureInfo.CurrentCulture),
                EndOfTenancyDate = resident?.EndOfTenancy.ToString(CultureInfo.CurrentCulture),
                CurrentBalance = resident?.CurrentRentBalance.ToString(CultureInfo.CurrentCulture),
                Present = resident?.IsPresent.ToString(CultureInfo.CurrentCulture),
                Terminated = resident?.IsTerminated.ToString(CultureInfo.CurrentCulture),
                Service = resident?.ServiceCharge.ToString(CultureInfo.CurrentCulture),
                OtherCharge = resident?.OtherCharges.ToString(CultureInfo.CurrentCulture),
                AgreementType = "agreement",
                TenureType = "tenure"
                    
            };

        }
    }
}

