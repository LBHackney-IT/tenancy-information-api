using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace TenancyInformationApi.V1.Infrastructure
{
    [Table("lookup", Schema = "dbo")]
    public class UhAgreementType
    {
        // [Column("lu_type")] private const string LookupType = "ZAG";
        [StringLength(1)]
        [Column("lu_ref"), Key] public string UhAgreementTypeId { get; set; }
        [Column("lu_desc")] public string Description { get; set; }
    }
}
