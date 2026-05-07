using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.Gateways;
using TokenAdministrationApi.V1.UseCase.Interfaces;

namespace TokenAdministrationApi.V1.UseCase
{
   public class GetTokenOptionsUseCase : IGetTokenOptionsUseCase
    {
        private readonly ITokensGateway _gateway;
        public GetTokenOptionsUseCase(ITokensGateway gateway)
        {
            _gateway = gateway;
        }
        
        public TokenOptionsResponse Execute()
        {
            return _gateway.GetTokenOptions();
        }
    }
}