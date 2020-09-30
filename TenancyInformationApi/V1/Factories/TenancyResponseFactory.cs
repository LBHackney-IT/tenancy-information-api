using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TenancyInformationApi.V1.Boundary.Response;
using TenancyInformationApi.V1.Domain;
using Resident = TenancyInformationApi.V1.Boundary.Response.Resident;

namespace TenancyInformationApi.V1.Factories
{
    public static class TenancyResponseFactory
    {
        public static TenancyInformationResponse ToResponse(this Tenancy tenancy)
        {
            if (tenancy == null) return null;
            return new TenancyInformationResponse
            {
                TenancyAgreementReference = tenancy.TenancyAgreementReference,
                CommencementOfTenancyDate = tenancy.CommencementOfTenancyDate,
                EndOfTenancyDate = tenancy.EndOfTenancyDate,
                CurrentBalance = tenancy.CurrentBalance?.ToString(CultureInfo.CurrentCulture),
                Present = tenancy.Present,
                Terminated = tenancy.Terminated,
                PaymentReference = tenancy.PaymentReference,
                HouseholdReference = tenancy.HouseholdReference,
                PropertyReference = tenancy.PropertyReference,
                TenureType = tenancy.Tenure,
                AgreementType = tenancy.Agreement,
                Service = tenancy.Service?.ToString(CultureInfo.CurrentCulture),
                OtherCharge = tenancy.OtherCharge?.ToString(CultureInfo.CurrentCulture),
                Residents = tenancy.Residents?.ToResponse(),
                Address = tenancy.Address,
                Postcode = tenancy.Postcode
            };
        }

        public static List<TenancyInformationResponse> ToResponse(this IEnumerable<Tenancy> tenancies)
        {
            return tenancies.Select(x => x.ToResponse()).ToList();
        }

        private static List<Resident> ToResponse(this IEnumerable<Domain.Resident> residents)
        {
            return residents.Select(r => new Resident
            {
                FirstName = r.FirstName,
                LastName = r.LastName,
                DateOfBirth = r.DateOfBirth?.ToString("yyyy-MM-dd"),
                PersonNumber = r.PersonNumber,
                Responsible = r.Responsible,
                Title = r.Title
            }).ToList();
        }
    }
}
