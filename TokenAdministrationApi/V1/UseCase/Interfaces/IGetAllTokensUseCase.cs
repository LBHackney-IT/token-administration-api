using TokenAdministrationApi.V1.Boundary.Request;
using TokenAdministrationApi.V1.Boundary.Response;

namespace TokenAdministrationApi.V1.UseCase.Interfaces
{
    public interface IGetAllTokensUseCase
    {
        TokensListResponse Execute(GetTokensRequest request);
    }
}
