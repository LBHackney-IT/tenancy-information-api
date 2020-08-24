using System.Collections.Generic;
using System.Linq;
using TenancyInformationApi.V1.Domain;
using TenancyInformationApi.V1.Infrastructure;

namespace TenancyInformationApi.V1.Factories
{
    public static class TenancyFactory
    {
        public static Tenancy ToDomain(this UhTenancyAgreement uhTenancyAgreement, UhAgreementType agreementType)
        {
            return uhTenancyAgreement.ToDomain(agreementType, uhTenancyAgreement.UhTenureType);
        }

        public static Tenancy ToDomain(this UhTenancyAgreement uhTenancyAgreement, UhAgreementType agreementType, UhTenureType tenureType)
        {
            var tenure = tenureType?.UhTenureTypeId == null && tenureType?.Description == null
                ? null
                : $"{tenureType?.UhTenureTypeId}: {tenureType?.Description}";
            var agreement = agreementType?.UhAgreementTypeId == null
                ? null
                : $"{agreementType.UhAgreementTypeId.Trim()}: {agreementType.Description?.Trim()}";

            return new Tenancy
            {
                TenancyReference = uhTenancyAgreement?.TenancyAgreementReference?.Trim(),
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
                Agreement = agreement
            };
        }
    }
}
