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
        private string _tenancyRef;
        private Tenancy _tenancy;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<ITenancyGateway>();
            _classUnderTest = new GetTenancyByIdUseCase(_mockGateway.Object);

            _tenancyRef = _fixture.Create<string>();
            _tenancy = _fixture.Create<Tenancy>();

        }

        [TearDown] public void TearDown() => _mockGateway.Reset();

        [Test]
        public void CanRetrieveTenancyById()
        {
            _mockGateway.Setup(x => x.GetById(_tenancyRef)).Returns(_tenancy);

            var result = _classUnderTest.Execute(_tenancyRef);

            result.Should().NotBeNull();
            result.Should().BeOfType<TenancyInformationResponse>();
            result.Should().BeEquivalentTo(_tenancy.ToResponse());
        }

        [Test]
        public void CanHandleNullTenancyObjects()
        {
            var result = _classUnderTest.Execute(_tenancyRef);

            result.Should().NotBeNull();
            result.Should().BeOfType<TenancyInformationResponse>();
        }
    }
}
