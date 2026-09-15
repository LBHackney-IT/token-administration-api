using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Gateways;
using TokenAdministrationApi.V1.UseCase.Interfaces;

namespace TokenAdministrationApi.V1.UseCase
{
    public class PostApiUseCase : IPostApiUseCase
    {
        private readonly ITokenConfigurationGateway _gateway;

        public PostApiUseCase(ITokenConfigurationGateway gateway)
        {
            _gateway = gateway;
        }
        public ApiLookupOption Execute(ApiLookupOption api)
        {
            return _gateway.CreateApiLookup(api);
        }
    }
}
