using TokenAdministrationApi.V1.Domain;

namespace TokenAdministrationApi.V1.UseCase.Interfaces
{
    public interface IGenerateJwtUseCase
    {
        string GenerateJwtToken(GenerateJwtRequest tokenRequestObject);
    }
}
