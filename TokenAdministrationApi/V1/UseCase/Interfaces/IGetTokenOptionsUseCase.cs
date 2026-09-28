using System.Threading.Tasks;
using TokenAdministrationApi.V1.Domain;

namespace TokenAdministrationApi.V1.UseCase.Interfaces
{
    public interface IGetTokenOptionsUseCase
    {
        Task<TokenOptions> Execute();
    }
}
