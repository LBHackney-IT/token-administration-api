using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.Gateways;
using TokenAdministrationApi.V1.UseCase;

namespace TokenAdministrationApi.Tests.V1.UseCase
{
    public class GetTokenOptionsUseCaseTests
    {
        private Mock<ITokensGateway> _mockGateway;
        private GetTokenOptionsUseCase _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<ITokensGateway>();
            _classUnderTest = new GetTokenOptionsUseCase(_mockGateway.Object);
        }

        [Test]
        public async Task EnsureGetTokenOptionsUseCaseCallsGateway()
        {
            var response = new TokenOptionsResponse();
            _mockGateway.Setup(x => x.GetTokenOptions()).ReturnsAsync(response);

            await _classUnderTest.Execute();

            _mockGateway.Verify(x => x.GetTokenOptions(), Times.Once);
        }

        [Test]
        public async Task GetsTokenOptionsFromTheGateway()
        {
            var expectedResponse = new TokenOptionsResponse
            {
                ConsumerTypes = { new ConsumerTypeOptionResponse { Id = 1, TypeName = "token-options-consumer" } },
                ApiLookups = { new ApiLookupOptionResponse { Id = 1, ApiName = "contracts-api", ApiGatewayId = "gw-test-1234567" } },
                ApiEndpoints = { new ApiEndpointOptionResponse { Id = 1, ApiLookupId = 1, EndpointName = "/api/v1/token-options-test" } }
            };
            _mockGateway.Setup(x => x.GetTokenOptions()).ReturnsAsync(expectedResponse);

            var result = await _classUnderTest.Execute();

            result.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
