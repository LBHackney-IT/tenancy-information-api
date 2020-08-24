using TenancyInformationApi.V1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NUnit.Framework;

namespace TenancyInformationApi.Tests
{
    [TestFixture]
    public class UhTests
    {
        private IDbContextTransaction _transaction;
        protected UhContext UhContext { get; private set; }

        [SetUp]
        public void RunBeforeAnyTests()
        {
            var builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(ConnectionString.TestDatabase());
            UhContext = new UhContext(builder.Options);

            UhContext.Database.EnsureCreated();
            _transaction = UhContext.Database.BeginTransaction();
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}
