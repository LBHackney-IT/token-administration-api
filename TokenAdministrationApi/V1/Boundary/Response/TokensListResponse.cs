using System.Collections.Generic;
using TokenAdministrationApi.V1.Domain;

namespace TokenAdministrationApi.V1.Boundary.Response
{
    public class TokensListResponse
    {
        public List<AuthToken> Tokens { get; set; }
    }
}
