using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Domain;

namespace TokenAdministrationApi.V1.Factories
{
    public static class RequestFactory
    {
        public static ApiLookupOption ToDomain(this CreateApiLookupRequest request)
        {
            return new ApiLookupOption
            {
                ApiName = request.ApiName,
                ApiGatewayId = request.ApiGatewayId
            };
        }

        public static ApiEndpointOption ToDomain(this CreateEndpointRequest request, int apiLookupId)
        {
            return new ApiEndpointOption
            {
                ApiLookupId = apiLookupId,
                EndpointName = request.EndpointName
            };
        }
    }
}
