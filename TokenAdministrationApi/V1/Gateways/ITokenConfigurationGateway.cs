using System.Threading.Tasks;
using TokenAdministrationApi.V1.Domain;

namespace TokenAdministrationApi.V1.Gateways
{
    public interface ITokenConfigurationGateway
    {
        Task<TokenOptions> GetTokenOptions();
        ApiLookupOption CreateApiLookup(ApiLookupOption api);
        ApiEndpointOption CreateEndpoint(ApiEndpointOption endpoint);
    }
}
