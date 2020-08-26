using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using TenancyInformationApi.V1.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TenancyInformationApi.V1.UseCase.Interfaces;
using Moq;
using TenancyInformationApi.V1.Boundary.Response;
using TenancyInformationApi.V1.UseCase;

namespace TenancyInformationApi.Tests.V1.Controllers
{
    [TestFixture]
    public class TenancyInformationControllerTests
    {
        private TenancyInformationController _classUnderTest;
        private Mock<IGetTenancyByIdUseCase> _getByIdMock;
        private Mock<IListTenancies> _listTenancies;

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
            var fixture = new Fixture();
            var stubbedResponse = new ListTenanciesResponse
            {
                Tenancies = fixture.CreateMany<TenancyInformationResponse>().ToList()
            };
            _listTenancies.Setup(x => x.Execute()).Returns(stubbedResponse);
            var response = _classUnderTest.ListTenancies() as ObjectResult;
            response.StatusCode.Should().Be(200);
            response.Value.Should().BeEquivalentTo(stubbedResponse);
        }
    }
}
