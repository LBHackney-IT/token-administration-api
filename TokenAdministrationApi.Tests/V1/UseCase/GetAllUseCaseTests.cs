using System.Linq;
using AutoFixture;
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Factories;
using TokenAdministrationApi.V1.Gateways;
using TokenAdministrationApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TokenAdministrationApi.V1.Boundary.Request;
using TokenAdministrationApi.V1.Infrastructure;

namespace TokenAdministrationApi.Tests.V1.UseCase
{
    public class GetAllUseCaseTests
    {
        private Mock<ITokensGateway> _mockGateway;
        private GetAllTokensUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void SetUp()
        {
            _mockGateway = new Mock<ITokensGateway>();
            _classUnderTest = new GetAllTokensUseCase(_mockGateway.Object);
            _fixture = new Fixture();
        }
        [Test]
        public void EnsureGetAllTokensUseCaseCallsGateway()
        {
            var stubbedEntities = _fixture.CreateMany<AuthToken>().ToList();
            _mockGateway.Setup(x => x.GetAllTokens(null)).Returns(stubbedEntities);
            _classUnderTest.Execute(new GetTokensRequest { Enabled = null });

            _mockGateway.Verify(x => x.GetAllTokens(null), Times.Once);
        }
        [Test]
        public void GetsAllTokensFromTheGateway()
        {
            var stubbedEntities = _fixture.CreateMany<AuthToken>().ToList();
            _mockGateway.Setup(x => x.GetAllTokens(null)).Returns(stubbedEntities);

            var expectedResponse = new TokensListResponse { Tokens = stubbedEntities };

            _classUnderTest.Execute(new GetTokensRequest { Enabled = null }).Should().BeEquivalentTo(expectedResponse);
        }
    }
}
