using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenancyInformationApi.V1.Infrastructure
{
    [Table("lookup", Schema = "dbo")]
    public class UhAgreementType
    {
        [StringLength(3)]
        [Column("lu_type")] public string LookupType { get; set; }

        [StringLength(3)]
        [Column("lu_ref")] public string UhAgreementTypeId { get; set; }

        [MaxLength(80)]
        [Column("lu_desc")] public string Description { get; set; }
    }
}
