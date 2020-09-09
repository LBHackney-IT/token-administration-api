using TokenAdministrationApi.V1.Factories;
using TokenAdministrationApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using AutoFixture;

namespace TokenAdministrationApi.Tests.V1.Factories
{
    [TestFixture]
    public class EntityFactoryTest
    {
        [Test]
        public void CanMapADatabaseEntityToADomainObject()
        {
            var fixture = new Fixture();
            var tokenData = fixture.Build<AuthTokens>().Create();
            var apiData = fixture.Build<ApiNameLookup>().Create();
            var apiEndpointData = fixture.Build<ApiEndpointNameLookup>().Create();
            var consumerData = fixture.Build<ConsumerTypeLookup>().Create();

            var response = tokenData.ToDomain(apiEndpointData.ApiEndpointName, apiData.ApiName, consumerData.TypeName);

            response.ApiEndpointName.Should().Be(apiEndpointData.ApiEndpointName);
            response.ApiName.Should().Be(apiData.ApiName);
            response.HttpMethodType.Should().Be(tokenData.HttpMethodType);
            response.ConsumerType.Should().Be(consumerData.TypeName);
            response.Id.Should().Be(tokenData.Id);
            response.ExpirationDate.Should().Be(tokenData.ExpirationDate);
            response.Environment.Should().Be(tokenData.Environment);
            response.ConsumerName.Should().Be(tokenData.ConsumerName);
            response.Enabled.Should().Be(tokenData.Enabled);
        }
    }
}
