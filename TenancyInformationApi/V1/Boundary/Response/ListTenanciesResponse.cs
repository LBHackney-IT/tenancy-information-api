using System.Collections.Generic;

namespace TenancyInformationApi.V1.Boundary.Response
{
    public class ListTenanciesResponse
    {
        public List<TenancyInformationResponse> Tenancies { get; set; }
        public string NextCursor { get; set; }
    }
}
