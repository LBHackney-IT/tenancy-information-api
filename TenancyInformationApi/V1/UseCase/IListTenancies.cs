using TenancyInformationApi.V1.Boundary.Response;

namespace TenancyInformationApi.V1.UseCase
{
    public interface IListTenancies
    {
        ListTenanciesResponse Execute(int limit, int cursor);
    }
}
