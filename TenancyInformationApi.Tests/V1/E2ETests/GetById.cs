using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TenancyInformationApi.V1.Boundary.Response;


namespace TenancyInformationApi.Tests.V1.E2ETests
{
    [TestFixture]
    public class GetById : IntegrationTests<Startup>
    {
        [Test]
        public void GetResidentInformationByIdReturn400()
        {
            var tenancyReference = "XXXTest";
            var uri = new Uri($"api/v1/tenancies/{tenancyReference}", UriKind.Relative);
            var response = Client.GetAsync(uri);
            var statusCode = response.Result.StatusCode;
            statusCode.Should().Be(400);

            var content = response.Result.Content.ReadAsAsync<string>().Result;
            content.Should().BeEquivalentTo("tag_ref is malformed or missing.");
        }

        [Test]

        public void GetResidentInformationByIdReturn404()
        {
            var tenancyReference = "123-456";
            var uri = new Uri($"api/v1/tenancies/{tenancyReference}", UriKind.Relative);
            var response = Client.GetAsync(uri);
            var statusCode = response.Result.StatusCode;
            statusCode.Should().Be(404);
        }

        [Test]
        public async Task GetResidentInformationByIdReturn200()
        {
            var tenancyReference = "1234/456";
            var expectedResponse = E2ETestHelper.AddPersonWithRelatedEntitiesToDb(DatabaseContext, tenancyReference);
            RemoveAddressAndResidentExpectedDetails(expectedResponse);

            var uri = new Uri($"api/v1/tenancies/1234-456", UriKind.Relative);
            var response = Client.GetAsync(uri);
            var statusCode = response.Result.StatusCode;
            statusCode.Should().Be(200);

            var content = response.Result.Content;
            var stringContent = await content.ReadAsStringAsync().ConfigureAwait(true);
            var convertedResponse = JsonConvert.DeserializeObject<TenancyInformationResponse>(stringContent);

            convertedResponse.Should().BeEquivalentTo(expectedResponse);
        }

        private static void RemoveAddressAndResidentExpectedDetails(TenancyInformationResponse expectedResponse)
        {
            expectedResponse.Address = null;
            expectedResponse.Postcode = null;
            expectedResponse.Residents = null;
        }
    }
}
