using System.Linq;
using AutoFixture;
using FluentAssertions;
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

            var result = UhContext.UhTenancyAgreements.ToList().LastOrDefault();

            result.Should().BeEquivalentTo(databaseEntity);
        }
    }
}
