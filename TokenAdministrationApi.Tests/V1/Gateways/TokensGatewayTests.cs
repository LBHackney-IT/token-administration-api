using AutoFixture;
using TokenAdministrationApi.Tests.V1.Helper;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Gateways;
using FluentAssertions;
using NUnit.Framework;

namespace TokenAdministrationApi.Tests.V1.Gateways
{
    //TODO: Rename Tests to match gateway name
    //For instruction on how to run tests please see the wiki: https://github.com/LBHackney-IT/lbh-TokenAdministrationApi/wiki/Running-the-test-suite.
    [TestFixture]
    public class TokensGatewayTests : DatabaseTests
    {
        //private readonly Fixture _fixture = new Fixture();
        private TokensGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new TokensGateway(DatabaseContext);
        }
        //TODO: Add tests here for the get all method.
    }
}
