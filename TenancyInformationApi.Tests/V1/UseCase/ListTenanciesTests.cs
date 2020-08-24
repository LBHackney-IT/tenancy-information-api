using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TenancyInformationApi.V1.Domain;
using TenancyInformationApi.V1.Factories;
using TenancyInformationApi.V1.Gateways;
using TenancyInformationApi.V1.UseCase;

namespace TenancyInformationApi.Tests.V1.UseCase
{
    public class ListTenanciesTests
    {
        private ListTenancies _classUnderTest;
        private Mock<ITenancyGateway> _mockGateway;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<ITenancyGateway>();
            _classUnderTest = new ListTenancies(_mockGateway.Object);
        }

        [Test]
        public void GivenNoQueryParametersExecuteCallsTheGatewayToGetRecords()
        {
            _mockGateway.Setup(x => x.ListTenancies()).Returns(new List<Tenancy>()).Verifiable();
            _classUnderTest.Execute();
            _mockGateway.Verify();
        }

        [Test]
        public void ExecuteReturnsTenanciesFromTheGatewayMappedToResponses()
        {
            var fixture = new Fixture();
            var stubbedTenancies = fixture.CreateMany<Tenancy>().ToList();
            _mockGateway.Setup(x => x.ListTenancies()).Returns(stubbedTenancies);

            _classUnderTest.Execute().Tenancies.Should().BeEquivalentTo(stubbedTenancies.ToResponse());
        }
    }
}
