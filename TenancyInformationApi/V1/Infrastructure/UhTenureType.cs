using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenancyInformationApi.V1.Infrastructure
{
    [Table("tenure", Schema = "dbo")]
    public class UhTenureType
    {
        [StringLength(3)]
        [Column("ten_type")] public string UhTenureTypeId { get; set; }
        [Column("ten_desc")] public string Description { get; set; }
    }
}
