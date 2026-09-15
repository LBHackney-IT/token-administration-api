using System.Linq;
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

        public static TokenOptions ToDomain(this TokenOptionsEntity tokenOptions)
        {
            return new TokenOptions
            {
                ConsumerTypes = tokenOptions.ConsumerTypes.Select(consumerType => consumerType.ToDomain()).ToList(),
                ApiLookups = tokenOptions.ApiLookups.Select(api => api.ToDomain()).ToList(),
                ApiEndpoints = tokenOptions.ApiEndpoints.Select(endpoint => endpoint.ToDomain()).ToList()
            };
        }

        public static ConsumerTypeOption ToDomain(this ConsumerTypeLookup consumerType)
        {
            return new ConsumerTypeOption
            {
                Id = consumerType.Id,
                TypeName = consumerType.TypeName
            };
        }

        public static ApiLookupOption ToDomain(this ApiNameLookup api)
        {
            return new ApiLookupOption
            {
                Id = api.Id,
                ApiName = api.ApiName,
                ApiGatewayId = api.ApiGatewayId
            };
        }

        public static ApiEndpointOption ToDomain(this ApiEndpointNameLookup endpoint, string apiName = null)
        {
            return new ApiEndpointOption
            {
                Id = endpoint.Id,
                ApiLookupId = endpoint.ApiLookupId,
                ApiName = apiName,
                EndpointName = endpoint.ApiEndpointName
            };
        }
    }
}
