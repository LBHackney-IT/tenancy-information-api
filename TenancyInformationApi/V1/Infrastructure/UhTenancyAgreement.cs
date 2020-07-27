using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;

namespace TenancyInformationApi.V1.Infrastructure
{
    [Table("tenagree")]
    public class UhTenancyAgreement
    {
        [Column("tag_ref"), Key]   public string   TenancyAgreementReference { get; set; }
        [Column("cot")]            public DateTime CommencementOfTenancy     { get; set; }
        [Column("eot")]            public DateTime EndOfTenancy              { get; set; }
        [Column("cur_bal")]        public float    CurrentRentBalance        { get; set; }
        [Column("present")]        public bool     IsPresent                 { get; set; }
        [Column("terminated")]     public bool     IsTerminated              { get; set; }
        [Column("u_saff_rentacc")] public string   PaymentReference          { get; set; }
        [Column("house_ref")]      public string   HouseholdReference        { get; set; }
        [Column("prop_ref")]       public string   PropertyReference         { get; set; }
        [Column("service")]        public float    ServiceCharge             { get; set; }
        [Column("other_charge")]   public float    OtherCharges              { get; set; }

        [Column("tenure")] public   string          UhTenureTypeId    { get; set; }
        public                      UhTenureType    UhTenureType      { get; set; }
        [Column("agr_type")] public string          UhAgreementTypeId { get; set; }
        public                      UhAgreementType UhAgreementType   { get; set; }
    }
}
