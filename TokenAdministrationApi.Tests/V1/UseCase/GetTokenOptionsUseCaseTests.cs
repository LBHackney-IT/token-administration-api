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
        public void EnsureGetTokenOptionsUseCaseCallsGateway()
        {
            var response = new TokenOptionsResponse();
            _mockGateway.Setup(x => x.GetTokenOptions()).Returns(response);

            _classUnderTest.Execute();

            _mockGateway.Verify(x => x.GetTokenOptions(), Times.Once);
        }

        [Test]
        public void GetsTokenOptionsFromTheGateway()
        {
            var expectedResponse = new TokenOptionsResponse
            {
                ConsumerTypes = { new ConsumerTypeOptionResponse { Id = 1, TypeName = "token-options-consumer" } },
                ApiLookups = { new ApiLookupOptionResponse { Id = 1, ApiName = "contracts-api", ApiGatewayId = "gw-test-1234567" } },
                ApiEndpoints = { new ApiEndpointOptionResponse { Id = 1, ApiLookupId = 1, EndpointName = "/api/v1/token-options-test" } }
            };
            _mockGateway.Setup(x => x.GetTokenOptions()).Returns(expectedResponse);

            _classUnderTest.Execute().Should().BeEquivalentTo(expectedResponse);
        }
    }
}
