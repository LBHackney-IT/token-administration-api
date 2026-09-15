using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TokenAdministrationApi.Tests.V1.Helper;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Domain.Exceptions;
using TokenAdministrationApi.V1.Gateways;
using TokenAdministrationApi.V1.Infrastructure;

namespace TokenAdministrationApi.Tests.V1.Gateways
{
    [TestFixture]
    public class TokenConfigurationGatewayTests : DatabaseTests
    {
        private TokenConfigurationGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new TokenConfigurationGateway(DatabaseContext);
        }

        [Test]
        public async Task ShouldGetTokenOptionsFromDatabase()
        {
            var consumerType = new ConsumerTypeLookup
            {
                Id = 1,
                TypeName = "token-options-consumer"
            };
            var api = new ApiNameLookup
            {
                Id = 1,
                ApiName = "contracts-api",
                ApiGatewayId = "gw-test-1234567"
            };
            var apiEndpoint = new ApiEndpointNameLookup
            {
                Id = 1,
                ApiLookupId = api.Id,
                ApiEndpointName = "/api/v1/token-options-test"
            };

            DatabaseContext.ConsumerTypeLookups.Add(consumerType);
            DatabaseContext.ApiNameLookups.Add(api);
            DatabaseContext.ApiEndpointNameLookups.Add(apiEndpoint);
            DatabaseContext.SaveChanges();

            var result = await _classUnderTest.GetTokenOptions();

            result.ConsumerTypes.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new ConsumerTypeOption
                {
                    Id = consumerType.Id,
                    TypeName = consumerType.TypeName
                });
            result.ApiLookups.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new ApiLookupOption
                {
                    Id = api.Id,
                    ApiName = api.ApiName,
                    ApiGatewayId = api.ApiGatewayId
                });
            result.ApiEndpoints.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(new ApiEndpointOption
                {
                    Id = apiEndpoint.Id,
                    ApiLookupId = apiEndpoint.ApiLookupId,
                    EndpointName = apiEndpoint.ApiEndpointName
                });
        }

        [Test]
        public void CreateApiLookupShouldInsertApiLookupInDatabase()
        {
            var api = new ApiLookupOption
            {
                ApiName = "housing-api",
                ApiGatewayId = "gw-housing-dev"
            };

            var result = _classUnderTest.CreateApiLookup(api);

            var databaseRecord = DatabaseContext.ApiNameLookups.Find(result.Id);
            databaseRecord.Should().NotBeNull();
            databaseRecord.ApiName.Should().Be(api.ApiName);
            databaseRecord.ApiGatewayId.Should().Be(api.ApiGatewayId);
        }

        [Test]
        public void CreateApiLookupShouldThrowDuplicateApiExceptionIfNameAlreadyExists()
        {
            var existingApi = new ApiNameLookup
            {
                ApiName = "housing-api",
                ApiGatewayId = "gw-housing-dev"
            };
            DatabaseContext.ApiNameLookups.Add(existingApi);
            DatabaseContext.SaveChanges();

            var api = new ApiLookupOption
            {
                ApiName = "housing-api",
                ApiGatewayId = "gw-housing-test"
            };

            Action createDuplicateApi = () => _classUnderTest.CreateApiLookup(api);
            createDuplicateApi.Should().Throw<DuplicateApiException>()
                .WithMessage("API name or gateway ID already exists.");
        }

        [Test]
        public void CreateApiLookupShouldThrowDuplicateApiExceptionIfGatewayIdAlreadyExists()
        {
            var existingApi = new ApiNameLookup
            {
                ApiName = "housing-api",
                ApiGatewayId = "gw-housing-dev"
            };
            DatabaseContext.ApiNameLookups.Add(existingApi);
            DatabaseContext.SaveChanges();

            var api = new ApiLookupOption
            {
                ApiName = "repairs-api",
                ApiGatewayId = "gw-housing-dev"
            };

            Action createApiWithDuplicateGatewayId = () => _classUnderTest.CreateApiLookup(api);
            createApiWithDuplicateGatewayId.Should().Throw<DuplicateApiException>()
                .WithMessage("API name or gateway ID already exists.");
        }

        [Test]
        public void CreateEndpointShouldInsertEndpointForParentApi()
        {
            var api = AddApiLookup();
            var endpoint = new ApiEndpointOption
            {
                ApiLookupId = api.Id,
                EndpointName = "/tenancies"
            };

            var result = _classUnderTest.CreateEndpoint(endpoint);

            var databaseRecord = DatabaseContext.ApiEndpointNameLookups.Find(result.Id);
            databaseRecord.Should().NotBeNull();
            databaseRecord.ApiLookupId.Should().Be(api.Id);
            databaseRecord.ApiEndpointName.Should().Be(endpoint.EndpointName);
            result.ApiName.Should().Be(api.ApiName);
        }

        [Test]
        public void CreateEndpointShouldThrowLookupValueDoesNotExistExceptionIfParentApiDoesNotExist()
        {
            var endpoint = new ApiEndpointOption
            {
                ApiLookupId = 999,
                EndpointName = "/tenancies"
            };

            Action createEndpointForMissingApi = () => _classUnderTest.CreateEndpoint(endpoint);
            createEndpointForMissingApi.Should().Throw<LookupValueDoesNotExistException>()
                .WithMessage("API lookup was not found.");
        }

        [Test]
        public void CreateEndpointShouldThrowDuplicateEndpointExceptionIfEndpointAlreadyExistsForApi()
        {
            var api = AddApiLookup();
            DatabaseContext.ApiEndpointNameLookups.Add(new ApiEndpointNameLookup
            {
                ApiLookupId = api.Id,
                ApiEndpointName = "/tenancies"
            });
            DatabaseContext.SaveChanges();

            var endpoint = new ApiEndpointOption
            {
                ApiLookupId = api.Id,
                EndpointName = "/tenancies"
            };

            Action createDuplicateEndpoint = () => _classUnderTest.CreateEndpoint(endpoint);
            createDuplicateEndpoint.Should().Throw<DuplicateEndpointException>()
                .WithMessage("Endpoint already exists for this API.");
        }

        private ApiNameLookup AddApiLookup()
        {
            var api = new ApiNameLookup
            {
                ApiName = "housing-api",
                ApiGatewayId = "gw-housing-dev"
            };
            DatabaseContext.ApiNameLookups.Add(api);
            DatabaseContext.SaveChanges();

            return api;
        }
    }
}
