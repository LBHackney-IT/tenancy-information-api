using TenancyInformationApi.V1.Boundary.Response;
using TenancyInformationApi.V1.Factories;
using TenancyInformationApi.V1.Gateways;

namespace TenancyInformationApi.V1.UseCase
{
    public class ListTenancies : IListTenancies
    {
        private readonly ITenancyGateway _gateway;

        public ListTenancies(ITenancyGateway gateway)
        {
            _gateway = gateway;
        }

        public ListTenanciesResponse Execute()
        {
            var tenancies = _gateway.ListTenancies();
            return new ListTenanciesResponse
            {
                Tenancies = tenancies.ToResponse()
            };
        }
    }
}
