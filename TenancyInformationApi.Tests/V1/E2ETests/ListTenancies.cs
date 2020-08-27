using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
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
                E2ETestHelper.AddPersonWithRelatedEntitiesToDb(DatabaseContext, "12345/2", "a", "x"),
                E2ETestHelper.AddPersonWithRelatedEntitiesToDb(DatabaseContext, "54326/9", "b", "y"),
                E2ETestHelper.AddPersonWithRelatedEntitiesToDb(DatabaseContext, "453627/2", "c", "z"),
            };

            var response = await CallApiListEndpointWithQueryString("").ConfigureAwait(true);
            response.StatusCode.Should().Be(200);

            var returnedTenancies = await DeserializeResponse(response).ConfigureAwait(true);
            returnedTenancies.Tenancies.Should().BeEquivalentTo(expectedResponses);
        }

        [Test]
        public async Task WithNoCursorAndLimitWillReturnTheFirstPageOfTenanciesWithTheNextCursor()
        {
            var faker = new Faker();
            var allSavedEntities = Enumerable.Range(0, 25)
                .Select(r =>
                {
                    var tagRef = $"{r}{faker.Random.Int(1000, 9999):0000}/{faker.Random.Int(1, 9)}";
                    var agreementId = GetLetterFromAlphabetPosition(r);
                    var tenureId = GetLetterFromAlphabetPosition(r + 2);
                    return E2ETestHelper.AddPersonWithRelatedEntitiesToDb(DatabaseContext, tagRef, agreementId, tenureId);
                }).ToList();

            var response = await CallApiListEndpointWithQueryString("").ConfigureAwait(true);
            response.StatusCode.Should().Be(200);

            var returnedTenancies = await DeserializeResponse(response).ConfigureAwait(true);
            returnedTenancies.Tenancies.Should().BeEquivalentTo(allSavedEntities.Take(20));

            var finalTagRef = allSavedEntities.ElementAt(19).TenancyAgreementReference;
            var expectedCursor = $"{finalTagRef.Substring(0, 6)}{finalTagRef.Substring(7, 1)}";
            returnedTenancies.NextCursor.Should().Be(expectedCursor);
        }

        [Test]
        public async Task WithCursorAndLimitGivenWillCorrectlyPaginateTenanciesReturned()
        {
            var faker = new Faker();
            var allSavedEntities = Enumerable.Range(0, 17)
                .Select(r =>
                {
                    var tagRef = $"{r}{faker.Random.Int(1000, 9999):0000}/{faker.Random.Int(1, 9)}";
                    var agreementId = GetLetterFromAlphabetPosition(r);
                    var tenureId = GetLetterFromAlphabetPosition(r + 2);
                    return E2ETestHelper.AddPersonWithRelatedEntitiesToDb(DatabaseContext, tagRef, agreementId, tenureId);
                }).ToList();

            var finalTagRef = allSavedEntities.ElementAt(2).TenancyAgreementReference;
            var cursor = $"{finalTagRef.Substring(0, 5)}{finalTagRef.Substring(6, 1)}";

            var response = await CallApiListEndpointWithQueryString($"?limit=12&cursor={cursor}").ConfigureAwait(true);
            response.StatusCode.Should().Be(200);

            var returnedTenancies = await DeserializeResponse(response).ConfigureAwait(true);
            returnedTenancies.Tenancies.Should().BeEquivalentTo(allSavedEntities.Skip(3).Take(12));
        }

        [Test]
        public async Task WithAddressQueryParametersOnlyReturnMatchingTenancies()
        {
            var expectedResponses = new List<TenancyInformationResponse>
            {
                E2ETestHelper.AddPersonWithRelatedEntitiesToDb(DatabaseContext, "12345/2", "a", "x", "1 Hillman Road"),
                E2ETestHelper.AddPersonWithRelatedEntitiesToDb(DatabaseContext, "54326/9", "b", "y", "6 HillmanRoad"),
                E2ETestHelper.AddPersonWithRelatedEntitiesToDb(DatabaseContext, "453627/2", "c", "z", "8 Fox Street"),
            };

            var response = await CallApiListEndpointWithQueryString("?address=hillmanroad").ConfigureAwait(true);
            response.StatusCode.Should().Be(200);

            var returnedTenancies = await DeserializeResponse(response).ConfigureAwait(true);
            returnedTenancies.Tenancies.Count.Should().Be(2);
            returnedTenancies.Tenancies.Should().BeEquivalentTo(expectedResponses.GetRange(0, 2));
        }

        [Test]
        public async Task WithPostcodeQueryParametersOnlyReturnMatchingTenancies()
        {
            var expectedResponses = new List<TenancyInformationResponse>
            {
                E2ETestHelper.AddPersonWithRelatedEntitiesToDb(DatabaseContext, "12345/2", "a", "x", postcode: "E67YH"),
                E2ETestHelper.AddPersonWithRelatedEntitiesToDb(DatabaseContext, "54326/9", "b", "y", postcode: "e6  7yH"),
                E2ETestHelper.AddPersonWithRelatedEntitiesToDb(DatabaseContext, "453627/2", "c", "z", postcode: "SE9 5uh"),
            };

            var response = await CallApiListEndpointWithQueryString("?postcode=E67YH").ConfigureAwait(true);
            response.StatusCode.Should().Be(200);

            var returnedTenancies = await DeserializeResponse(response).ConfigureAwait(true);
            returnedTenancies.Tenancies.Count.Should().Be(2);
            returnedTenancies.Tenancies.Should().BeEquivalentTo(expectedResponses.GetRange(0, 2));
        }

        private static string GetLetterFromAlphabetPosition(int position)
        {
            // Being used to generate ordered string based ID's which are all unique. To help test pagination and prevent duplicate key errors in test setup.
            return Convert.ToChar(Enumerable.Range('a', 27).ElementAt(position)).ToString();
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
