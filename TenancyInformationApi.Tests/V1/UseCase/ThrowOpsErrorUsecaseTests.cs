using System;
using TenancyInformationApi.V1.UseCase;
using FluentAssertions;
using NUnit.Framework;

namespace TenancyInformationApi.Tests.V1.UseCase
{
    [TestFixture]
    public class ThrowOpsErrorUsecaseTests
    {
        [Test]
        public void ThrowsTestOpsErrorException()
        {
            Action call = ThrowOpsErrorUsecase.Execute;
            call.Should().Throw<TestOpsErrorException>();
        }
    }
}
