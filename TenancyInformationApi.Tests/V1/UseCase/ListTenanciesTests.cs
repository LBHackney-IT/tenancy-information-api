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
            SetupMockGatewayToExpectParameters();
            _classUnderTest.Execute(20, 0, null);
            _mockGateway.Verify();
        }

        [Test]
        public void ExecuteReturnsTenanciesFromTheGatewayMappedToResponses()
        {
            var stubbedTenancies = _fixture.CreateMany<Tenancy>().ToList();
            SetupMockGatewayToExpectParameters(stubbedTenancies: stubbedTenancies);

            _classUnderTest.Execute(20, 0, null).Tenancies.Should().BeEquivalentTo(stubbedTenancies.ToResponse());
        }

        [Test]
        public void ExecuteCallsTheGatewayWithLimitAndFormattedCursor()
        {
            SetupMockGatewayToExpectParameters(limit: 23, cursor: 236712);
            _classUnderTest.Execute(23, 236712, null);
            _mockGateway.Verify();
        }

        [Test]
        public void ExecuteIfNotCursorSuppliedPasses0ToTheGateway()
        {
            SetupMockGatewayToExpectParameters(cursor: 0);
            _classUnderTest.Execute(20, 0, null);
            _mockGateway.Verify();
        }

        [Test]
        public void IfLimitLessThanTheMinimumWillUseTheMinimumLimit()
        {
            SetupMockGatewayToExpectParameters(limit: 10);
            _classUnderTest.Execute(0, 0, null);
            _mockGateway.Verify();
        }

        [Test]
        public void IfLimitMoreThanTheMaximumWillUseTheMaximumLimit()
        {
            SetupMockGatewayToExpectParameters(limit: 100);
            _classUnderTest.Execute(400, 0, null);
            _mockGateway.Verify();
        }

        [Test]
        public void ReturnsTheNextCursorFormattedCorrectly()
        {
            var faker = new Faker();
            var stubbedTenancies = _fixture.CreateMany<Tenancy>(10).Select((t, index) =>
            {
                var tagRef = $"{index}{faker.Random.Int(0, 9999):0000}/{faker.Random.Int(1, 9)}";
                t.TenancyAgreementReference = tagRef;
                return t;
            }).ToList();
            SetupMockGatewayToExpectParameters(limit: 10, stubbedTenancies: stubbedTenancies);

            var tagRefForNextCursor = stubbedTenancies.Last().TenancyAgreementReference;
            var expectedNextCursor = $"{tagRefForNextCursor.Substring(0, 5)}{tagRefForNextCursor.Substring(6, 1)}";

            _classUnderTest.Execute(10, 0, null).NextCursor.Should().Be(expectedNextCursor);
        }

        [Test]
        public void WhenAtTheEndOfTheResidentListReturnsNullForTheNextCursor()
        {
            var stubbedTenancies = _fixture.CreateMany<Tenancy>(7);
            SetupMockGatewayToExpectParameters(limit: 10, stubbedTenancies: stubbedTenancies);

            _classUnderTest.Execute(10, 0, null).NextCursor.Should().Be(null);
        }

        [Test]
        public void ExecuteCallsTheGatewayWithAddressQueryParameter()
        {
            var addressQuery = _fixture.Create<string>();
            SetupMockGatewayToExpectParameters(addressQuery: addressQuery);

            _classUnderTest.Execute(20, 0, addressQuery);
            _mockGateway.Verify();
        }

        private void SetupMockGatewayToExpectParameters(int? limit = null, int? cursor = null, string addressQuery = null, IEnumerable<Tenancy> stubbedTenancies = null)
        {
            _mockGateway
                .Setup(x =>
                    x.ListTenancies(It.Is<int>(l => CheckLimit(limit, l)), cursor ?? It.IsAny<int>(), addressQuery))
                .Returns(stubbedTenancies?.ToList() ?? new List<Tenancy>()).Verifiable();
        }

        private static bool CheckLimit(int? limit, int l)
        {
            return limit == null || l == limit.Value;
        }
    }
}
