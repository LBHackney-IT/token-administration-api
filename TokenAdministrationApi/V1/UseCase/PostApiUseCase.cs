using System;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Boundary.Response;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Domain.Exceptions;
using TokenAdministrationApi.V1.Factories;
using TokenAdministrationApi.V1.Gateways;
using TokenAdministrationApi.V1.UseCase.Interfaces;

namespace TokenAdministrationApi.V1.UseCase
{
    public class PostApiUseCase : IPostApiUseCase
    {
        private readonly ITokensGateway _gateway;

        public PostApiUseCase(ITokensGateway gateway)
        {
            _gateway = gateway;
        }
        public ApiLookupOptionResponse Execute(CreateApiLookupRequest request)
        {
            return _gateway.CreateApiLookup(request);
        }
        
    }
}