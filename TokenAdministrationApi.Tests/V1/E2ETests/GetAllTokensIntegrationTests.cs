using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TokenAdministrationApi.Tests.V1.Helper;
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.Domain;

namespace TokenAdministrationApi.Tests.V1.E2ETests
{
    public class GetAllTokensIntegrationTests : IntegrationTests<Startup>
    {
        [Test]
        public async Task CanRetrieveAllTokens()
        {
            var enabledToken = AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(true, DatabaseContext);
            var disabledToken = AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(false, DatabaseContext);

            var expectedResponse = new TokensListResponse()
            {
                Tokens = new List<AuthToken>()
               {enabledToken, disabledToken }
            };

            var url = new Uri($"/api/v1/tokens", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            response.StatusCode.Should().Be(200);
            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var tokens = JsonConvert.DeserializeObject<TokensListResponse>(data);
            tokens.Tokens.Count.Should().Be(2);
            tokens.Should().BeEquivalentTo(expectedResponse);
        }
        [Test]
        public async Task CanRetrieveOnlyEnabledTokens()
        {
            var enabledToken = AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(true, DatabaseContext);
            AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(false, DatabaseContext); //add disabled token

            var expectedResponse = new TokensListResponse()
            {
                Tokens = new List<AuthToken>()
                {enabledToken }
            };

            var url = new Uri($"/api/v1/tokens?enabled=true", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            response.StatusCode.Should().Be(200);
            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var tokens = JsonConvert.DeserializeObject<TokensListResponse>(data);
            tokens.Tokens.Count.Should().Be(1);
            tokens.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public async Task CanRetrieveOnlyDisabledTokens()
        {
            AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(true, DatabaseContext); //add enabled token
            var disabledtoken = AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(false, DatabaseContext);

            var expectedResponse = new TokensListResponse()
            {
                Tokens = new List<AuthToken>()
                {disabledtoken }
            };

            var url = new Uri($"/api/v1/tokens?enabled=false", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            response.StatusCode.Should().Be(200);
            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var tokens = JsonConvert.DeserializeObject<TokensListResponse>(data);
            tokens.Tokens.Count.Should().Be(1);
            tokens.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
