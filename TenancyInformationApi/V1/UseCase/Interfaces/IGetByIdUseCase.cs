using TenancyInformationApi.V1.Boundary.Response;

namespace TenancyInformationApi.V1.UseCase.Interfaces
{
    public interface IGetByIdUseCase
    {
        ResponseObject Execute(int id);
    }
}
