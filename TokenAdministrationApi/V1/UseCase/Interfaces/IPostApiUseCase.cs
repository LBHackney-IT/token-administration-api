using TokenAdministrationApi.V1.Domain;

namespace TokenAdministrationApi.V1.UseCase.Interfaces
{
    public interface IPostApiUseCase
    {
        ApiLookupOption Execute(ApiLookupOption api);
    }
}
