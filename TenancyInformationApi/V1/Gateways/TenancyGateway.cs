using System;
using System.Collections.Generic;
using System.Linq;
using TenancyInformationApi.V1.Domain;
using TenancyInformationApi.V1.Factories;
using TenancyInformationApi.V1.Infrastructure;

namespace TenancyInformationApi.V1.Gateways
{
    //TODO: Rename to match the data source that is being accessed in the gateway eg. MosaicGateway
    public class TenancyGateway : ITenancyGateway
    {
        private readonly UhContext _uhContext;

        public TenancyGateway(UhContext databaseContext)
        {
            _uhContext = databaseContext;
        }

        public Tenancy GetById(string id)
        {
            return _uhContext
                .UhTenancyAgreements
                .Find(id)?
                .ToDomain();
        }

        public List<Tenancy> GetAll()
        {
            return _uhContext
                .UhTenancyAgreements
                .ToList()
                .ToDomain();
        }
    }
}
