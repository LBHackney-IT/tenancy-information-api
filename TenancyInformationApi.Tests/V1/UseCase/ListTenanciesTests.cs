using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Bogus;
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
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<ITenancyGateway>();
            _classUnderTest = new ListTenancies(_mockGateway.Object);
        }

        [Test]
        public void GivenNoQueryParametersExecuteCallsTheGatewayToGetRecords()
        {
            _mockGateway.Setup(x => x.ListTenancies(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Tenancy>()).Verifiable();
            _classUnderTest.Execute(20, 0);
            _mockGateway.Verify();
        }

        [Test]
        public void ExecuteReturnsTenanciesFromTheGatewayMappedToResponses()
        {
            var stubbedTenancies = _fixture.CreateMany<Tenancy>().ToList();
            _mockGateway.Setup(x => x.ListTenancies(It.IsAny<int>(), It.IsAny<int>())).Returns(stubbedTenancies);

            _classUnderTest.Execute(20, 0).Tenancies.Should().BeEquivalentTo(stubbedTenancies.ToResponse());
        }

        [Test]
        public void ExecuteCallsTheGatewayWithLimitAndFormattedCursor()
        {
            _mockGateway.Setup(x => x.ListTenancies(23, 236712)).Returns(new List<Tenancy>()).Verifiable();
            _classUnderTest.Execute(23, 236712);
            _mockGateway.Verify();
        }

        [Test]
        public void ExecuteIfNotCursorSuppliedPasses0ToTheGateway()
        {
            _mockGateway.Setup(x => x.ListTenancies(It.IsAny<int>(), 0)).Returns(new List<Tenancy>()).Verifiable();
            _classUnderTest.Execute(20, 0);
            _mockGateway.Verify();
        }

        [Test]
        public void IfLimitLessThanTheMinimumWillUseTheMinimumLimit()
        {
            _mockGateway.Setup(x => x.ListTenancies(10, It.IsAny<int>()))
                .Returns(new List<Tenancy>()).Verifiable();
            _classUnderTest.Execute(0, 0);
            _mockGateway.Verify();
        }

        [Test]
        public void IfLimitMoreThanTheMaximumWillUseTheMaximumLimit()
        {
            _mockGateway.Setup(x => x.ListTenancies(100, It.IsAny<int>()))
                .Returns(new List<Tenancy>()).Verifiable();
            _classUnderTest.Execute(400, 0);
            _mockGateway.Verify();
        }

        [Test]
        public void ReturnsTheNextCursorFormattedCorrectly()
        {
            var faker = new Faker();
            var stubbedResidents = _fixture.CreateMany<Tenancy>(10).Select((t, index) =>
            {
                var tagRef = $"{index}{faker.Random.Int(0, 9999):0000}/{faker.Random.Int(1, 9)}";
                t.TenancyAgreementReference = tagRef;
                return t;
            }).ToList();

            _mockGateway
                .Setup(x => x.ListTenancies(10, It.IsAny<int>()))
                .Returns(stubbedResidents);
            var tagRefForNextCursor = stubbedResidents.Last().TenancyAgreementReference;
            var expectedNextCursor = $"{tagRefForNextCursor.Substring(0, 5)}{tagRefForNextCursor.Substring(6, 1)}";

            _classUnderTest.Execute(10, 0).NextCursor.Should().Be(expectedNextCursor);
        }

        [Test]
        public void WhenAtTheEndOfTheResidentListReturnsNullForTheNextCursor()
        {
            var stubbedResidents = _fixture.CreateMany<Tenancy>(7);

            _mockGateway.Setup(x =>
                    x.ListTenancies(10, It.IsAny<int>()))
                .Returns(stubbedResidents.ToList());

            _classUnderTest.Execute(10, 0).NextCursor.Should().Be(null);
        }
    }
}
