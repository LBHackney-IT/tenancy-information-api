using System.Linq;
using AutoFixture;
using TenancyInformationApi.V1.Infrastructure;
using NUnit.Framework;

namespace TenancyInformationApi.Tests.V1.Infrastructure
{
    [TestFixture]
    public class UhContextTests : UhTests
    {
        [Test]
        public void CanGetADatabaseEntity()
        {
            var databaseEntity = new Fixture().Create<UhTenancyAgreement>();
            UhContext.Add(databaseEntity);
            UhContext.SaveChanges();

            var result = UhContext.UhTenancyAgreements.ToList().FirstOrDefault();

            Assert.AreEqual(result, databaseEntity);
        }
    }
}
