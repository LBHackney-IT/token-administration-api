using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using TokenAdministrationApi.Tests.V1.Helper;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.Infrastructure;

namespace TokenAdministrationApi.Tests.V1.E2ETests
{
    public class PostTokenIntegrationTests : IntegrationTests<Startup>
    {
        private readonly Faker _faker = new Faker();
        [Test]
        public async Task CanGenerateAnAuthTokenAsync()
        {
            var (apiLookup, apiEndpointLookup, consumerTypeLookup) = AddLookupsToDatabase();
            var tokenRequest = new TokenRequestObject
            {
                Consumer = _faker.Random.AlphaNumeric(10),
                ConsumerType = consumerTypeLookup.Id,
                ExpiresAt = _faker.Date.Future(),
                ApiEndpoint = apiEndpointLookup.Id,
                ApiName = apiLookup.Id,
                HttpMethodType = "GET",
                AuthorizedBy = _faker.Person.Email,
                Environment = _faker.Random.AlphaNumeric(5),
                RequestedBy = _faker.Person.Email
            };
            Environment.SetEnvironmentVariable("jwtSecret", _faker.Random.String());
            var jwtSecret = Environment.GetEnvironmentVariable("jwtSecret");

            var url = new Uri($"/api/v1/tokens", UriKind.Relative);
            var content = new StringContent(JsonConvert.SerializeObject(tokenRequest), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(url, content).ConfigureAwait(true);
            content.Dispose();

            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var apiResponse = JsonConvert.DeserializeObject<GenerateTokenResponse>(data);

            var claimsDecrypted = ValidateJwtTokenHelper.GetJwtClaims(apiResponse.Token, jwtSecret);

            response.StatusCode.Should().Be(201);

            claimsDecrypted.Find(x => x.Type == "id").Value.Should().Be(apiResponse.Id.ToString(CultureInfo.InvariantCulture));
            claimsDecrypted.Find(x => x.Type == "consumerName").Value.Should().Be(tokenRequest.Consumer);
            claimsDecrypted.Find(x => x.Type == "consumerType").Value.Should()
                .Be(tokenRequest.ConsumerType.ToString(CultureInfo.InvariantCulture));
            apiResponse.Should().BeOfType<GenerateTokenResponse>();
            apiResponse.GeneratedAt.Date.Should().Be(DateTime.Now.Date);
            apiResponse.ExpiresAt.Value.Should().BeSameDateAs(tokenRequest.ExpiresAt.Value);
        }
        [Test]
        public async Task Return400IfRequestParametersAreMissing()
        {
            var request = new TokenRequestObject()
            {
                ApiEndpoint = 1,
                ApiName = 2,
                Consumer = "test"
            };

            var url = new Uri($"/api/v1/tokens", UriKind.Relative);
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(url, content).ConfigureAwait(true);
            content.Dispose();
            response.StatusCode.Should().Be(400);
        }

        [Test]
        public async Task Return400IfExpiryDateSuppliedIsNotInTheFuture()
        {
            var tokenRequest = new TokenRequestObject
            {
                Consumer = _faker.Random.AlphaNumeric(10),
                ConsumerType = _faker.Random.Int(5),
                ExpiresAt = _faker.Date.Past(),
                ApiEndpoint = _faker.Random.Int(0, 10),
                ApiName = _faker.Random.Int(0, 10),
                HttpMethodType = "GET",
                AuthorizedBy = _faker.Person.Email,
                Environment = _faker.Random.AlphaNumeric(5),
                RequestedBy = _faker.Person.Email
            };

            var url = new Uri($"/api/v1/tokens", UriKind.Relative);
            var content = new StringContent(JsonConvert.SerializeObject(tokenRequest), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(url, content).ConfigureAwait(true);
            content.Dispose();
            response.StatusCode.Should().Be(400);
        }
        [Test]
        public async Task Returns201IfAllRequestParametersButExpiresAtAreSupplied()
        {
            var (apiLookup, apiEndpointLookup, consumerTypeLookup) = AddLookupsToDatabase();
            var tokenRequest = new TokenRequestObject
            {
                Consumer = _faker.Random.AlphaNumeric(10),
                ConsumerType = consumerTypeLookup.Id,
                ApiEndpoint = apiEndpointLookup.Id,
                ApiName = apiLookup.Id,
                HttpMethodType = "GET",
                AuthorizedBy = _faker.Person.Email,
                Environment = _faker.Random.AlphaNumeric(5),
                RequestedBy = _faker.Person.Email
            };
            Environment.SetEnvironmentVariable("jwtSecret", _faker.Random.String());

            var url = new Uri($"/api/v1/tokens", UriKind.Relative);
            var content = new StringContent(JsonConvert.SerializeObject(tokenRequest), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(url, content).ConfigureAwait(true);
            content.Dispose();
            response.StatusCode.Should().Be(201);
        }
        [Test]
        public async Task Return400IfHttpMethodTypeSuppliedIsInvalid()
        {
            var tokenRequest = new TokenRequestObject
            {
                Consumer = _faker.Random.AlphaNumeric(10),
                ConsumerType = _faker.Random.Int(5),
                ApiEndpoint = _faker.Random.Int(0, 10),
                ApiName = _faker.Random.Int(0, 10),
                HttpMethodType = "TEST",
                AuthorizedBy = _faker.Person.Email,
                Environment = _faker.Random.AlphaNumeric(5),
                RequestedBy = _faker.Person.Email
            };

            var url = new Uri($"/api/v1/tokens", UriKind.Relative);
            var content = new StringContent(JsonConvert.SerializeObject(tokenRequest), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(url, content).ConfigureAwait(true);
            content.Dispose();
            response.StatusCode.Should().Be(400);
        }

        [Test]
        public async Task Return400IfLookupIdsDoNotExist()
        {
            var tokenRequest = new TokenRequestObject
            {
                Consumer = _faker.Random.AlphaNumeric(10),
                ConsumerType = _faker.Random.Int(5),
                ApiEndpoint = _faker.Random.Int(0, 10),
                ApiName = _faker.Random.Int(0, 10),
                HttpMethodType = "GET",
                AuthorizedBy = _faker.Person.Email,
                Environment = _faker.Random.AlphaNumeric(5),
                RequestedBy = _faker.Person.Email
            };
            Environment.SetEnvironmentVariable("jwtSecret", _faker.Random.String());

            var url = new Uri($"/api/v1/tokens", UriKind.Relative);
            var content = new StringContent(JsonConvert.SerializeObject(tokenRequest), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync(url, content).ConfigureAwait(true);
            content.Dispose();
            response.StatusCode.Should().Be(400);
        }

        private (ApiNameLookup, ApiEndpointNameLookup, ConsumerTypeLookup) AddLookupsToDatabase()
        {
            var api = new ApiNameLookup
            {
                ApiName = _faker.Random.Word()
            };
            var apiEndpoint = new ApiEndpointNameLookup
            {
                ApiEndpointName = _faker.Internet.UrlWithPath()
            };
            var consumerType = new ConsumerTypeLookup
            {
                TypeName = _faker.Random.Word()
            };
            DatabaseContext.ApiNameLookups.Add(api);
            DatabaseContext.ApiEndpointNameLookups.Add(apiEndpoint);
            DatabaseContext.ConsumerTypeLookups.Add(consumerType);
            DatabaseContext.SaveChanges();
            return (api, apiEndpoint, consumerType);
        }
    }
}
