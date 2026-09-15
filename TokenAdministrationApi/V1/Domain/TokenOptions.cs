using System.Collections.Generic;

namespace TokenAdministrationApi.V1.Domain
{
    public class TokenOptions
    {
        public List<ConsumerTypeOption> ConsumerTypes { get; set; } = [];
        public List<ApiLookupOption> ApiLookups { get; set; } = [];
        public List<ApiEndpointOption> ApiEndpoints { get; set; } = [];
    }

    public class ConsumerTypeOption
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
    }

    public class ApiLookupOption
    {
        public int Id { get; set; }
        public string ApiName { get; set; }
        public string ApiGatewayId { get; set; }
    }

    public class ApiEndpointOption
    {
        public int Id { get; set; }
        public int ApiLookupId { get; set; }
        public string ApiName { get; set; }
        public string EndpointName { get; set; }
    }
}
