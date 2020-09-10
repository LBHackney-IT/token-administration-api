using TokenAdministrationApi.V1.Boundary.Request;
using TokenAdministrationApi.V1.Boundary.Response;
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
            return new TokensListResponse { Tokens = _gateway.GetAllTokens(request.Enabled) };
        }
    }
}
