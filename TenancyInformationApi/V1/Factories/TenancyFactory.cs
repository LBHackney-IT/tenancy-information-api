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
            var tenure = string.Join(": ",
                uhTenancyAgreement?.UhTenureTypeId,
                uhTenancyAgreement?.UhTenureType.Description);
            var agreement = string.Join(": ",
                uhTenancyAgreement?.UhAgreementTypeId,
                uhTenancyAgreement?.UhAgreementType.Description);

            return new Tenancy
            {
                TenancyReference = uhTenancyAgreement?.TenancyAgreementReference,
                CommencementOfTenancyDate = uhTenancyAgreement.CommencementOfTenancy,
                EndOfTenancyDate = uhTenancyAgreement.EndOfTenancy,
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
