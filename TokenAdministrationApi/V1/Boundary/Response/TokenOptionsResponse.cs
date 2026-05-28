using System.Collections.Generic;

namespace TokenAdministrationApi.V1.Boundary.Response
{
    public class TokenOptionsResponse
    {
        public List<ConsumerTypeOptionResponse> ConsumerTypes { get; set; } = [];
        public List<ApiLookupOptionResponse> ApiLookups { get; set; } = [];
        public List<ApiEndpointOptionResponse> ApiEndpoints { get; set; } = [];
    }

    public class ConsumerTypeOptionResponse
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
    }

    public class ApiLookupOptionResponse
    {
        public int Id { get; set; }
        public string ApiName { get; set; }
        public string ApiGatewayId { get; set; }
    }

    public class ApiEndpointOptionResponse
    {
        public int Id { get; set; }
        public int ApiLookupId { get; set; }
        public string EndpointName { get; set; }
    }
}
