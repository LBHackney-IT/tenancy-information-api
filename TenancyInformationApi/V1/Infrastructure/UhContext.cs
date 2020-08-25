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
            modelBuilder.Entity<UHResident>()
                .HasKey(o => new { o.PersonNumber, o.HouseReference });
        }

        public DbSet<UhTenancyAgreement> UhTenancyAgreements { get; set; }
        public DbSet<UhAgreementType> UhTenancyAgreementsType { get; set; }
        public DbSet<UhTenureType> UhTenure { get; set; }
        public DbSet<UHProperty> UhProperties { get; set; }
        public DbSet<UHResident> UhResidents { get; set; }

    }
}
