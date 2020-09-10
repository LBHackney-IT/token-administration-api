using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Domain;

namespace TokenAdministrationApi.V1.Factories
{
    public static class GenerateJwtFactory
    {
        public static GenerateJwtRequest ToJwtRequest(TokenRequestObject tokenRequestObject, int id)
        {
            return new GenerateJwtRequest
            {
                ConsumerName = tokenRequestObject.Consumer,
                ConsumerType = tokenRequestObject.ConsumerType,
                ExpiresAt = tokenRequestObject.ExpiresAt,
                Id = id
            };
        }
    }
}
