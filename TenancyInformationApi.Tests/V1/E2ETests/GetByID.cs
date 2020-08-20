using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
       
        public void GetResidentInformationByIdReturn400()
        {
            var tenancyReference = "XXXTest";
            var expectedResponse = E2ETestHelper.AddPersonWithRelatedEntitiestoDb(DatabaseContext, tenancyReference);
            var uri = new Uri($"api/v1/tenancies/{tenancyReference}", UriKind.Relative);
            var response = Client.GetAsync(uri);
            var statuscode = response.Result.StatusCode;
            statuscode.Should().Be(400);

            var content = response.Result.Content.ReadAsAsync<string>().Result;
            content.Should().BeEquivalentTo("tag_ref is malformed or missing.");
        }

        [Test]

        public void GetResidentInformationByIdReturn404()
        {
            var tenancyReference = "123-456";
            var uri = new Uri($"api/v1/tenancies/{tenancyReference}", UriKind.Relative);
            var response = Client.GetAsync(uri);
            var statuscode = response.Result.StatusCode;
            statuscode.Should().Be(404);
        }

        [Test]
        public async Task GetResidentInformationByIdReturn200()
        {
            var tenancyReference = "1234/456";
            var expectedResponse = E2ETestHelper.AddPersonWithRelatedEntitiestoDb(DatabaseContext, tenancyReference);
            tenancyReference = "1234-456";
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
