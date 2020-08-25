using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenancyInformationApi.V1.Infrastructure
{
    [Table("property", Schema = "dbo")]
    public class UHProperty
    {
        [Column("prop_ref")]
        [StringLength(12)]
        [Key]
        public string PropertyReference { get; set; }

        [Column("house_ref")]
        [StringLength(10)]
        public string HouseReference { get; set; }

        [Column("address1")]
        [StringLength(255)]
        public string AddressLine1 { get; set; }

        [Column("post_code")]
        [StringLength(10)]
        public string Postcode { get; set; }

        [Column("u_llpg_ref")]
        [StringLength(16)]
        public string UPRN { get; set; }

        [Column("managed_property")]
        public bool ManagedProperty { get; set; }

        [Column("ownership")]
        [StringLength(10)]
        public string Ownership { get; set; }

        [Column("letable")]
        public bool Letable { get; set; }

        [Column("lounge")]
        public bool Lounge { get; set; }

        [Column("laundry")]
        public bool Laundry { get; set; }

        [Column("visitor_bed")]
        public bool VisitorBed { get; set; }

        [Column("store")]
        public bool Store { get; set; }

        [Column("warden_flat")]
        public bool WardenFlat { get; set; }

        [Column("sheltered")]
        public bool Sheltered { get; set; }

        [Column("shower")]
        public bool Shower { get; set; }

        [Column("rtb")]
        public bool Rtb { get; set; }

        [Column("core_shared")]
        public bool CoreShared { get; set; }

        [Column("asbestos")]
        public bool Asbestos { get; set; }

        [Column("no_single_beds")]
        public int NoSingleBeds { get; set; }

        [Column("no_double_beds")]
        public int NoDoubleBeds { get; set; }

        [Column("online_repairs")]
        public bool OnlineRepairs { get; set; }

        [Column("repairable")]
        public bool Repairable { get; set; }

        [Column("dtstamp")]
        public DateTime Dtstamp { get; set; }
    }
}
