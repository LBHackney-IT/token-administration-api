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
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.Controllers;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.UseCase.Interfaces;

namespace TokenAdministrationApi.Tests.V1.Controllers
{
    public class TokenAdministrationControllerTests
    {
        private TokenAdministrationApiController _classUnderTest;
        private Mock<IGetAllTokensUseCase> _mockGetAllTokensUseCase;

        [SetUp]
        public void Setup()
        {
            _mockGetAllTokensUseCase = new Mock<IGetAllTokensUseCase>();
            _classUnderTest = new TokenAdministrationApiController(_mockGetAllTokensUseCase.Object);
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
    }
}
