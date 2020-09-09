using System;
using ApiAuthTokenGenerator.V1.Boundary.Response;
using Bogus;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Gateways;
using TokenAdministrationApi.V1.UseCase;
using TokenAdministrationApi.V1.UseCase.Interfaces;

namespace TokenAdministrationApi.Tests.V1.UseCase
{
    public class PostTokenUseCaseTests
    {
        private PostTokenUseCase _classUnderTest;
        private Mock<IGenerateJwtUseCase> _mockGenerateJwtHelper;
        private Mock<ITokensGateway> _mockGateway;
        private Faker _faker;
        [SetUp]
        public void Setup()
        {
            _mockGenerateJwtHelper = new Mock<IGenerateJwtUseCase>();
            _mockGateway = new Mock<ITokensGateway>();
            _classUnderTest = new PostTokenUseCase(_mockGenerateJwtHelper.Object, _mockGateway.Object);
            _faker = new Faker();
        }
        [Test]
        public void UseCaseShouldCallHelperMethodToGenerateJwtToken()
        {
            var tokenRequest = GetTokenRequestObject();
            var jwtTokenResult = _faker.Random.AlphaNumeric(20);
            _mockGateway.Setup(x => x.GenerateToken(tokenRequest)).Returns(_faker.Random.Int(1, 100));
            _mockGenerateJwtHelper.Setup(x => x.GenerateJwtToken(It.IsAny<GenerateJwtRequest>())).Returns(jwtTokenResult);

            _classUnderTest.Execute(tokenRequest);

            _mockGenerateJwtHelper.Verify(x => x.GenerateJwtToken(It.IsAny<GenerateJwtRequest>()), Times.Once);
        }
        [Test]
        public void UseCaseShouldCallGatewayToInsertTokenData()
        {
            var tokenRequest = GetTokenRequestObject();
            _mockGateway.Setup(x => x.GenerateToken(tokenRequest)).Returns(_faker.Random.Int(1, 100));
            var jwtTokenResult = _faker.Random.AlphaNumeric(20);
            _mockGenerateJwtHelper.Setup(x => x.GenerateJwtToken(It.IsAny<GenerateJwtRequest>())).Returns(jwtTokenResult);
            _classUnderTest.Execute(tokenRequest);

            _mockGateway.Verify(x => x.GenerateToken(It.IsAny<TokenRequestObject>()), Times.Once);
        }

        [Test]
        public void VerifyThatHelperIsNotCalledIfGatewayFailsToInsertRecordAndExceptionIsThrown()
        {
            var tokenRequest = GetTokenRequestObject();
            _mockGateway.Setup(x => x.GenerateToken(tokenRequest)).Returns(0);

            Func<GenerateTokenResponse> testDelegate = () => _classUnderTest.Execute(tokenRequest);
            testDelegate.Should().Throw<TokenNotInsertedException>();
            _mockGenerateJwtHelper.Verify(x => x.GenerateJwtToken(It.IsAny<GenerateJwtRequest>()), Times.Never);
        }

        [Test]
        public void VerifyThatExceptionIsThrownIfTokenIsNotGenerated()
        {
            var tokenRequest = GetTokenRequestObject();
            _mockGateway.Setup(x => x.GenerateToken(tokenRequest)).Returns(_faker.Random.Int(1, 100));
            _mockGenerateJwtHelper.Setup(x => x.GenerateJwtToken(It.IsAny<GenerateJwtRequest>())).Returns(() => null);

            Func<GenerateTokenResponse> testDelegate = () => _classUnderTest.Execute(tokenRequest);
            testDelegate.Should().Throw<JwtTokenNotGeneratedException>();
        }

        [Test]
        public void GenerateTokenResponseIsReturnedWhenAllOperationsAreSuccessful()
        {
            var tokenRequest = GetTokenRequestObject();
            var tokenId = _faker.Random.Int(1, 100);
            _mockGateway.Setup(x => x.GenerateToken(tokenRequest)).Returns(tokenId);
            var jwtTokenResult = _faker.Random.AlphaNumeric(20);
            _mockGenerateJwtHelper.Setup(x => x.GenerateJwtToken(It.IsAny<GenerateJwtRequest>())).Returns(jwtTokenResult);

            var response = _classUnderTest.Execute(tokenRequest);

            response.Should().NotBeNull();
            response.Token.Should().Be(jwtTokenResult);
            response.Id.Should().Be(tokenId);
            response.GeneratedAt.Date.Should().BeSameDateAs(DateTime.Now.Date);
            response.ExpiresAt.Should().Be(tokenRequest.ExpiresAt);
        }

        private TokenRequestObject GetTokenRequestObject()
        {
            return new TokenRequestObject
            {
                Consumer = _faker.Random.String(),
                ConsumerType = _faker.Random.Int(5),
                ExpiresAt = _faker.Date.Future()
            };
        }
    }
}
