using System.Collections.Generic;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Domain;

namespace TokenAdministrationApi.V1.Gateways
{
    public interface ITokensGateway
    {
        List<AuthToken> GetAllTokens(int limit, int cursor, bool? enabled);
        int GenerateToken(TokenRequestObject tokenRequestObject);
    }
}
