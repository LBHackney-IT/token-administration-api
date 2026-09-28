using System.Collections.Generic;

namespace TokenAdministrationApi.V1.Infrastructure
{
    public class TokenOptionsEntity
    {
        public List<ConsumerTypeLookup> ConsumerTypes { get; set; } = [];
        public List<ApiNameLookup> ApiLookups { get; set; } = [];
        public List<ApiEndpointNameLookup> ApiEndpoints { get; set; } = [];
    }
}
