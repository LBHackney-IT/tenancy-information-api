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

    public static class UhEntityHelper
    {
        public static UhTenancyAgreement FromDomain<T>(T tenancy) where T : Tenancy =>
            new UhTenancyAgreement
            {
                TenancyAgreementReference = tenancy.TenancyReference,
                CommencementOfTenancy = tenancy.CommencementOfTenancyDate,
                EndOfTenancy = tenancy.EndOfTenancyDate,
                CurrentRentBalance = tenancy.CurrentBalance,
                IsPresent = tenancy.Present,
                IsTerminated = tenancy.Terminated,
                PaymentReference = tenancy.PaymentReference,
                HouseholdReference = tenancy.HouseholdReference,
                PropertyReference = tenancy.PropertyReference,
                ServiceCharge = tenancy.Service,
                OtherCharges = tenancy.OtherCharge,
                UhTenureTypeId = tenancy.Tenure,
                UhAgreementTypeId = tenancy.Agreement,
                UhTenureType = new Fixture().Create<UhTenureType>(),
                UhAgreementType = new Fixture().Create<UhAgreementType>()
            };
    }
}
