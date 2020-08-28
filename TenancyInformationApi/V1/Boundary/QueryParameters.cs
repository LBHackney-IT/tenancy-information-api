using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace TenancyInformationApi.V1.Boundary
{
    public class QueryParameters
    {
        [FromQuery(Name = "address")]
        public string Address { get; set; }

        [FromQuery(Name = "postcode")]
        public string Postcode { get; set; }

        [FromQuery(Name = "freehold_only")]
        [DefaultValue(false)]
        public bool FreeholdsOnly { get; set; } = false;

        [FromQuery(Name = "leasehold_only")]
        [DefaultValue(false)]
        public bool LeaseholdsOnly { get; set; } = false;

        [FromQuery(Name = "limit")]
        [DefaultValue(20)]
        public int Limit { get; set; } = 20;

        [FromQuery(Name = "cursor")]
        [DefaultValue(0)]
        public int Cursor { get; set; } = 0;
    }
}
