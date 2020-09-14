using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenAdministrationApi.V1.Boundary.Request;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.Controllers;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.UseCase.Interfaces;

namespace TokenAdministrationApi.Tests.V1.Controllers
{
    public class TokenAdministrationControllerTests
    {
        private TokenAdministrationApiController _classUnderTest;
        private Mock<IPostTokenUseCase> _mockPostTokenUseCase;
        private Mock<IGetAllTokensUseCase> _mockGetAllTokensUseCase;

        [SetUp]
        public void Setup()
        {
            _mockGetAllTokensUseCase = new Mock<IGetAllTokensUseCase>();
            _mockPostTokenUseCase = new Mock<IPostTokenUseCase>();
            _classUnderTest = new TokenAdministrationApiController(_mockGetAllTokensUseCase.Object, _mockPostTokenUseCase.Object);
        }
        [Test]
        public void EnsureControllerListTokensMethodCallsGetAllTokensUseCase()
        {
            var response = new TokensListResponse();
            _mockGetAllTokensUseCase.Setup(x => x.Execute(It.IsAny<GetTokensRequest>())).Returns(response);
            _classUnderTest.ListTokens(It.IsAny<GetTokensRequest>());

            _mockGetAllTokensUseCase.Verify(x => x.Execute(It.IsAny<GetTokensRequest>()), Times.Once);
        }

        [Test]
        public void ControllerListTokensMethodShouldReturnResponseOfTypeTokensListResponse()
        {
            var response = new TokensListResponse();
            _mockGetAllTokensUseCase.Setup(x => x.Execute(It.IsAny<GetTokensRequest>())).Returns(response);
            var result = _classUnderTest.ListTokens(It.IsAny<GetTokensRequest>()) as OkObjectResult;

            result.Should().NotBeNull();
            result.Value.Should().BeOfType<TokensListResponse>();
        }

        [Test]
        public void ControllerListTokensMethodShouldReturn200StatusCode()
        {
            var response = new TokensListResponse();
            _mockGetAllTokensUseCase.Setup(x => x.Execute(It.IsAny<GetTokensRequest>())).Returns(response);
            var result = _classUnderTest.ListTokens(It.IsAny<GetTokensRequest>()) as OkObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Test]
        public void ControllerListTokensMethodCanListTokens()
        {
            var fixture = new Fixture();
            var expectedResponse = new TokensListResponse() { Tokens = new List<AuthToken>() { fixture.Build<AuthToken>().Create() } };
            _mockGetAllTokensUseCase.Setup(x => x.Execute(It.IsAny<GetTokensRequest>())).Returns(expectedResponse);
            var result = _classUnderTest.ListTokens(It.IsAny<GetTokensRequest>()) as OkObjectResult;

            result.Should().NotBeNull();
            result.Value.Should().Be(expectedResponse);
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