using System.Collections.Generic;
using System.Globalization;
using TenancyInformationApi.Tests.V1.Helper;
using TenancyInformationApi.V1.Boundary.Response;
using TenancyInformationApi.V1.Infrastructure;

namespace TenancyInformationApi.Tests
{
    public static class E2ETestHelper
    {
        public static TenancyInformationResponse AddPersonWithRelatedEntitiesToDb(UhContext context,
            string tenancyReference = null, string agreementId = null, string tenureTypeId = null,
            string address = null, string postcode = null)
        {
            var agreementLookup = AddAgreementTypeToDatabase(context, agreementId);
            var tenureTypeLookup = AddTenureTypeToDatabase(context, tenureTypeId);

            var tenancyAgreement = TestHelper.CreateDatabaseTenancyEntity(tenancyReference, agreementLookup.UhAgreementTypeId, tenureTypeLookup.UhTenureTypeId);
            context.UhTenancyAgreements.Add(tenancyAgreement);
            context.SaveChanges();

            var property = TestHelper.CreateDatabaseProperty(tenancyAgreement.PropertyReference, address, postcode);
            context.UhProperties.Add(property);
            context.SaveChanges();

            var resident = TestHelper.CreateDatabaseResident(tenancyAgreement.HouseholdReference);
            context.UhResidents.Add(resident);
            context.SaveChanges();

            return new TenancyInformationResponse
            {
                TenancyAgreementReference = tenancyAgreement.TenancyAgreementReference,
                HouseholdReference = tenancyAgreement.HouseholdReference,
                PropertyReference = tenancyAgreement.PropertyReference,
                Address = property.AddressLine1,
                Postcode = property.Postcode,
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
                Residents = new List<Resident>{
                    new Resident
                    {
                        FirstName = resident.FirstName,
                        LastName = resident.LastName,
                        DateOfBirth = resident.DateOfBirth.ToString("yyyy-MM-dd")
                    }
                }
            };
        }

        private static UhAgreementType AddAgreementTypeToDatabase(UhContext context, string agreementId = null)
        {
            var agreementLookup = TestHelper.CreateAgreementTypeLookup();
            agreementLookup.UhAgreementTypeId = agreementId ?? agreementLookup.UhAgreementTypeId;
            context.UhTenancyAgreementsType.Add(agreementLookup);
            context.SaveChanges();
            return agreementLookup;
        }

        private static UhTenureType AddTenureTypeToDatabase(UhContext context, string tenureTypeId = null)
        {
            var tenureLookup = TestHelper.CreateTenureTypeLookup();
            tenureLookup.UhTenureTypeId = tenureTypeId ?? tenureLookup.UhTenureTypeId;
            context.UhTenure.Add(tenureLookup);
            context.SaveChanges();
            return tenureLookup;
        }
    }
}
