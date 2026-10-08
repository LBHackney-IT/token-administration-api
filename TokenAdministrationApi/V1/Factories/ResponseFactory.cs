using System.Linq;
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.Domain;

namespace TokenAdministrationApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static TokenOptionsResponse ToResponse(this TokenOptions tokenOptions)
        {
            return new TokenOptionsResponse
            {
                ConsumerTypes = tokenOptions.ConsumerTypes.Select(consumerType => consumerType.ToResponse()).ToList(),
                ApiLookups = tokenOptions.ApiLookups.Select(api => api.ToResponse()).ToList(),
                ApiEndpoints = tokenOptions.ApiEndpoints.Select(endpoint => endpoint.ToOptionResponse()).ToList()
            };
        }

        public static ApiLookupOptionResponse ToResponse(this ApiLookupOption api)
        {
            return new ApiLookupOptionResponse
            {
                Id = api.Id,
                ApiName = api.ApiName,
                ApiGatewayId = api.ApiGatewayId
            };
        }

        public static CreateEndpointResponse ToResponse(this ApiEndpointOption endpoint)
        {
            return new CreateEndpointResponse
            {
                Id = endpoint.Id,
                ApiLookupId = endpoint.ApiLookupId,
                ApiName = endpoint.ApiName,
                EndpointName = endpoint.EndpointName
            };
        }

        private static ConsumerTypeOptionResponse ToResponse(this ConsumerTypeOption consumerType)
        {
            return new ConsumerTypeOptionResponse
            {
                Id = consumerType.Id,
                TypeName = consumerType.TypeName
            };
        }

        private static ApiEndpointOptionResponse ToOptionResponse(this ApiEndpointOption endpoint)
        {
            return new ApiEndpointOptionResponse
            {
                Id = endpoint.Id,
                ApiLookupId = endpoint.ApiLookupId,
                EndpointName = endpoint.EndpointName
            };
        }
    }
}
