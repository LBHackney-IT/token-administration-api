using System.Collections.Generic;
using TokenAdministrationApi.V1.Domain;

namespace TokenAdministrationApi.V1.Gateways
{
    public interface ITokensGateway
    {
        List<AuthToken> GetAllTokens(bool? enabled);
    }
}
