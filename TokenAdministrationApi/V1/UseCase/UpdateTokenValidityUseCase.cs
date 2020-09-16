using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Domain.Exceptions;
using TokenAdministrationApi.V1.Gateways;
using TokenAdministrationApi.V1.UseCase.Interfaces;

namespace TokenAdministrationApi.V1.UseCase
{
    public class UpdateTokenValidityUseCase : IUpdateTokenValidityUseCase
    {
        private readonly ITokensGateway _tokensGateway;

        public UpdateTokenValidityUseCase(ITokensGateway tokensGateway)
        {
            _tokensGateway = tokensGateway;
        }

        public void Execute(int tokenId, UpdateTokenRequest request)
        {
            var response = _tokensGateway.UpdateToken(tokenId, request.Enabled);
            if (response == null) throw new TokenRecordNotFoundException();
        }
    }
}
