using System;
using AutoFixture;
using FluentAssertions;
using TenancyInformationApi.V1.Gateways;
using TenancyInformationApi.V1.UseCase;
using Moq;
using NUnit.Framework;
using TenancyInformationApi.V1.Boundary.Response;
using TenancyInformationApi.V1.Domain;
using TenancyInformationApi.V1.Factories;

namespace TenancyInformationApi.Tests.V1.UseCase
{
    public class GetByIdUseCaseTests
    {
        private Mock<ITenancyGateway> _mockGateway;
        private GetTenancyByIdUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<ITenancyGateway>();
            _classUnderTest = new GetTenancyByIdUseCase(_mockGateway.Object);
        }

        [Test]
        public void CanRetrieveTenancyById()
        {
            var tenancyRef = _fixture.Create<string>();
            var tenancy = _fixture.Create<Tenancy>();
            _mockGateway.Setup(x => x.GetById(tenancyRef)).Returns(tenancy);

            var result = _classUnderTest.Execute(tenancyRef);

            result.Should().NotBeNull();
            result.Should().BeOfType<TenancyInformationResponse>();
            result.Should().BeEquivalentTo(tenancy.ToResponse());
        }
    }
}
