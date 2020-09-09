using System;
using System.Globalization;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using TokenAdministrationApi.V1.Gateways;
using NUnit.Framework;
using TokenAdministrationApi.V1.Boundary.Requests;

namespace TokenAdministrationApi.Tests.V1.Gateways
{
    //For instruction on how to run tests please see the wiki: https://github.com/LBHackney-IT/lbh-TokenAdministrationApi/wiki/Running-the-test-suite.
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
        public void InsertingATokenRecordShouldReturnAnId()
        {
            var tokenRequest = _fixture.Build<TokenRequestObject>().Create();

            var response = _classUnderTest.GenerateToken(tokenRequest);

            response.Should().NotBe(0);
        }
        [Test]
        public void InsertedRecordShouldBeInsertedOnceInTheDatabase()
        {
            var tokenRequest = _fixture.Build<TokenRequestObject>().Create();

            var response = _classUnderTest.GenerateToken(tokenRequest);

            var databaseRecord = DatabaseContext.Tokens.Where(x => x.Id == response);
            var defaultRecordRetrieved = databaseRecord.FirstOrDefault();

            databaseRecord.Count().Should().Be(1);
        }
        [Test]
        public void InsertedRecordShouldBeInTheDatabase()
        {
            var tokenRequest = _fixture.Build<TokenRequestObject>().Create();

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
    }
}
