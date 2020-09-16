using TokenAdministrationApi.V1.Boundary.Requests;

namespace TokenAdministrationApi.V1.UseCase.Interfaces
{
    public interface IUpdateTokenValidityUseCase
    {
        void Execute(int tokenId, UpdateTokenRequest request);
    }
}
