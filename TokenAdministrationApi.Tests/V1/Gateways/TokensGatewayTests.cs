using System;
using System.Globalization;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using TokenAdministrationApi.V1.Gateways;
using NUnit.Framework;
using TokenAdministrationApi.V1.Infrastructure;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.Tests.V1.Helper;
using TokenAdministrationApi.V1.Domain;
using System.Collections.Generic;
using TokenAdministrationApi.V1.Domain.Exceptions;

namespace TokenAdministrationApi.Tests.V1.Gateways
{
    [TestFixture]
    public class TokensGatewayTests : DatabaseTests
    {
        private readonly IFixture _fixture = new Fixture();
        private TokensGateway _classUnderTest;
        [SetUp]
        public void Setup()
        {
            _classUnderTest = new TokensGateway(DatabaseContext);
        }

        [Test]
        public void ShouldGetAllTokensFromDatabase()
        {
            var token = AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(null, DatabaseContext);
            var disabledToken = AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(false, DatabaseContext);
            var result = _classUnderTest.GetAllTokens(10, 0, null);

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result.Find(x => x.Id == token.Id).ApiEndpointName.Should().Be(token.ApiEndpointName);
            result.Find(x => x.Id == token.Id).ApiName.Should().Be(token.ApiName);
            result.Find(x => x.Id == token.Id).ConsumerName.Should().Be(token.ConsumerName);
            result.Find(x => x.Id == token.Id).ConsumerType.Should().Be(token.ConsumerType);
            result.Find(x => x.Id == token.Id).Enabled.Should().Be(token.Enabled);
            result.Find(x => x.Id == token.Id).Environment.Should().Be(token.Environment);
            result.Find(x => x.Id == token.Id).ExpirationDate.Should().Be(token.ExpirationDate);
            result.Find(x => x.Id == token.Id).HttpMethodType.Should().Be(token.HttpMethodType);
            result.Find(x => x.Id == token.Id).Id.Should().Be(token.Id);

            result.Find(x => x.Id == disabledToken.Id).ApiEndpointName.Should().Be(disabledToken.ApiEndpointName);
            result.Find(x => x.Id == disabledToken.Id).ApiName.Should().Be(disabledToken.ApiName);
            result.Find(x => x.Id == disabledToken.Id).ConsumerName.Should().Be(disabledToken.ConsumerName);
            result.Find(x => x.Id == disabledToken.Id).ConsumerType.Should().Be(disabledToken.ConsumerType);
            result.Find(x => x.Id == disabledToken.Id).Enabled.Should().Be(disabledToken.Enabled);
            result.Find(x => x.Id == disabledToken.Id).Environment.Should().Be(disabledToken.Environment);
            result.Find(x => x.Id == disabledToken.Id).ExpirationDate.Should().Be(disabledToken.ExpirationDate);
            result.Find(x => x.Id == disabledToken.Id).HttpMethodType.Should().Be(disabledToken.HttpMethodType);
            result.Find(x => x.Id == disabledToken.Id).Id.Should().Be(disabledToken.Id);
        }
        [Test]
        public void ShouldGetOnlyDisabledTokensFromDatabase()
        {
            AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(true, DatabaseContext);
            AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(false, DatabaseContext);
            var result = _classUnderTest.GetAllTokens(10, 0, false);

            result.Should().NotBeNull();
            result.Count.Should().Be(1);
        }

        [Test]
        public void ShouldGetOnlyEnabledTokensFromDatabase()
        {
            AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(true, DatabaseContext);
            AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(true, DatabaseContext);
            AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(false, DatabaseContext);
            var result = _classUnderTest.GetAllTokens(10, 0, true);

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }

        [Test]
        public void IfNoTokensMatchingCriteriaAreFoundShouldReturnEmptyListOfTokens()
        {
            var result = _classUnderTest.GetAllTokens(10, 0, true);
            result.Should().NotBeNull();
            result.Count.Should().Be(0);
            result.Should().BeEquivalentTo(new List<AuthToken>());
        }

        [Test]
        public void InsertingATokenRecordShouldReturnAnId()
        {
            var tokenWithLookUpValues = AddTokenLookupValues();
            var tokenRequest = _fixture.Build<TokenRequestObject>()
                .With(x => x.ApiEndpoint, tokenWithLookUpValues.ApiEndpointNameLookupId)
                .With(x => x.ApiName, tokenWithLookUpValues.ApiLookupId)
                .With(x => x.ConsumerType, tokenWithLookUpValues.ConsumerTypeLookupId)
                .Create();

            var response = _classUnderTest.GenerateToken(tokenRequest);

            response.Should().NotBe(0);
        }
        [Test]
        public void InsertedRecordShouldBeInsertedOnceInTheDatabase()
        {
            var tokenWithLookUpValues = AddTokenLookupValues();
            var tokenRequest = _fixture.Build<TokenRequestObject>()
                .With(x => x.ApiEndpoint, tokenWithLookUpValues.ApiEndpointNameLookupId)
                .With(x => x.ApiName, tokenWithLookUpValues.ApiLookupId)
                .With(x => x.ConsumerType, tokenWithLookUpValues.ConsumerTypeLookupId)
                .Create();

            var response = _classUnderTest.GenerateToken(tokenRequest);

            var databaseRecord = DatabaseContext.Tokens.Where(x => x.Id == response);
            var defaultRecordRetrieved = databaseRecord.FirstOrDefault();

            databaseRecord.Count().Should().Be(1);
        }
        [Test]
        public void InsertedRecordShouldBeInTheDatabase()
        {
            var tokenWithLookUpValues = AddTokenLookupValues();
            var tokenRequest = _fixture.Build<TokenRequestObject>()
                .With(x => x.ApiEndpoint, tokenWithLookUpValues.ApiEndpointNameLookupId)
                .With(x => x.ApiName, tokenWithLookUpValues.ApiLookupId)
                .With(x => x.ConsumerType, tokenWithLookUpValues.ConsumerTypeLookupId)
                .Create();

            var response = _classUnderTest.GenerateToken(tokenRequest);

            var databaseRecord = DatabaseContext.Tokens.Where(x => x.Id == response);
            var defaultRecordRetrieved = databaseRecord.FirstOrDefault();

            defaultRecordRetrieved.RequestedBy.Should().Be(tokenRequest.RequestedBy);
            defaultRecordRetrieved.Enabled.Should().BeTrue();
            defaultRecordRetrieved.ExpirationDate.Should().Be(tokenRequest.ExpiresAt);
            defaultRecordRetrieved.DateCreated.Date.Should().Be(DateTime.Now.Date);
            defaultRecordRetrieved.Environment.Should().Be(tokenRequest.Environment);
            defaultRecordRetrieved.HttpMethodType.Should().Be(tokenRequest.HttpMethodType.ToUpper(CultureInfo.InvariantCulture));
            defaultRecordRetrieved.ConsumerTypeLookupId.Should().Be(tokenRequest.ConsumerType);
            defaultRecordRetrieved.ConsumerName.Should().Be(tokenRequest.Consumer);
            defaultRecordRetrieved.AuthorizedBy.Should().Be(tokenRequest.AuthorizedBy);
            defaultRecordRetrieved.ApiEndpointNameLookupId.Should().Be(tokenRequest.ApiEndpoint);
            defaultRecordRetrieved.ApiLookupId.Should().Be(tokenRequest.ApiName);
        }

        [Test]
        public void UpdateTokenReturnsNullIfTokenDoesntExist()
        {
            var response = _classUnderTest.UpdateToken(3, false);
            response.Should().BeNull();
        }

        [Test]
        public void UpdateTokenWillChangeEnabledToTrue()
        {
            var token = _fixture.Create<AuthTokens>();
            token.Enabled = false;
            DatabaseContext.Tokens.Add(token);
            DatabaseContext.SaveChanges();

            _classUnderTest.UpdateToken(token.Id, true);
            DatabaseContext.Tokens.Find(token.Id).Enabled.Should().BeTrue();
        }

        [Test]
        public void UpdateTokenWillChangeEnabledToFalse()
        {
            var token = _fixture.Create<AuthTokens>();
            token.Enabled = true;
            DatabaseContext.Tokens.Add(token);
            DatabaseContext.SaveChanges();

            _classUnderTest.UpdateToken(token.Id, false);
            DatabaseContext.Tokens.Find(token.Id).Enabled.Should().BeFalse();
        }

        [Test]
        public void UpdateTokenWillReturnId()
        {
            var token = _fixture.Create<AuthTokens>();
            token.Enabled = false;
            DatabaseContext.Tokens.Add(token);
            DatabaseContext.SaveChanges();

            var response = _classUnderTest.UpdateToken(token.Id, true);
            response.Should().Be(token.Id);
        }

        [Test]
        public void UpdateTokenShouldNotChangeAnyOtherFields()
        {
            var token = _fixture.Create<AuthTokens>();
            token.Enabled = false;
            DatabaseContext.Tokens.Add(token);
            DatabaseContext.SaveChanges();

            _classUnderTest.UpdateToken(token.Id, true);

            DatabaseContext.Tokens.Find(token.Id).Should()
                .BeEquivalentTo(token, options => options.Excluding(x => x.Enabled));
        }

        [Test]
        public void InsertTokenShouldThrowLookupValueDoesNotExistExceptionIfLookupValueDoesNotExist()
        {
            var tokenRequest = _fixture.Build<TokenRequestObject>()
                .With(x => x.ApiEndpoint, 999) //random number that does not exist
                .Create();

            Func<int> testDelegate = () => _classUnderTest.GenerateToken(tokenRequest);
            testDelegate.Should().Throw<LookupValueDoesNotExistException>();

        }
        private AuthTokens AddTokenLookupValues()
        {
            var fixture = new Fixture();
            var api = fixture.Build<ApiNameLookup>().Create();
            DatabaseContext.Add(api);
            DatabaseContext.SaveChanges();

            var apiEndpoint = fixture.Build<ApiEndpointNameLookup>()
                .With(x => x.ApiLookupId, api.Id).Create();
            DatabaseContext.Add(apiEndpoint);

            var consumerType = fixture.Build<ConsumerTypeLookup>().Create();
            DatabaseContext.Add(consumerType);
            DatabaseContext.SaveChanges();

            return fixture.Build<AuthTokens>()
                .With(x => x.ApiEndpointNameLookupId, apiEndpoint.Id)
                .With(x => x.ApiLookupId, api.Id)
                .With(x => x.ConsumerTypeLookupId, consumerType.Id)
                .With(x => x.ExpirationDate, DateTime.MaxValue.Date)
                .Create();
        }
    }
}
