using Microsoft.EntityFrameworkCore;

namespace TenancyInformationApi.V1.Infrastructure
{
    public class UhContext : DbContext
    {
        public UhContext(DbContextOptions options) : base(options)
        { }

        public DbSet<UhTenancyAgreement> UhTenancyAgreements { get; set; }

        public DbSet<UhAgreementType> UhTenancyAgreementsType { get; set; }

        public DbSet<UhTenureType> UhTenure { get; set; }

    }
}
