using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Infrastructure;

namespace TokenAdministrationApi.V1.Factories
{
    public static class EntityFactory
    {
        public static AuthToken ToDomain(this AuthTokens token)
        {
            return new AuthToken
            {
                Id = token.Id,
                ApiEndpointName = token.ApiEndpointNameLookup?.ApiEndpointName,
                ApiName = token.ApiLookup?.ApiName,
                HttpMethodType = token.HttpMethodType,
                ConsumerName = token.ConsumerName,
                ConsumerType = token.ConsumerTypeLookup?.TypeName,
                Environment = token.Environment,
                ExpirationDate = token.ExpirationDate,
                Enabled = token.Enabled
            };
        }
    }
}
