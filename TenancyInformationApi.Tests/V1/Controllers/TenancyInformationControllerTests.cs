using AutoFixture;
using TenancyInformationApi.V1.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using TenancyInformationApi.V1.UseCase.Interfaces;
using Moq;
using TenancyInformationApi.V1.Boundary.Response;

namespace TenancyInformationApi.Tests.V1.Controllers
{
    [TestFixture]
    public class TenancyInformationControllerTests
    {
        private TenancyInformationController _classUnderTest;
        private Mock<IGetTenancyByIdUseCase> _getByIdMock;

        [SetUp]
        public void SetUp()
        {
            _getByIdMock = new Mock<IGetTenancyByIdUseCase>();
            _classUnderTest = new TenancyInformationController(_getByIdMock.Object);
        }

        [Test]
        public void ViewRecordReturnsBadRequestForInvalidArguments()
        {
            // 1231 is not a valid tag_ref - should return BadRequest400
            var response = _classUnderTest.ViewRecord("1231") as ObjectResult;
            response.Should().NotBeNull();
            response?.StatusCode.Should().Be(400);
        }

        [Test]
        public void ViewRecordReturnsNotFoundForMissingRecord()
        {
            _getByIdMock.Setup(x => x.Execute("123/1")).Returns(new TenancyInformationResponse());
            // 123/1 is not in the db - should return NotFound404
            var response = _classUnderTest.ViewRecord("123-1") as ObjectResult;
            response?.StatusCode.Should().Be(404);
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
    }
}
