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

        public List<Tenancy> ListTenancies(int limit, int cursor, string addressQuery, string postcodeQuery,
                bool leaseholdsOnly, bool freeholdsOnly, string propertyReference)
        {
            var tagRefsToRetrieve = TagRefsToReturn(limit, cursor, addressQuery, postcodeQuery, leaseholdsOnly,
                freeholdsOnly, propertyReference);
            var tenancies = GetTenancyDetailsForTagRefs(tagRefsToRetrieve);

            return GroupByTagRefAndMapToDomain(tenancies);
        }

        private static List<Tenancy> GroupByTagRefAndMapToDomain(List<DatabaseRecords> tenancies)
        {
            return tenancies.GroupBy(t => t.Agreement.TenancyAgreementReference, t => t, (key, grp) =>
            {
                var first = grp.First();
                var domain = first.Agreement.ToDomain(first.AgreementType, first.TenureType, first.Property);
                domain.Residents = grp.Select(g => g.Resident?.ToDomain()).Where(r => r != null).ToList();
                return domain;
            }).ToList();
        }

        private List<DatabaseRecords> GetTenancyDetailsForTagRefs(List<string> tagRefsToRetrieve)
        {
            return (
                from agreement in _uhContext.UhTenancyAgreements
                join tenureType in _uhContext.UhTenure on agreement.UhTenureTypeId equals tenureType.UhTenureTypeId
                join agreementType in _uhContext.UhTenancyAgreementsType on agreement.UhAgreementTypeId.ToString()
                    equals agreementType.UhAgreementTypeId.Trim()
                join property in _uhContext.UhProperties on agreement.PropertyReference equals property.PropertyReference
                join res in _uhContext.UhResidents on agreement.HouseholdReference equals res.HouseReference into rs
                from resident in rs.DefaultIfEmpty()
                let tagRefFormattedForPagination =
                    Convert.ToInt32(agreement.TenancyAgreementReference.Replace("/", "").Replace("Z", ""))
                where tagRefsToRetrieve.Contains(agreement.TenancyAgreementReference)
                where agreementType.LookupType == "ZAG"
                orderby tagRefFormattedForPagination
                select new DatabaseRecords
                {
                    Agreement = agreement,
                    TenureType = tenureType,
                    AgreementType = agreementType,
                    Property = property,
                    Resident = resident
                }).ToList();
        }

        private class DatabaseRecords
        {
            public UhTenancyAgreement Agreement { get; set; }
            public UhTenureType TenureType { get; set; }
            public UhAgreementType AgreementType { get; set; }
            public UHProperty Property { get; set; }
            public UHResident Resident { get; set; }
        }

        private List<string> TagRefsToReturn(int limit, int cursor, string addressQuery, string postcodeQuery, bool leaseholdsOnly,
            bool freeholdsOnly, string propertyReference)
        {
            var invalidTagRefList = GetInvalidTagRefList();
            var addressSearchPattern = GetSearchPattern(addressQuery);
            var postcodeSearchPattern = GetSearchPattern(postcodeQuery);

            return (
                from agreement in _uhContext.UhTenancyAgreements
                join tenureType in _uhContext.UhTenure on agreement.UhTenureTypeId equals tenureType.UhTenureTypeId
                join agreementType in _uhContext.UhTenancyAgreementsType on agreement.UhAgreementTypeId.ToString()
                    equals agreementType.UhAgreementTypeId.Trim()
                join property in _uhContext.UhProperties on agreement.PropertyReference equals property.PropertyReference
                let tagRefFormattedForPagination =
                    Convert.ToInt32(agreement.TenancyAgreementReference.Replace("/", "").Replace("Z", ""))
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
                where string.IsNullOrEmpty(propertyReference)
                      || EF.Functions.ILike(property.PropertyReference, propertyReference)
                orderby tagRefFormattedForPagination
                select agreement.TenancyAgreementReference
            ).Take(limit).ToList();
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
