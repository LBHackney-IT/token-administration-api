using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.Factories;
using TokenAdministrationApi.V1.Gateways;
using TokenAdministrationApi.V1.UseCase.Interfaces;

namespace TokenAdministrationApi.V1.UseCase
{
    //TODO: Rename class name and interface name to reflect the entity they are representing eg. GetAllClaimantsUseCase
    public class GetAllTokensUseCase : IGetAllUseCase
    {
        private readonly ITokensGateway _gateway;
        public GetAllUseCase(ITokensGateway gateway)
        {
            _gateway = gateway;
        }

        public ResponseObjectList Execute()
        {
            return new ResponseObjectList { ResponseObjects = _gateway.GetAll().ToResponse() };
        }
    }
}
