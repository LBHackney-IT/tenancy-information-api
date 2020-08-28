using System.Linq;
using AutoFixture;
using TenancyInformationApi.V1.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TenancyInformationApi.V1.UseCase.Interfaces;
using Moq;
using TenancyInformationApi.V1.Boundary;
using TenancyInformationApi.V1.Boundary.Response;
using TenancyInformationApi.V1.Domain;
using TenancyInformationApi.V1.UseCase;

namespace TenancyInformationApi.Tests.V1.Controllers
{
    [TestFixture]
    public class TenancyInformationControllerTests
    {
        private TenancyInformationController _classUnderTest;
        private Mock<IGetTenancyByIdUseCase> _getByIdMock;
        private Mock<IListTenancies> _listTenancies;
        private Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _getByIdMock = new Mock<IGetTenancyByIdUseCase>();
            _listTenancies = new Mock<IListTenancies>();
            _classUnderTest = new TenancyInformationController(_getByIdMock.Object, _listTenancies.Object);
        }

        [Test]
        public void ViewRecordReturnsBadRequestForInvalidArguments()
        {
            // 1231 is not a valid tag_ref - should return BadRequest400
            var response = _classUnderTest.ViewRecord("1231") as ObjectResult;
            response.Should().NotBeNull();
            response?.StatusCode.Should().Be(400);
            response.Value.Should().Be("tag_ref is malformed or missing.");
        }

        [Test]
        public void ViewRecordReturnsNotFoundForMissingRecord()
        {
            _getByIdMock.Setup(x => x.Execute("123/1")).Returns((TenancyInformationResponse) null);
            // 123/1 is not in the db - should return NotFound404
            var response = _classUnderTest.ViewRecord("123-1") as ObjectResult;
            response?.StatusCode.Should().Be(404);
            response.Value.Should().Be("No tenancy was found for the provided tag_ref 123/1.");
        }

        [Test]
        public void ViewRecordReturnsRecordForValidRequest()
        {
            _getByIdMock.Setup(x => x.Execute("123/1")).Returns(new Fixture().Create<TenancyInformationResponse>());
            // 123/1 has been added - should return OK200

            var response = _classUnderTest.ViewRecord("123-1") as ObjectResult;

            response.Should().NotBeNull();
            response?.StatusCode.Should().Be(200);
        }

        [Test]
        public void ListTenanciesReturnsRecordsObtainedFromTheUseCase()
        {
            var queryParameters = new QueryParameters
            {
                Limit = _fixture.Create<int>(),
                Cursor = _fixture.Create<int>()
            };
            var stubbedResponse = new ListTenanciesResponse
            {
                Tenancies = _fixture.CreateMany<TenancyInformationResponse>().ToList()
            };
            _listTenancies
                .Setup(x => x.Execute(queryParameters.Limit, queryParameters.Cursor, It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(stubbedResponse);
            var response = _classUnderTest.ListTenancies(queryParameters) as ObjectResult;
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(stubbedResponse);
        }

        [Test]
        public void ListTenanciesWillAssignDefaultValuesToLimitAndCursor()
        {
            var stubbedResponse = new ListTenanciesResponse
            {
                Tenancies = _fixture.CreateMany<TenancyInformationResponse>().ToList()
            };
            _listTenancies
                .Setup(x => x.Execute(20, 0, It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(stubbedResponse);
            var response = _classUnderTest.ListTenancies(new QueryParameters()) as ObjectResult;
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(stubbedResponse);
        }

        [Test]
        public void ListTenanciesWillPassQueryParametersToTheUseCase()
        {
            var queryParameters = new QueryParameters
            {
                Address = _fixture.Create<string>(),
                Postcode = _fixture.Create<string>(),
                FreeholdsOnly = _fixture.Create<bool>(),
                LeaseholdsOnly = _fixture.Create<bool>()
            };
            _classUnderTest.ListTenancies(queryParameters);
            _listTenancies.Verify(x => x.Execute(It.IsAny<int>(), It.IsAny<int>(),
                queryParameters.Address, queryParameters.Postcode, queryParameters.LeaseholdsOnly, queryParameters.FreeholdsOnly));
        }

        [Test]
        public void ListTenanciesIfPostcodeIsValidWillReturn400ResponseCode()
        {
            _listTenancies
                .Setup(x => x.Execute(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new InvalidQueryParameterException("The parameters are all wrong"));
            var response = _classUnderTest.ListTenancies(new QueryParameters()) as ObjectResult;
            response.StatusCode.Should().Be(400);
            response.Value.Should().BeEquivalentTo("The parameters are all wrong");
        }
    }
}
