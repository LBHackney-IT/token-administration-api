using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenAdministrationApi.V1.Boundary.Response
{
    public class TokenOptionsResponse
    {
        public List<ConsumerTypeOptionResponse> ConsumerTypes { get; set; } = new();
        public List<ApiLookupOptionResponse> ApiLookups { get; set; } = new();
        public List<ApiEndpointOptionResponse> ApiEndpoints { get; set; } = new();
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

