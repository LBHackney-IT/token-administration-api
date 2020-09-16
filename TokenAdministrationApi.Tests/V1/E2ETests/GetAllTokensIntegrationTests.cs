using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Test]
        public async Task GettingTokensWithNoCursorAndLimitShouldReturnDefaultLimitAndNextCursor()
        {
            var addMultipleTokensToDb = Enumerable.Range(0, 25)
                .Select(x =>
                    AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(false, DatabaseContext, x + 1))
                .ToList();

            var url = new Uri($"/api/v1/tokens?enabled=false", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            response.StatusCode.Should().Be(200);
            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var tokens = JsonConvert.DeserializeObject<TokensListResponse>(data);
            tokens.Tokens.Count.Should().Be(20);
            tokens.Tokens.Should().BeEquivalentTo(addMultipleTokensToDb.Take(20));
            tokens.NextCursor.Should().Be("20");
        }

        [Test]
        public async Task IfNumberOfTokensInDbIsLessThanMaxLimitNextCursorShouldBeNull()
        {
            var addMultipleTokensToDb = Enumerable.Range(0, 10)
                .Select(x =>
                    AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(false, DatabaseContext, x + 1))
                .ToList();

            var url = new Uri($"/api/v1/tokens?enabled=false", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            response.StatusCode.Should().Be(200);
            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var tokens = JsonConvert.DeserializeObject<TokensListResponse>(data);
            tokens.Tokens.Count.Should().Be(10);
            tokens.Tokens.Should().BeEquivalentTo(addMultipleTokensToDb);
            tokens.NextCursor.Should().Be(null);
        }

        [Test]
        public async Task IfLimitAndCursorIsSuppliedShouldReturnCorrectSetOfTokens()
        {
            var addMultipleTokensToDb = Enumerable.Range(0, 15) //insert a number of tokens that given the limit and cursor would make 'NextCursor' null
                .Select(x =>
                    AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(false, DatabaseContext, x + 1))
                .ToList();

            var url = new Uri($"/api/v1/tokens?limit=11&cursor=11", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            response.StatusCode.Should().Be(200);
            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var tokens = JsonConvert.DeserializeObject<TokensListResponse>(data);
            tokens.Tokens.Count.Should().Be(4);
            tokens.Tokens.Should().BeEquivalentTo(addMultipleTokensToDb.Skip(11));
            tokens.NextCursor.Should().Be(null);
        }

        [Test]
        public async Task IfManyTokensInDbAndLimitAndCursorIsSuppliedShouldReturnCorrectSetOfTokens()
        {
            var addMultipleTokensToDb = Enumerable.Range(0, 35) //insert multiple tokens that will require a 'NextCursor' to be returned
                .Select(x =>
                    AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(false, DatabaseContext, x + 1))
                .ToList();

            var url = new Uri($"/api/v1/tokens?limit=11&cursor=11", UriKind.Relative);
            var response = await Client.GetAsync(url).ConfigureAwait(true);
            response.StatusCode.Should().Be(200);
            var data = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
            var tokens = JsonConvert.DeserializeObject<TokensListResponse>(data);
            tokens.Tokens.Count.Should().Be(11);
            tokens.Tokens.Should().BeEquivalentTo(addMultipleTokensToDb.Skip(11).Take(11));
            tokens.NextCursor.Should().Be("22");
        }
    }
}
