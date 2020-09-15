using System;
using Bogus;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Domain.Exceptions;
using TokenAdministrationApi.V1.Gateways;
using TokenAdministrationApi.V1.UseCase;

namespace TokenAdministrationApi.Tests.V1.UseCase
{
    public class UpdateTokenValidityUseCaseTests
    {
        private UpdateTokenValidityUseCase _classUnderTest;
        private Mock<ITokensGateway> _tokenGateway;

        [SetUp]
        public void SetUp()
        {
            _tokenGateway = new Mock<ITokensGateway>();
            _classUnderTest = new UpdateTokenValidityUseCase(_tokenGateway.Object);
        }

        [Test]
        public void ItPassesIdAndEnabledToTheGateway()
        {
            var faker = new Faker();
            var tokenId = faker.Random.Int();
            var enabled = faker.Random.Bool();
            _tokenGateway.Setup(x => x.UpdateToken(tokenId, enabled)).Returns(tokenId).Verifiable();
            _classUnderTest.Execute(tokenId, new UpdateTokenRequest { Enabled = enabled });
            _tokenGateway.Verify();
        }

        [Test]
        public void IfTheGatewayReturnsNullThrowsTokenNotFoundException()
        {
            _tokenGateway.Setup(x => x.UpdateToken(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns((int?) null);

            var action = new Action(() => _classUnderTest.Execute(3, new UpdateTokenRequest { Enabled = true }));
            action.Should().Throw<TokenRecordNotFoundException>();
        }
    }
}
