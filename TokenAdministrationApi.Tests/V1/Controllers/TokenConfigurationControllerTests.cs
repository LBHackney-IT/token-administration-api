using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.Controllers;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Domain.Exceptions;
using TokenAdministrationApi.V1.UseCase.Interfaces;

namespace TokenAdministrationApi.Tests.V1.Controllers
{
    public class TokenConfigurationControllerTests
    {
        private TokenConfigurationController _classUnderTest;
        private Mock<IGetTokenOptionsUseCase> _getTokenOptionsUseCase;
        private Mock<IPostApiUseCase> _postApiUseCase;
        private Mock<IPostEndpointUseCase> _postEndpointUseCase;

        [SetUp]
        public void Setup()
        {
            _getTokenOptionsUseCase = new Mock<IGetTokenOptionsUseCase>();
            _postApiUseCase = new Mock<IPostApiUseCase>();
            _postEndpointUseCase = new Mock<IPostEndpointUseCase>();

            _classUnderTest = new TokenConfigurationController(_getTokenOptionsUseCase.Object,
                _postApiUseCase.Object, _postEndpointUseCase.Object);
        }

        [Test]
        public async Task EnsureControllerGetTokenOptionsMethodCallsUseCase()
        {
            _getTokenOptionsUseCase.Setup(x => x.Execute()).ReturnsAsync(new TokenOptions());

            await _classUnderTest.GetTokenOptions();

            _getTokenOptionsUseCase.Verify(x => x.Execute(), Times.Once);
        }

        [Test]
        public async Task ControllerGetTokenOptionsMethodShouldReturnResponseOfTypeTokenOptionsResponse()
        {
            _getTokenOptionsUseCase.Setup(x => x.Execute()).ReturnsAsync(new TokenOptions());

            var result = await _classUnderTest.GetTokenOptions() as OkObjectResult;

            result.Should().NotBeNull();
            result.Value.Should().BeOfType<TokenOptionsResponse>();
        }

        [Test]
        public async Task ControllerGetTokenOptionsMethodShouldReturn200StatusCode()
        {
            _getTokenOptionsUseCase.Setup(x => x.Execute()).ReturnsAsync(new TokenOptions());

            var result = await _classUnderTest.GetTokenOptions() as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Test]
        public async Task ControllerGetTokenOptionsMethodCanReturnTokenOptions()
        {
            var tokenOptions = new TokenOptions
            {
                ConsumerTypes = new List<ConsumerTypeOption>
                {
                    new ConsumerTypeOption { Id = 1, TypeName = "token-options-consumer" }
                },
                ApiLookups = new List<ApiLookupOption>
                {
                    new ApiLookupOption { Id = 1, ApiName = "contracts-api", ApiGatewayId = "gw-test-1234567" }
                },
                ApiEndpoints = new List<ApiEndpointOption>
                {
                    new ApiEndpointOption { Id = 1, ApiLookupId = 1, EndpointName = "/api/v1/token-options-test" }
                }
            };
            _getTokenOptionsUseCase.Setup(x => x.Execute()).ReturnsAsync(tokenOptions);

            var result = await _classUnderTest.GetTokenOptions() as OkObjectResult;
            var response = result?.Value as TokenOptionsResponse;

            response.Should().NotBeNull();
            response.ConsumerTypes[0].TypeName.Should().Be("token-options-consumer");
            response.ApiLookups[0].ApiName.Should().Be("contracts-api");
            response.ApiEndpoints[0].EndpointName.Should().Be("/api/v1/token-options-test");
        }

        [Test]
        public void ControllerPostApiMethodShouldReturn201WithCreatedApi()
        {
            var request = new CreateApiLookupRequest { ApiName = "housing-api", ApiGatewayId = "gw-housing-dev" };
            var api = new ApiLookupOption { Id = 1, ApiName = request.ApiName, ApiGatewayId = request.ApiGatewayId };
            _postApiUseCase.Setup(x => x.Execute(It.IsAny<ApiLookupOption>())).Returns(api);

            var result = _classUnderTest.PostApi(request) as ObjectResult;
            var response = result?.Value as ApiLookupOptionResponse;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(201);
            response.Should().BeEquivalentTo(new ApiLookupOptionResponse
            {
                Id = api.Id,
                ApiName = api.ApiName,
                ApiGatewayId = api.ApiGatewayId
            });
        }

        [Test]
        public void ControllerPostApiMethodShouldReturn409IfApiAlreadyExists()
        {
            var request = new CreateApiLookupRequest { ApiName = "housing-api", ApiGatewayId = "gw-housing-dev" };
            _postApiUseCase.Setup(x => x.Execute(It.IsAny<ApiLookupOption>()))
                .Throws(new DuplicateApiException("API name or gateway ID already exists."));

            var result = _classUnderTest.PostApi(request) as ObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(409);
            result.Value.Should().Be("API name or gateway ID already exists.");
        }

        [Test]
        public void ControllerPostEndpointMethodShouldReturn201WithCreatedEndpoint()
        {
            var apiLookupId = 1;
            var request = new CreateEndpointRequest { EndpointName = "/tenancies" };
            var endpoint = new ApiEndpointOption
            {
                Id = 10,
                ApiLookupId = apiLookupId,
                ApiName = "housing-api",
                EndpointName = request.EndpointName
            };
            _postEndpointUseCase.Setup(x => x.Execute(It.IsAny<ApiEndpointOption>())).Returns(endpoint);

            var result = _classUnderTest.PostEndpoint(apiLookupId, request) as ObjectResult;
            var response = result?.Value as CreateEndpointResponse;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(201);
            response.Should().BeEquivalentTo(new CreateEndpointResponse
            {
                Id = endpoint.Id,
                ApiLookupId = endpoint.ApiLookupId,
                ApiName = endpoint.ApiName,
                EndpointName = endpoint.EndpointName
            });
        }

        [Test]
        public void ControllerPostEndpointMethodShouldReturn404IfApiLookupDoesNotExist()
        {
            var nonExistentApiLookupId = 999;
            var request = new CreateEndpointRequest { EndpointName = "/tenancies" };
            _postEndpointUseCase.Setup(x => x.Execute(It.IsAny<ApiEndpointOption>()))
                .Throws(new LookupValueDoesNotExistException("API lookup was not found."));

            var result = _classUnderTest.PostEndpoint(nonExistentApiLookupId, request) as NotFoundObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
            result.Value.Should().Be("API lookup was not found.");
        }

        [Test]
        public void ControllerPostEndpointMethodShouldReturn409IfEndpointAlreadyExists()
        {
            var request = new CreateEndpointRequest { EndpointName = "/tenancies" };
            _postEndpointUseCase.Setup(x => x.Execute(It.IsAny<ApiEndpointOption>()))
                .Throws(new DuplicateEndpointException("Endpoint already exists for this API."));

            var result = _classUnderTest.PostEndpoint(1, request) as ObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(409);
            result.Value.Should().Be("Endpoint already exists for this API.");
        }
    }
}
