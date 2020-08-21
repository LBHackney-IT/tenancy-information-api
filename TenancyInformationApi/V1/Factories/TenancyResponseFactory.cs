using System.Globalization;
using TenancyInformationApi.V1.Boundary.Response;
using TenancyInformationApi.V1.Domain;

namespace TenancyInformationApi.V1.Factories
{
    public static class TenancyResponseFactory
    {
        public static TenancyInformationResponse ToResponse(this Tenancy tenancy)
        {
            return new TenancyInformationResponse
            {
                TenancyAgreementReference = tenancy?.TenancyReference,
                CommencementOfTenancyDate = tenancy?.CommencementOfTenancyDate,
                EndOfTenancyDate = tenancy?.EndOfTenancyDate,
                CurrentBalance = tenancy?.CurrentBalance?.ToString(CultureInfo.CurrentCulture),
                Present = tenancy?.Present.ToString(CultureInfo.CurrentCulture),
                Terminated = tenancy?.Terminated.ToString(CultureInfo.CurrentCulture),
                PaymentReference = tenancy?.PaymentReference,
                HouseholdReference = tenancy?.HouseholdReference,
                PropertyReference = tenancy?.PropertyReference,
                TenureType = tenancy?.Tenure,
                AgreementType = tenancy?.Agreement,
                Service = tenancy?.Service?.ToString(CultureInfo.CurrentCulture),
                OtherCharge = tenancy?.OtherCharge?.ToString(CultureInfo.CurrentCulture)
            };
        }
    }
}
