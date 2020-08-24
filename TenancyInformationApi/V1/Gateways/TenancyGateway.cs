using System.Linq;
using Microsoft.EntityFrameworkCore;
using TenancyInformationApi.V1.Domain;
using TenancyInformationApi.V1.Factories;
using TenancyInformationApi.V1.Infrastructure;

namespace TenancyInformationApi.V1.Gateways
{
    public class TenancyGateway : ITenancyGateway
    {
        private readonly UhContext _uhContext;

        public TenancyGateway(UhContext databaseContext)
        {
            _uhContext = databaseContext;
        }

        public Tenancy GetById(string id)
        {
            var tenancyAgreement = _uhContext.UhTenancyAgreements
                .Include(ta => ta.UhTenureType)
                .FirstOrDefault(ta => ta.TenancyAgreementReference == id);
            if (tenancyAgreement == null) return null;

            var agreementLookup = _uhContext.UhTenancyAgreementsType
                .Where(a => a.LookupType == "ZAG")
                .First(a => a.UhAgreementTypeId.Trim() == tenancyAgreement.UhAgreementTypeId.ToString());

            return tenancyAgreement.ToDomain(agreementLookup);
        }
    }
}
