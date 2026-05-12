using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.Infrastructure;

namespace TokenAdministrationApi.Tests.V1.E2ETests
{
    public class GetTokenOptionsIntegrationTests : IntegrationTests<Startup>
    {
        [Test]
        public async Task CanRetrieveTokenOptions()
        {
            var consumerType = new ConsumerTypeLookup
            {
                TypeName = "token-options-consumer"
            };
            DatabaseContext.ConsumerTypeLookups.Add(consumerType);
            DatabaseContext.SaveChanges();

            var api = new ApiNameLookup
            {
                ApiName = "contracts-api",
                ApiGatewayId = "gw-test-1234567"
            };
            DatabaseContext.ApiNameLookups.Add(api);
            DatabaseContext.SaveChanges();

            var apiEndpoint = new ApiEndpointNameLookup
            {
                ApiLookupId = api.Id,
                ApiEndpointName = "/api/v1/token-options-test"
            };
            DatabaseContext.ApiEndpointNameLookups.Add(apiEndpoint);
            DatabaseContext.SaveChanges();

            var url = new Uri("/api/v1/tokens/options", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);

            response.StatusCode.Should().Be(200);

            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var tokenOptions = JsonConvert.DeserializeObject<TokenOptionsResponse>(data);

            tokenOptions.Should().NotBeNull();
            tokenOptions.ConsumerTypes.Should().ContainSingle();
            tokenOptions.ApiLookups.Should().ContainSingle();
            tokenOptions.ApiEndpoints.Should().ContainSingle();

            tokenOptions.ConsumerTypes[0].TypeName.Should().Be("token-options-consumer");
            tokenOptions.ApiLookups[0].ApiName.Should().Be("contracts-api");
            tokenOptions.ApiLookups[0].ApiGatewayId.Should().Be("gw-test-1234567");
            tokenOptions.ApiEndpoints[0].EndpointName.Should().Be("/api/v1/token-options-test");
            tokenOptions.ApiEndpoints[0].ApiLookupId.Should().Be(api.Id);
        }
    }
}
