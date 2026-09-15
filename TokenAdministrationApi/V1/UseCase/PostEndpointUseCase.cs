using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Gateways;
using TokenAdministrationApi.V1.UseCase.Interfaces;

namespace TokenAdministrationApi.V1.UseCase
{
    public class PostEndpointUseCase : IPostEndpointUseCase
    {
        private readonly ITokenConfigurationGateway _gateway;

        public PostEndpointUseCase(ITokenConfigurationGateway gateway)
        {
            _gateway = gateway;
        }

        public ApiEndpointOption Execute(ApiEndpointOption endpoint)
        {
            return _gateway.CreateEndpoint(endpoint);
        }
    }
}
