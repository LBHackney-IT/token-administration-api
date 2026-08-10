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
    public class PostEndpointUseCase : IPostEndpointUseCase
    {
        private readonly ITokensGateway _gateway;

        public PostEndpointUseCase(ITokensGateway gateway)
        {
            _gateway = gateway;
        }

        public CreateEndpointResponse Execute(int apiLookupId, CreateEndpointRequest request)
        {
            return _gateway.CreateEndpoint(apiLookupId, request);
        }
    }
}
