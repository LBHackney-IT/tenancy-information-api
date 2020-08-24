using AutoFixture;
using TenancyInformationApi.V1.Domain;
using TenancyInformationApi.V1.Gateways;
using FluentAssertions;
using NUnit.Framework;
using TenancyInformationApi.V1.Factories;
using TenancyInformationApi.V1.Infrastructure;

namespace TenancyInformationApi.Tests.V1.Gateways
{
    [TestFixture]
    public class TenancyGatewayTests : UhTests
    {
        private readonly Fixture _fixture = new Fixture();
        private TenancyGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new TenancyGateway(UhContext);
        }

        [Test]
        public void GetByIdReturnsNullIfEntityDoesntExist()
        {
            var response = _classUnderTest.GetById("123/2");

            response.Should().BeNull();
        }

        [Test]
        public void GetByIdReturnsTheEntityIfItExists()
        {
            var uhTenancy = _fixture.Create<UhTenancyAgreement>();
            UhContext.UhTenancyAgreements.Add(uhTenancy);
            UhContext.SaveChanges();
            var expected = uhTenancy.ToDomain();

            var response = _classUnderTest.GetById(uhTenancy.TenancyAgreementReference);

            response.Should().BeEquivalentTo(expected);
        }
    }
}
