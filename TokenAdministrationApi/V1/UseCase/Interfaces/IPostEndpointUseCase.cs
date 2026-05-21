using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Boundary.Response;

namespace TokenAdministrationApi.V1.UseCase.Interfaces
{
    public interface IPostEndpointUseCase
    {
         ApiEndpointOptionResponse Execute(CreateEndpointRequest request);
    }
}
