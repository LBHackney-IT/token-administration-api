using System;
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
        public async Task CanCreateEndpointForApiLookupAsync()
        {
            var api = new ApiNameLookup
            {
                ApiName = "housing-api",
                ApiGatewayId = "gw-housing-dev"
            };
            DatabaseContext.ApiNameLookups.Add(api);
            DatabaseContext.SaveChanges();

            var request = new CreateEndpointRequest { EndpointName = "/tenancies" };
            var content = new StringContent(
                JsonConvert.SerializeObject(request),
                Encoding.UTF8,
                "application/json");

            var response = await Client.PostAsync(
                new Uri($"/api/v1/tokens/apis/{api.Id}/endpoints", UriKind.Relative),
                content).ConfigureAwait(true);
            content.Dispose();

            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var endpointResponse = JsonConvert.DeserializeObject<CreateEndpointResponse>(data);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            endpointResponse.ApiLookupId.Should().Be(api.Id);
            endpointResponse.ApiName.Should().Be(api.ApiName);
            endpointResponse.EndpointName.Should().Be(request.EndpointName);
        }
    }
}
