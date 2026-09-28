using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Domain.Exceptions;
using TokenAdministrationApi.V1.Factories;
using TokenAdministrationApi.V1.Infrastructure;

namespace TokenAdministrationApi.V1.Gateways
{
    public class TokenConfigurationGateway : ITokenConfigurationGateway
    {
        private readonly TokenDatabaseContext _databaseContext;

        public TokenConfigurationGateway(TokenDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<TokenOptions> GetTokenOptions()
        {
            var tokenOptions = new TokenOptionsEntity
            {
                ConsumerTypes = await _databaseContext.ConsumerTypeLookups.ToListAsync(),
                ApiLookups = await _databaseContext.ApiNameLookups.ToListAsync(),
                ApiEndpoints = await _databaseContext.ApiEndpointNameLookups.ToListAsync()
            };

            return tokenOptions.ToDomain();
        }

        public ApiLookupOption CreateApiLookup(ApiLookupOption api)
        {
            var apiName = api.ApiName.Trim();
            var apiGatewayId = api.ApiGatewayId.Trim();

            var apiAlreadyExists = _databaseContext.ApiNameLookups.Any(existingApi =>
                existingApi.ApiName == apiName ||
                existingApi.ApiGatewayId == apiGatewayId);

            if (apiAlreadyExists)
            {
                throw new DuplicateApiException("API name or gateway ID already exists.");
            }

            var apiLookup = new ApiNameLookup
            {
                ApiName = apiName,
                ApiGatewayId = apiGatewayId
            };

            _databaseContext.ApiNameLookups.Add(apiLookup);
            _databaseContext.SaveChanges();

            return apiLookup.ToDomain();
        }

        public ApiEndpointOption CreateEndpoint(ApiEndpointOption endpointRequest)
        {
            var apiLookup = _databaseContext.ApiNameLookups.Find(endpointRequest.ApiLookupId);
            if (apiLookup == null)
            {
                throw new LookupValueDoesNotExistException("API lookup was not found.");
            }

            var endpointName = endpointRequest.EndpointName.Trim();
            var endpointAlreadyExists = _databaseContext.ApiEndpointNameLookups.Any(endpoint =>
                endpoint.ApiLookupId == endpointRequest.ApiLookupId &&
                endpoint.ApiEndpointName == endpointName);

            if (endpointAlreadyExists)
            {
                throw new DuplicateEndpointException("Endpoint already exists for this API.");
            }

            var endpoint = new ApiEndpointNameLookup
            {
                ApiLookupId = endpointRequest.ApiLookupId,
                ApiEndpointName = endpointName
            };

            _databaseContext.ApiEndpointNameLookups.Add(endpoint);
            _databaseContext.SaveChanges();

            return endpoint.ToDomain(apiLookup.ApiName);
        }
    }
}
