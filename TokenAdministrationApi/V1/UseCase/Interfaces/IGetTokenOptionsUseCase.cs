using System.Threading.Tasks;
using TokenAdministrationApi.V1.Boundary.Response;

namespace TokenAdministrationApi.V1.UseCase.Interfaces
{
    public interface IGetTokenOptionsUseCase
    {
        Task<TokenOptionsResponse> ExecuteAsync();
    }
}
