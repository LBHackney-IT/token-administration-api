using Bogus;
using FluentAssertions;
using NUnit.Framework;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Factories;

namespace TokenAdministrationApi.Tests.V1.Factories
{
    public class GenerateJwtFactoryTests
    {
        private Faker _faker;

        [SetUp]
        public void Setup()
        {
            _faker = new Faker();
        }
        [Test]
        public void CanMapInputToJwtRequestObject()
        {
            var tokenRequest = new TokenRequestObject
            {
                Consumer = _faker.Random.String(),
                ConsumerType = _faker.Random.Int(5),
                ExpiresAt = _faker.Date.Future()
            };

            var id = _faker.Random.Number(0, 20);

            var factoryResponse = GenerateJwtFactory.ToJwtRequest(tokenRequest, id);

            factoryResponse.Id.Should().Be(id);
            factoryResponse.ConsumerName.Should().Be(tokenRequest.Consumer);
            factoryResponse.ConsumerType.Should().Be(tokenRequest.ConsumerType);
            factoryResponse.ExpiresAt.Should().Be(tokenRequest.ExpiresAt);
        }
    }
}
