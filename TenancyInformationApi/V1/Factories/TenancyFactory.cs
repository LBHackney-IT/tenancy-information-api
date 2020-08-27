using System.Collections.Generic;
using System.Linq;
using TenancyInformationApi.V1.Domain;
using TenancyInformationApi.V1.Infrastructure;

namespace TenancyInformationApi.V1.Factories
{
    public static class TenancyFactory
    {
        public static Tenancy ToDomain(this UhTenancyAgreement uhTenancyAgreement, UhAgreementType agreementType, UHProperty property = null)
        {
            return uhTenancyAgreement.ToDomain(agreementType, uhTenancyAgreement.UhTenureType, property);
        }

        public static List<Resident> ToDomain(this IEnumerable<UHResident> residents)
        {
            return residents.Select(r => r.ToDomain()).ToList();
        }

        public static Resident ToDomain(this UHResident resident)
        {
            return new Resident
            {
                FirstName = resident.FirstName,
                LastName = resident.LastName,
                DateOfBirth = resident.DateOfBirth
            };
        }

        public static Tenancy ToDomain(this UhTenancyAgreement uhTenancyAgreement, UhAgreementType agreementType, UhTenureType tenureType, UHProperty property = null)
        {
            var tenure = tenureType == null
                ? null
                : $"{tenureType.UhTenureTypeId.Trim()}: {tenureType.Description.Trim()}";
            var agreement = agreementType?.UhAgreementTypeId == null
                ? null
                : $"{agreementType.UhAgreementTypeId.Trim()}: {agreementType.Description?.Trim()}";

            return new Tenancy
            {
                TenancyAgreementReference = uhTenancyAgreement?.TenancyAgreementReference?.Trim(),
                CommencementOfTenancyDate = uhTenancyAgreement.CommencementOfTenancy?.ToString("yyyy-MM-dd"),
                EndOfTenancyDate = uhTenancyAgreement.EndOfTenancy?.ToString("yyyy-MM-dd"),
                CurrentBalance = uhTenancyAgreement.CurrentRentBalance,
                Present = uhTenancyAgreement.IsPresent,
                Terminated = uhTenancyAgreement.IsTerminated,
                PaymentReference = uhTenancyAgreement.PaymentReference,
                HouseholdReference = uhTenancyAgreement.HouseholdReference,
                PropertyReference = uhTenancyAgreement.PropertyReference,
                Service = uhTenancyAgreement.ServiceCharge,
                OtherCharge = uhTenancyAgreement.OtherCharges,
                Tenure = tenure,
                Agreement = agreement,
                Address = property?.AddressLine1.Trim(),
                Postcode = property?.Postcode.Trim()
            };
        }
    }
}
