using TenancyInformationApi.V1.Boundary.Response;

namespace TenancyInformationApi.V1.UseCase
{
    public interface IListTenancies
    {
        ListTenanciesResponse Execute(int limit, int cursor, string addressQuery, string postcodeQuery,
                bool leaseholdsOnly, bool freeholdsOnly, string propertyReference);
    }
}
