using System;
using System.Globalization;
using TenancyInformationApi.Tests.V1.Helper;
using TenancyInformationApi.V1.Boundary.Response;
using TenancyInformationApi.V1.Infrastructure;

namespace TenancyInformationApi.Tests
{
    public static class E2ETestHelper
    {
        public static TenancyInformationResponse AddPersonWithRelatedEntitiesToDb(UhContext context, string tenancyReference = null)
        {
            var agreementLookup = AddAgreementTypeToDatabase(context);
            var tenureTypeLookup = TestHelper.CreateTenureTypeLookup();
            var tenancyAgreement = TestHelper.CreateDatabaseTenancyEntity(tenancyReference, agreementLookup.UhAgreementTypeId, tenureTypeLookup.UhTenureTypeId);
            context.UhTenure.Add(tenureTypeLookup);
            context.SaveChanges();

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
                AgreementType = $"{tenancyAgreement.UhAgreementTypeId}: {agreementLookup?.Description}",
                TenureType = $"{tenureTypeLookup.UhTenureTypeId}: {tenureTypeLookup?.Description}",
            };
        }

        private static UhAgreementType AddAgreementTypeToDatabase(UhContext context)
        {
            var agreementLookup = TestHelper.CreateAgreementTypeLookup();
            context.UhTenancyAgreementsType.Add(agreementLookup);
            try
            {
                context.SaveChanges();
                return agreementLookup;
            }
            catch (InvalidOperationException)
            {
                return AddAgreementTypeToDatabase(context);
            }
        }
    }
}
