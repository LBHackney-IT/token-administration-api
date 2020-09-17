using System.Collections.Generic;
using System.Linq;
using TokenAdministrationApi.V1.Boundary.Request;
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Factories;
using TokenAdministrationApi.V1.Gateways;
using TokenAdministrationApi.V1.UseCase.Interfaces;

namespace TokenAdministrationApi.V1.UseCase
{
    public class GetAllTokensUseCase : IGetAllTokensUseCase
    {
        private readonly ITokensGateway _gateway;
        public GetAllTokensUseCase(ITokensGateway gateway)
        {
            _gateway = gateway;
        }

        public TokensListResponse Execute(GetTokensRequest request)
        {
            var limit = request.Limit < 10 ? 10 : request.Limit;
            limit = request.Limit > 100 ? 100 : limit;
            var tokens = _gateway.GetAllTokens(limit, request.Cursor, request.Enabled);
            return new TokensListResponse
            {
                Tokens = tokens,
                NextCursor = GetNextCursor(tokens, limit)

            };

        }
        private static string GetNextCursor(List<AuthToken> tokens, int limit)
        {
            return tokens.Count == limit ? tokens.Max(r => r.Id).ToString() : null;
        }
    }
}
