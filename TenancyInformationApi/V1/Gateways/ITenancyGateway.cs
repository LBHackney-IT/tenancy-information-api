using System.Collections.Generic;
using TenancyInformationApi.V1.Domain;

namespace TenancyInformationApi.V1.Gateways
{
    public interface ITenancyGateway
    {
        Tenancy GetById(string id);
        List<Tenancy> ListTenancies(int limit, int cursor, string addressQuery, string postcodeQuery, bool leaseholdsOnly, bool freeholdsOnly);
    }
}
