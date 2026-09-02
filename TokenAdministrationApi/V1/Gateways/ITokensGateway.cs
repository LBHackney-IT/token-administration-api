using System.Collections.Generic;
using System.Threading.Tasks;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Boundary.Response;

namespace TokenAdministrationApi.V1.Gateways
{
    public interface ITokensGateway
    {
        List<AuthToken> GetAllTokens(int limit, int cursor, bool? enabled);
        int GenerateToken(TokenRequestObject tokenRequestObject);
        int? UpdateToken(int tokenId, bool enabled);
        Task<TokenOptionsResponse> GetTokenOptionsAsync();

        ApiLookupOptionResponse CreateApiLookup(CreateApiLookupRequest request);

        CreateEndpointResponse CreateEndpoint(int apiLookupId, CreateEndpointRequest request);
    }
}
