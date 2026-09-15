using System.Threading.Tasks;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Gateways;
using TokenAdministrationApi.V1.UseCase.Interfaces;

namespace TokenAdministrationApi.V1.UseCase
{
    public class GetTokenOptionsUseCase : IGetTokenOptionsUseCase
    {
        private readonly ITokenConfigurationGateway _gateway;
        public GetTokenOptionsUseCase(ITokenConfigurationGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<TokenOptions> Execute()
        {
            return await _gateway.GetTokenOptions();
        }
    }
}
