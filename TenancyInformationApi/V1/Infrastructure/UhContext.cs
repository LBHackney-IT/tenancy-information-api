using Microsoft.EntityFrameworkCore;

namespace TenancyInformationApi.V1.Infrastructure
{
    public class UhContext : DbContext
    {
        public UhContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UhAgreementType>()
                .HasKey(o => new { o.LookupType, o.UhAgreementTypeId });
        }

        public DbSet<UhTenancyAgreement> UhTenancyAgreements { get; set; }

        public DbSet<UhAgreementType> UhTenancyAgreementsType { get; set; }

        public DbSet<UhTenureType> UhTenure { get; set; }

    }
}
