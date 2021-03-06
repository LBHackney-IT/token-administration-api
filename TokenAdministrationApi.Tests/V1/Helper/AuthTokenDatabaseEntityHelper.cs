using AutoFixture;
using Bogus;
using System;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Infrastructure;

namespace TokenAdministrationApi.Tests.V1.Helper
{
    public static class AuthTokenDatabaseEntityHelper
    {
        private static readonly Faker _faker = new Faker();
        public static AuthTokens CreateDatabaseEntity(TokenDatabaseContext context)
        {
            //insert lookup values (FK constraints)
            var fixture = new Fixture();
            var api = fixture.Build<ApiNameLookup>().Create();
            context.Add(api);
            context.SaveChanges();

            var apiEndpoint = fixture.Build<ApiEndpointNameLookup>()
                .With(x => x.ApiLookupId, api.Id).Create();
            context.Add(apiEndpoint);

            var consumerType = fixture.Build<ConsumerTypeLookup>().Create();
            context.Add(consumerType);
            context.SaveChanges();

            var tokenData = fixture.Build<AuthTokens>()
                .With(x => x.ApiEndpointNameLookupId, apiEndpoint.Id)
                .With(x => x.ApiLookupId, api.Id)
                .With(x => x.ConsumerTypeLookupId, consumerType.Id)
                .With(x => x.ExpirationDate, DateTime.MaxValue.Date)
                .Create();

            return CreateDatabaseEntityFrom(tokenData);
        }

        public static AuthTokens CreateDatabaseEntityFrom(AuthTokens entity)
        {
            return new AuthTokens
            {
                Id = entity.Id,
                ApiEndpointNameLookupId = entity.ApiEndpointNameLookupId,
                ApiLookupId = entity.ApiLookupId,
                AuthorizedBy = entity.AuthorizedBy,
                ConsumerName = entity.ConsumerName,
                ConsumerTypeLookupId = entity.ConsumerTypeLookupId,
                DateCreated = entity.DateCreated,
                Environment = entity.Environment,
                HttpMethodType = entity.HttpMethodType,
                ExpirationDate = entity.ExpirationDate,
                RequestedBy = entity.RequestedBy,
                Enabled = entity.Enabled
            };
        }
        public static AuthToken AddTokenRecordToTheDatabase(bool? enabled, TokenDatabaseContext context, int id = 0)
        {
            var fixture = new Fixture();
            var api = fixture.Build<ApiNameLookup>()
                .With(x => x.Id, _faker.Random.Int())
                .Create();
            context.Add(api);
            context.SaveChanges();

            var apiEndpoint = fixture.Build<ApiEndpointNameLookup>()
                 .With(x => x.Id, _faker.Random.Int())
                 .With(x => x.ApiLookupId, api.Id)
                .Create();
            context.Add(apiEndpoint);

            var consumerType = fixture.Build<ConsumerTypeLookup>()
                .With(x => x.Id, _faker.Random.Int())
                .Create();
            context.Add(consumerType);
            context.SaveChanges();

            var tokenData = fixture.Build<AuthTokens>()
                .Without(t => t.Id)
                .Without(t => t.ApiLookup)
                .Without(t => t.ApiEndpointNameLookup)
                .Without(t => t.ConsumerTypeLookup)
                .With(x => x.ApiLookupId, api.Id)
                .With(x => x.ApiEndpointNameLookupId, apiEndpoint.Id)
                .With(x => x.ConsumerTypeLookupId, consumerType.Id)
                .With(x => x.ExpirationDate, DateTime.MaxValue.Date)
                .With(x => x.Enabled, enabled ?? false)
                .Create();

            if (id != 0)
            {
                tokenData.Id = id;
            }
            context.Add(tokenData);
            context.SaveChanges();

            return new AuthToken
            {
                ApiEndpointName = apiEndpoint.ApiEndpointName,
                HttpMethodType = tokenData.HttpMethodType,
                ApiName = api.ApiName,
                ConsumerType = consumerType.TypeName,
                ConsumerName = tokenData.ConsumerName,
                Environment = tokenData.Environment,
                ExpirationDate = tokenData.ExpirationDate,
                Enabled = tokenData.Enabled,
                Id = tokenData.Id
            };
        }
    }
}
