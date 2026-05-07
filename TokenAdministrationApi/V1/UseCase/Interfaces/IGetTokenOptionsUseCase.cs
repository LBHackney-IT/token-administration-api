using TokenAdministrationApi.V1.Boundary.Response;

namespace TokenAdministrationApi.V1.UseCase.Interfaces
{
    public interface IGetTokenOptionsUseCase

    {
        TokenOptionsResponse Execute();
    }
}
