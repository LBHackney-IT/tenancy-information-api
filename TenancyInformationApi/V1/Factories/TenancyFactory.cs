using System.Collections.Generic;
using System.Linq;
using TenancyInformationApi.V1.Domain;
using TenancyInformationApi.V1.Infrastructure;

namespace TenancyInformationApi.V1.Factories
{
    public static class TenancyFactory
    {
        public static Tenancy ToDomain(this UhTenancyAgreement uhTenancyAgreement)
        {
            var tenure = uhTenancyAgreement?.UhTenureTypeId == null && uhTenancyAgreement?.UhTenureType?.Description == null
                ? null
                : $"{uhTenancyAgreement?.UhTenureTypeId}: {uhTenancyAgreement?.UhTenureType?.Description}";
            var agreement = uhTenancyAgreement?.UhAgreementTypeId == null && uhTenancyAgreement?.UhAgreementType?.Description == null
                ? null
                : $"{uhTenancyAgreement?.UhAgreementTypeId}: {uhTenancyAgreement?.UhAgreementType?.Description}";

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

        public static List<Tenancy> ToDomain(this IEnumerable<UhTenancyAgreement> uhTenancyAgreements) =>
            uhTenancyAgreements.Select(tenancy => tenancy.ToDomain()).ToList();
    }
}
