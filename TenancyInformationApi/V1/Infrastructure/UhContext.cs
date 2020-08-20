using System;
using System.Collections.Generic;
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

        public DbSet<UhAgreementType> UhTenancyAgreementsType { get; set; }

        public DbSet<UhTenureType> UhTenure { get; set; }

    }
}
