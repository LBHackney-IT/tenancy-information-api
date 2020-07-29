using TenancyInformationApi.V1.Boundary.Response;
using TenancyInformationApi.V1.Factories;
using TenancyInformationApi.V1.Gateways;
using TenancyInformationApi.V1.UseCase.Interfaces;

namespace TenancyInformationApi.V1.UseCase
{
    public class GetTenancyByIdUseCase : IGetTenancyByIdUseCase
    {
        private readonly ITenancyGateway _gateway;
        public GetTenancyByIdUseCase(ITenancyGateway gateway)
        {
            _gateway = gateway;
        }

        public TenancyInformationResponse Execute(string id)
        {
            var result = _gateway.GetById(id).ToResponse();
            return result;
        }
    }
}
