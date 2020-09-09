using ApiAuthTokenGenerator.V1.Boundary.Response;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Controllers;
using TokenAdministrationApi.V1.UseCase.Interfaces;

namespace TokenAdministrationApi.Tests.V1.Controllers
{
    public class TokenAdministrationApiControllerTests
    {
        private TokenAdministrationApiController _classUnderTest;
        private Mock<IPostTokenUseCase> _mockPostTokenUseCase;
        private Mock<IGetAllUseCase> _getAllUseCase;

        [SetUp]
        public void Setup()
        {
            _mockPostTokenUseCase = new Mock<IPostTokenUseCase>();
            _getAllUseCase = new Mock<IGetAllUseCase>();
            _classUnderTest = new TokenAdministrationApiController( _getAllUseCase.Object, _mockPostTokenUseCase.Object);
        }

        [Test]
        public void EnsureControllerPostMethodCallsPostTokenUseCase()
        {
            var response = new GenerateTokenResponse();
            _mockPostTokenUseCase.Setup(x => x.Execute(It.IsAny<TokenRequestObject>())).Returns(response);
            _classUnderTest.GenerateToken(It.IsAny<TokenRequestObject>());

            _mockPostTokenUseCase.Verify(x => x.Execute(It.IsAny<TokenRequestObject>()), Times.Once);
        }

        [Test]
        public void ControllerPostMethodShouldReturnResponseOfTypeGenerateTokenResponse()
        {
            var response = new GenerateTokenResponse();
            _mockPostTokenUseCase.Setup(x => x.Execute(It.IsAny<TokenRequestObject>())).Returns(response);
            var result = _classUnderTest.GenerateToken(It.IsAny<TokenRequestObject>()) as CreatedAtActionResult;

            result.Should().NotBeNull();
            result.Value.Should().BeOfType<GenerateTokenResponse>();
        }

        [Test]
        public void ControllerPostMethodShouldReturn201StatusCode()
        {
            var response = new GenerateTokenResponse();
            _mockPostTokenUseCase.Setup(x => x.Execute(It.IsAny<TokenRequestObject>())).Returns(response);
            var result = _classUnderTest.GenerateToken(It.IsAny<TokenRequestObject>()) as CreatedAtActionResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(201);
        }
    }
}
