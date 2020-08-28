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

        public List<Tenancy> ListTenancies(int limit, int cursor, string addressQuery, string postcodeQuery, bool leaseholdsOnly, bool freeholdsOnly)
        {
            var invalidTagRefList = GetInvalidTagRefList();
            var addressSearchPattern = GetSearchPattern(addressQuery);
            var postcodeSearchPattern = GetSearchPattern(postcodeQuery);
            var tenancies = (
                from agreement in _uhContext.UhTenancyAgreements
                join tenureType in _uhContext.UhTenure on agreement.UhTenureTypeId equals tenureType.UhTenureTypeId
                join agreementType in _uhContext.UhTenancyAgreementsType on agreement.UhAgreementTypeId.ToString()
                    equals agreementType.UhAgreementTypeId.Trim()
                join property in _uhContext.UhProperties on agreement.PropertyReference equals property.PropertyReference
                let tagRefFormattedForPagination = Convert.ToInt32(agreement.TenancyAgreementReference.Replace("/", "").Replace("Z", ""))
                where !invalidTagRefList.Contains(agreement.TenancyAgreementReference)
                where !EF.Functions.ILike(agreement.TenancyAgreementReference, "DUMMY/%")
                where agreementType.LookupType == "ZAG"
                where !freeholdsOnly || tenureType.UhTenureTypeId == "FRE" || tenureType.UhTenureTypeId == "FRS"
                where !leaseholdsOnly || tenureType.UhTenureTypeId == "LEA"
                where tagRefFormattedForPagination > cursor
                where string.IsNullOrEmpty(addressQuery)
                      || EF.Functions.ILike(property.AddressLine1.Replace(" ", ""), addressSearchPattern)
                      || EF.Functions.ILike(property.Postcode.Replace(" ", ""), addressSearchPattern)
                where string.IsNullOrEmpty(postcodeQuery)
                      || EF.Functions.ILike(property.Postcode.Replace(" ", ""), postcodeSearchPattern)
                orderby tagRefFormattedForPagination
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

        private static List<string> GetInvalidTagRefList()
        {
            return new List<string>
            {
                "SUSP/LEGLEA",
                "SUSP/LEGRNT",
                "SSSSSS",
                "YYYYYY",
                "ZZZZZZ",
            };
        }

        private static string GetSearchPattern(string str)
        {
            return $"%{str?.Replace(" ", "")}%";
        }
    }
}
