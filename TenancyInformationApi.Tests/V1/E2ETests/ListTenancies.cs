using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using TenancyInformationApi.V1.Boundary.Response;

namespace TenancyInformationApi.Tests.V1.E2ETests
{
    public class ListTenancies : IntegrationTests<Startup>
    {
        [Test]
        public async Task IfThereAreNoTenanciesItWillReturnAnEmptyList()
        {
            var response = await CallApiListEndpointWithQueryString("").ConfigureAwait(true);
            response.StatusCode.Should().Be(200);

            var returnedTenancies = await DeserializeResponse(response).ConfigureAwait(true);
            returnedTenancies.Tenancies.Should().BeEmpty();
        }

        [Test]
        public async Task WithNoQueryParametersWillListAllStoredTenancies()
        {
            var expectedResponses = new List<TenancyInformationResponse>
            {
                E2ETestHelper.AddPersonWithRelatedEntitiesToDb(DatabaseContext),
                E2ETestHelper.AddPersonWithRelatedEntitiesToDb(DatabaseContext),
                E2ETestHelper.AddPersonWithRelatedEntitiesToDb(DatabaseContext),
            };

            var response = await CallApiListEndpointWithQueryString("").ConfigureAwait(true);
            response.StatusCode.Should().Be(200);

            var returnedTenancies = await DeserializeResponse(response).ConfigureAwait(true);
            returnedTenancies.Tenancies.Should().BeEquivalentTo(expectedResponses);
        }

        private async Task<HttpResponseMessage> CallApiListEndpointWithQueryString(string query)
        {
            var uri = new Uri($"api/v1/tenancies{query}", UriKind.Relative);
            return await Client.GetAsync(uri).ConfigureAwait(true);
        }

        private static async Task<ListTenanciesResponse> DeserializeResponse(HttpResponseMessage response)
        {
            var stringContent = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var returnedTenancies = JsonConvert.DeserializeObject<ListTenanciesResponse>(stringContent);
            return returnedTenancies;
        }
    }
}
