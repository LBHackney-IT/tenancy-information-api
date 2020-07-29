using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TenancyInformationApi.V1.Infrastructure
{
    public class UhContext : DbContext
    {
        public UhContext(DbContextOptions options) : base(options)
        {
            var tenancyAgreements = UhTenancyAgreements
                .Include(t => t.UhAgreementType)
                .Include(t => t.UhTenureType)
                .ToList();
        }

        public DbSet<UhTenancyAgreement> UhTenancyAgreements { get; set; }
    }

    [Table("lookup")]
    public class UhAgreementType
    {
        // [Column("lu_type")] private const string LookupType = "ZAG";
        [StringLength(1)]
        [Column("lu_ref"), Key] public string UhAgreementTypeId { get; set; }
        [Column("lu_desc")] public string Description { get; set; }
    }

    [Table("tenure")]
    public class UhTenureType
    {
        [StringLength(3)]
        [Column("ten_type")] public string UhTenureTypeId { get; set; }
        [Column("ten_desc")] public string Description { get; set; }
    }
}
