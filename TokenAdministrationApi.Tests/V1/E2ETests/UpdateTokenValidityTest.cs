using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using TokenAdministrationApi.V1.Infrastructure;

namespace TokenAdministrationApi.Tests.V1.E2ETests
{
    public class UpdateTokenValidityTest : IntegrationTests<Startup>
    {
        private readonly IFixture _fixture = new Fixture();

        [Test]
        public async Task IfTheTokenRecordIsFoundCanInvalidateAToken()
        {
            var savedToken = AddTokenToDatabase(true);

            var response = await CallPatchEndpointToDisableToken(savedToken.Id).ConfigureAwait(true);

            response.StatusCode.Should().Be(204);
            DatabaseContext.Tokens.Find(savedToken.Id).Enabled.Should().BeFalse();
        }

        [Test]
        public async Task IfTheTokenRecordIsFoundCanValidateAToken()
        {
            var savedToken = AddTokenToDatabase(false);

            var response = await CallPatchEndpointToEnableToken(savedToken.Id).ConfigureAwait(true);

            response.StatusCode.Should().Be(204);
            DatabaseContext.Tokens.Find(savedToken.Id).Enabled.Should().BeTrue();
        }

        [Test]
        public async Task IfTheTokenCanNotBeFoundReturnsNotFound()
        {
            var response = await CallPatchEndpointToDisableToken(6).ConfigureAwait(true);
            response.StatusCode.Should().Be(404);
        }

        [Test]
        public async Task IfTheTokenIdIsNotAnIntegerReturnsBadRequest()
        {
            var response = await CallPatchEndpointToDisableToken(tokenStrId: "f").ConfigureAwait(true);
            response.StatusCode.Should().Be(400);
        }

        private async Task<HttpResponseMessage> CallPatchEndpointToDisableToken(int? tokenId = null, string tokenStrId = null)
        {
            return await CallPatchEndpointWithRequest(tokenId?.ToString() ?? tokenStrId, request: "{\"enabled\": false }")
                .ConfigureAwait(true);
        }

        private async Task<HttpResponseMessage> CallPatchEndpointToEnableToken(int tokenId)
        {
            return await CallPatchEndpointWithRequest(tokenId.ToString(), request: "{\"enabled\": true }")
                .ConfigureAwait(true);
        }

        private async Task<HttpResponseMessage> CallPatchEndpointWithRequest(string tokenId, string request = null)
        {
            var url = new Uri($"/api/v1/tokens/{tokenId}", UriKind.Relative);
            var updateRequest = request;
            var content = new StringContent(updateRequest, Encoding.UTF8, "application/json");
            var response = await Client.PatchAsync(url, content).ConfigureAwait(true);
            content.Dispose();

            return response;
        }

        private AuthTokens AddTokenToDatabase(bool enabled)
        {
            var savedToken = _fixture.Create<AuthTokens>();
            savedToken.Enabled = enabled;
            DatabaseContext.Tokens.Add(savedToken);
            DatabaseContext.SaveChanges();
            return savedToken;
        }
    }
}
