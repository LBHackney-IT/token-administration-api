using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Infrastructure;

namespace TokenAdministrationApi.V1.Factories
{
    public static class EntityFactory
    {
        public static AuthToken ToDomain(this AuthTokens token, string apiEndpointName, string apiName,
          string consumerType)
        {
            return new AuthToken
            {
                Id = token.Id,
                ApiEndpointName = apiEndpointName,
                ApiName = apiName,
                HttpMethodType = token.HttpMethodType,
                ConsumerName = token.ConsumerName,
                ConsumerType = consumerType,
                Environment = token.Environment,
                ExpirationDate = token.ExpirationDate,
                Enabled = token.Enabled
            };
        }
    }
}
