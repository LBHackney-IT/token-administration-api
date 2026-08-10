using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.Infrastructure;

namespace TokenAdministrationApi.Tests.V1.E2ETests
{
    public class PostApiConfigurationIntegrationTests : IntegrationTests<Startup>
    {
        [Test]
        public async Task PostApiWithMissingFieldsReturnsBadRequest()
        {
            var apiCount = DatabaseContext.ApiNameLookups.Count();
            var response = await PostJsonAsync("/api/v1/tokens/apis", new { });
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseBody.Should().Contain("ApiName");
            responseBody.Should().Contain("ApiGatewayId");
            DatabaseContext.ApiNameLookups.Count().Should().Be(apiCount);
        }

        [Test]
        public async Task PostEndpointWithMissingFieldsReturnsBadRequest()
        {
            var api = AddApiLookup();
            var endpointCount = DatabaseContext.ApiEndpointNameLookups.Count();
            var response = await PostJsonAsync($"/api/v1/tokens/apis/{api.Id}/endpoints", new { });
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseBody.Should().Contain("EndpointName");
            DatabaseContext.ApiEndpointNameLookups.Count().Should().Be(endpointCount);
        }

        [Test]
        public async Task PostApiWithOverLengthGatewayIdReturnsBadRequest()
        {
            var apiCount = DatabaseContext.ApiNameLookups.Count();
            var request = new CreateApiLookupRequest
            {
                ApiName = "housing-api",
                ApiGatewayId = new string('a', 17)
            };
            var response = await PostJsonAsync("/api/v1/tokens/apis", request);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseBody.Should().Contain("ApiGatewayId");
            DatabaseContext.ApiNameLookups.Count().Should().Be(apiCount);
        }

        [Test]
        public async Task PostApiWithBlankFieldsReturnsBadRequest()
        {
            var apiCount = DatabaseContext.ApiNameLookups.Count();
            var request = new CreateApiLookupRequest
            {
                ApiName = string.Empty,
                ApiGatewayId = " "
            };
            var response = await PostJsonAsync("/api/v1/tokens/apis", request);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseBody.Should().Contain("ApiName");
            responseBody.Should().Contain("ApiGatewayId");
            DatabaseContext.ApiNameLookups.Count().Should().Be(apiCount);
        }

        [Test]
        public async Task PostEndpointWithBlankNameReturnsBadRequest()
        {
            var api = AddApiLookup();
            var endpointCount = DatabaseContext.ApiEndpointNameLookups.Count();
            var request = new CreateEndpointRequest
            {
                EndpointName = " "
            };
            var response = await PostJsonAsync($"/api/v1/tokens/apis/{api.Id}/endpoints", request);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseBody.Should().Contain("EndpointName");
            DatabaseContext.ApiEndpointNameLookups.Count().Should().Be(endpointCount);
        }

        [Test]
        public async Task PostEndpointWithUnknownParentReturnsNotFound()
        {
            var endpointCount = DatabaseContext.ApiEndpointNameLookups.Count();
            var request = new CreateEndpointRequest { EndpointName = "/tenancies" };
            var response = await PostJsonAsync($"/api/v1/tokens/apis/{int.MaxValue}/endpoints", request);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            responseBody.Should().Contain("API lookup was not found.");
            DatabaseContext.ApiEndpointNameLookups.Count().Should().Be(endpointCount);
        }

        [Test]
        public async Task CanCreateEndpointForApiLookupAsync()
        {
            var api = AddApiLookup();
            var request = new CreateEndpointRequest { EndpointName = "/tenancies" };
            var response = await PostJsonAsync($"/api/v1/tokens/apis/{api.Id}/endpoints", request);
            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var endpointResponse = JsonConvert.DeserializeObject<CreateEndpointResponse>(data);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            endpointResponse.ApiLookupId.Should().Be(api.Id);
            endpointResponse.ApiName.Should().Be(api.ApiName);
            endpointResponse.EndpointName.Should().Be(request.EndpointName);
        }

        private async Task<HttpResponseMessage> PostJsonAsync(string path, object request)
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(request),
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync(
                new Uri(path, UriKind.Relative),
                content).ConfigureAwait(true);
            content.Dispose();

            return response;
        }

        private ApiNameLookup AddApiLookup()
        {
            var api = new ApiNameLookup
            {
                ApiName = "housing-api",
                ApiGatewayId = "gw-housing-dev"
            };
            DatabaseContext.ApiNameLookups.Add(api);
            DatabaseContext.SaveChanges();

            return api;
        }
    }
}
