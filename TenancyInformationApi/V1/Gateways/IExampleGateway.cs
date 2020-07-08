using System.Collections.Generic;
using TenancyInformationApi.V1.Domain;

namespace TenancyInformationApi.V1.Gateways
{
    public interface IExampleGateway
    {
        Entity GetEntityById(int id);

        List<Entity> GetAll();
    }
}
