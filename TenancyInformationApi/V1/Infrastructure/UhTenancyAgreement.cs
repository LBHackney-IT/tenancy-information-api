using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenancyInformationApi.V1.Infrastructure
{
    [Table("tenagree", Schema = "dbo")]
    public class UhTenancyAgreement
    {
        [StringLength(11)] [Column("tag_ref"), Key] public string TenancyAgreementReference { get; set; }
        [StringLength(10)] [Column("house_ref")] public string HouseholdReference { get; set; }
        [StringLength(12)] [Column("prop_ref")] public string PropertyReference { get; set; }
        [StringLength(20)] [Column("u_saff_rentacc")] public string PaymentReference { get; set; }
        [StringLength(3)] [Column("tenure")] public string UhTenureTypeId { get; set; }
        [Column("agr_type")] public char UhAgreementTypeId { get; set; }
        [Column("present")] public bool IsPresent { get; set; }
        [Column("terminated")] public bool IsTerminated { get; set; }
        [Column("cur_bal")] public float? CurrentRentBalance { get; set; }
        [Column("service")] public float? ServiceCharge { get; set; }
        [Column("other_charge")] public float? OtherCharges { get; set; }
        [Column("cot")] public DateTime? CommencementOfTenancy { get; set; }
        [Column("eot")] public DateTime? EndOfTenancy { get; set; }

        public UhTenureType UhTenureType { get; set; }
    }
}
