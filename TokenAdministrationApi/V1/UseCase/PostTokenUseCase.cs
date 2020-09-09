using System;
using ApiAuthTokenGenerator.V1.Boundary.Response;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Factories;
using TokenAdministrationApi.V1.Gateways;
using TokenAdministrationApi.V1.UseCase.Interfaces;

namespace TokenAdministrationApi.V1.UseCase
{
    public class PostTokenUseCase : IPostTokenUseCase
    {
        private IGenerateJwtUseCase _generateJwtUseCase;
        private ITokensGateway _gateway;
        public PostTokenUseCase(IGenerateJwtUseCase generateJwtUseCase, ITokensGateway gateway)
        {
            _generateJwtUseCase = generateJwtUseCase;
            _gateway = gateway;
        }
        public GenerateTokenResponse Execute(TokenRequestObject tokenRequest)
        {
            var tokenId = _gateway.GenerateToken(tokenRequest);
            if (tokenId != 0)
            {
                var jwtToken = _generateJwtUseCase.GenerateJwtToken(GenerateJwtFactory.ToJwtRequest(tokenRequest, tokenId));
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    return new GenerateTokenResponse
                    {
                        Id = tokenId,
                        Token = jwtToken,
                        ExpiresAt = tokenRequest.ExpiresAt,
                        GeneratedAt = DateTime.Now
                    };
                }
                //TODO add logic to revert inserted record or update inserted record to reflect that JWT has not been generated
                throw new JwtTokenNotGeneratedException();
            }
            throw new TokenNotInsertedException();
        }
    }
}
