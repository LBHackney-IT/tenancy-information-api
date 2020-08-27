using System;
using System.Collections.Generic;
using System.Linq;
using TenancyInformationApi.V1.Boundary.Response;
using TenancyInformationApi.V1.Domain;
using TenancyInformationApi.V1.Factories;
using TenancyInformationApi.V1.Gateways;

namespace TenancyInformationApi.V1.UseCase
{
    public class ListTenancies : IListTenancies
    {
        private readonly ITenancyGateway _gateway;
        private readonly IValidatePostcode _validatePostcode;

        public ListTenancies(ITenancyGateway gateway, IValidatePostcode validatePostcode)
        {
            _validatePostcode = validatePostcode;
            _gateway = gateway;
        }

        public ListTenanciesResponse Execute(int limit, int cursor, string addressQuery, string postcodeQuery)
        {
            limit = limit < 10 ? 10 : limit;
            limit = limit > 100 ? 100 : limit;
            if (!_validatePostcode.Execute(postcodeQuery)) throw new InvalidQueryParameterException("The Postcode given does not have a valid format");

            var tenancies = _gateway.ListTenancies(limit, cursor, addressQuery, postcodeQuery);
            return new ListTenanciesResponse
            {
                Tenancies = tenancies.ToResponse(),
                NextCursor = tenancies.Count == limit ? GetNextCursor(tenancies) : null
            };
        }

        private static string GetNextCursor(List<Tenancy> tenancies)
        {
            return tenancies.Max(t => Convert.ToInt32(t.TenancyAgreementReference.Replace("/", ""))).ToString();
        }

        private void CheckPostcodeValid(string postcode)
        {
            if (string.IsNullOrWhiteSpace(postcode)) return;
            var validPostcode = _validatePostcode.Execute(postcode);
            if (!validPostcode) throw new InvalidQueryParameterException("The Postcode given does not have a valid format");
        }
    }
}
