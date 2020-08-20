using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenancyInformationApi.V1.Boundary.Response;
using TenancyInformationApi.V1.Infrastructure;

namespace TenancyInformationApi.Tests.V1.E2ETests
{
    [TestFixture]
    public class GetByID : IntegrationTests<Startup>
    {
        private IFixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
       
        public async Task GetResidentInformationByIdReturn200()
        {
            var tenancyReference = "XXXtestRef";
            var expectedResponse = E2ETestHelper.AddPersonWithRelatedEntitiestoDb(DatabaseContext, tenancyReference);
            var uri = new Uri($"api/v1/tenancies/{tenancyReference}", UriKind.Relative);
            var response = Client.GetAsync(uri);
            var statuscode = response.Result.StatusCode;
            statuscode.Should().Be(200);

            var content = response.Result.Content;
            var stringContent = await content.ReadAsStringAsync().ConfigureAwait(true);
            var convertedResponse = JsonConvert.DeserializeObject<TenancyInformationResponse>(stringContent);

            convertedResponse.Should().BeEquivalentTo(expectedResponse);
        }
        
    }
}
