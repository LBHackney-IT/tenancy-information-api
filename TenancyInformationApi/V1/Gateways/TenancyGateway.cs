using System;
using System.Collections.Generic;
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

        public List<Tenancy> ListTenancies(int limit, int cursor)
        {
            var tenancies = (
                from agreement in _uhContext.UhTenancyAgreements
                join tenureType in _uhContext.UhTenure on agreement.UhTenureTypeId equals tenureType.UhTenureTypeId
                join agreementType in _uhContext.UhTenancyAgreementsType on agreement.UhAgreementTypeId.ToString()
                    equals agreementType.UhAgreementTypeId.Trim()
                join property in _uhContext.UhProperties on agreement.PropertyReference equals property.PropertyReference
                where agreementType.LookupType == "ZAG"
                where Convert.ToInt32(agreement.TenancyAgreementReference.Replace("/", "")) > cursor
                orderby Convert.ToInt32(agreement.TenancyAgreementReference.Replace("/", ""))
                select new
                {
                    Agreement = agreement,
                    TenureType = tenureType,
                    AgreementType = agreementType,
                    Property = property,
                }).Take(limit);

            return tenancies.ToList().Select(t =>
            {
                var domain = t.Agreement.ToDomain(t.AgreementType, t.TenureType, t.Property);
                domain.Residents = _uhContext.UhResidents.Where(r => r.HouseReference == domain.HouseholdReference).ToDomain();
                return domain;
            }).ToList();
        }
    }
}
