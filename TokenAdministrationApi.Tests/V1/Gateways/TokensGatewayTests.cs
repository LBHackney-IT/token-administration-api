using AutoFixture;
using TokenAdministrationApi.Tests.V1.Helper;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Gateways;
using FluentAssertions;
using NUnit.Framework;
using TokenAdministrationApi.V1.Infrastructure;

namespace TokenAdministrationApi.Tests.V1.Gateways
{
    [TestFixture]
    public class TokensGatewayTests : DatabaseTests
    {
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
            var result = _classUnderTest.GetAllTokens(null);

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
            var result = _classUnderTest.GetAllTokens(false);

            result.Should().NotBeNull();
            result.Count.Should().Be(1);
        }

        [Test]
        public void ShouldGetOnlyEnabledTokensFromDatabase()
        {
            AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(true, DatabaseContext);
            AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(true, DatabaseContext);
            AuthTokenDatabaseEntityHelper.AddTokenRecordToTheDatabase(false, DatabaseContext);
            var result = _classUnderTest.GetAllTokens(true);

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
        }
    }
}
