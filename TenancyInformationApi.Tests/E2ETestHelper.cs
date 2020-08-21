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

        public static TenancyInformationResponse AddPersonWithRelatedEntitiesToDb(UhContext context, string tenancyReference)
        {
            var tenancyAgreement = TestHelper.CreateDatabaseTenancyEntity(tenancyReference);
            context.UhTenure.Add(tenancyAgreement.UhTenureType);
            context.UhTenancyAgreementsType.Add(tenancyAgreement.UhAgreementType);
            context.SaveChanges();
            tenancyAgreement.UhTenureType = null;
            tenancyAgreement.UhAgreementType = null;

            context.UhTenancyAgreements.Add(tenancyAgreement);
            context.SaveChanges();

            return new TenancyInformationResponse
            {
                TenancyAgreementReference = tenancyAgreement.TenancyAgreementReference,
                HouseholdReference = tenancyAgreement.HouseholdReference,
                PropertyReference = tenancyAgreement.PropertyReference,
                PaymentReference = tenancyAgreement.PaymentReference,
                CommencementOfTenancyDate = tenancyAgreement.CommencementOfTenancy?.ToString("yyyy-MM-dd"),
                EndOfTenancyDate = tenancyAgreement.EndOfTenancy?.ToString("yyyy-MM-dd"),
                CurrentBalance = tenancyAgreement.CurrentRentBalance?.ToString(CultureInfo.CurrentCulture),
                Present = tenancyAgreement.IsPresent.ToString(CultureInfo.CurrentCulture),
                Terminated = tenancyAgreement.IsTerminated.ToString(CultureInfo.CurrentCulture),
                Service = tenancyAgreement.ServiceCharge?.ToString(CultureInfo.CurrentCulture),
                OtherCharge = tenancyAgreement.OtherCharges?.ToString(CultureInfo.CurrentCulture),
                AgreementType = $"{tenancyAgreement.UhAgreementTypeId}: {tenancyAgreement.UhAgreementType?.Description}",
                TenureType = $"{tenancyAgreement.UhTenureTypeId}: {tenancyAgreement.UhTenureType?.Description}",


            };

        }
    }
}

