using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Bogus;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TenancyInformationApi.V1.Boundary.Response;
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
        private Mock<IValidatePostcode> _mockPostcodeValidator;

        [SetUp]
        public void SetUp()
        {
            _mockPostcodeValidator = new Mock<IValidatePostcode>();
            _mockPostcodeValidator.Setup(x => x.Execute(It.IsAny<string>())).Returns(true);
            _mockGateway = new Mock<ITenancyGateway>();
            _classUnderTest = new ListTenancies(_mockGateway.Object, _mockPostcodeValidator.Object);
        }

        [Test]
        public void GivenNoQueryParametersExecuteCallsTheGatewayToGetRecords()
        {
            SetupMockGatewayToExpectParameters();
            CallUseCaseWithArgs(20, 0);
            _mockGateway.Verify();
        }

        [Test]
        public void ExecuteReturnsTenanciesFromTheGatewayMappedToResponses()
        {
            var stubbedTenancies = _fixture.CreateMany<Tenancy>().ToList();
            SetupMockGatewayToExpectParameters(stubbedTenancies: stubbedTenancies);

            CallUseCaseWithArgs(20, 0).Tenancies.Should().BeEquivalentTo(stubbedTenancies.ToResponse());
        }

        [Test]
        public void ExecuteCallsTheGatewayWithLimitAndFormattedCursor()
        {
            SetupMockGatewayToExpectParameters(limit: 23, cursor: 236712);
            CallUseCaseWithArgs(23, 236712);
            _mockGateway.Verify();
        }

        [Test]
        public void ExecuteIfNotCursorSuppliedPasses0ToTheGateway()
        {
            SetupMockGatewayToExpectParameters(cursor: 0);
            CallUseCaseWithArgs(20, 0);
            _mockGateway.Verify();
        }

        [Test]
        public void IfLimitLessThanTheMinimumWillUseTheMinimumLimit()
        {
            SetupMockGatewayToExpectParameters(limit: 10);
            CallUseCaseWithArgs(0, 0);
            _mockGateway.Verify();
        }

        [Test]
        public void IfLimitMoreThanTheMaximumWillUseTheMaximumLimit()
        {
            SetupMockGatewayToExpectParameters(limit: 100);
            CallUseCaseWithArgs(400, 0);
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

            CallUseCaseWithArgs(10, 0).NextCursor.Should().Be(expectedNextCursor);
        }

        [Test]
        public void WhenAtTheEndOfTheResidentListReturnsNullForTheNextCursor()
        {
            var stubbedTenancies = _fixture.CreateMany<Tenancy>(7);
            SetupMockGatewayToExpectParameters(limit: 10, stubbedTenancies: stubbedTenancies);

            CallUseCaseWithArgs(10, 0).NextCursor.Should().Be(null);
        }

        [Test]
        public void ExecuteCallsTheGatewayWithAddressQueryParameter()
        {
            var addressQuery = _fixture.Create<string>();
            SetupMockGatewayToExpectParameters(addressQuery: addressQuery);

            CallUseCaseWithArgs(20, 0, addressQuery);
            _mockGateway.Verify();
        }

        [Test]
        public void ExecuteCallsTheGatewayWithPostcodeQueryParameter()
        {
            var postcodeQuery = _fixture.Create<string>();
            SetupMockGatewayToExpectParameters(postcodeQuery: postcodeQuery);

            CallUseCaseWithArgs(20, 0, postcode: postcodeQuery);
            _mockGateway.Verify();
        }

        [Test]
        public void IfPostcodeQueryIsInvalidExecuteWillReturnBadRequest()
        {
            var postcode = "E8881DY";
            SetupMockGatewayToExpectParameters(postcodeQuery: postcode);
            _mockPostcodeValidator.Setup(x => x.Execute(postcode)).Returns(false);

            Func<ListTenanciesResponse> testDelegate = () => _classUnderTest.Execute(15, 3, null, postcode);
            testDelegate.Should().Throw<InvalidQueryParameterException>()
                .WithMessage("The Postcode given does not have a valid format");
        }

        private void SetupMockGatewayToExpectParameters(int? limit = null, int? cursor = null,
            string addressQuery = null, string postcodeQuery = null, IEnumerable<Tenancy> stubbedTenancies = null)
        {
            _mockGateway
                .Setup(x =>
                    x.ListTenancies(It.Is<int>(l => CheckParameter(limit, l)), cursor ?? It.IsAny<int>(), addressQuery, postcodeQuery))
                .Returns(stubbedTenancies?.ToList() ?? new List<Tenancy>()).Verifiable();
        }

        private static bool CheckParameter(int? expectedParam, int receivedParam)
        {
            return expectedParam == null || receivedParam == expectedParam.Value;
        }

        private ListTenanciesResponse CallUseCaseWithArgs(int limit, int cursor, string address = null, string postcode = null)
        {
            return _classUnderTest.Execute(limit, cursor, address, postcode);
        }
    }
}
