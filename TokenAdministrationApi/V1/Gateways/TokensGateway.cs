using System.Collections.Generic;
using System.Linq;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Factories;
using TokenAdministrationApi.V1.Infrastructure;

namespace TokenAdministrationApi.V1.Gateways
{
    public class TokensGateway : ITokensGateway
    {
        private readonly TokenDatabaseContext _databaseContext;

        public TokensGateway(TokenDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<AuthToken> GetAllTokens(bool? enabled)
        {
            var tokenRecords = enabled != null ?
                _databaseContext.Tokens.Where(x => x.Enabled == enabled).ToList() : _databaseContext.Tokens.ToList();

            var tokenRecordsWithLookupValues = tokenRecords
              .Select(GetLookupValues)
              .ToList();

            return tokenRecordsWithLookupValues;
        }

        private AuthToken GetLookupValues(AuthTokens tokenRecord)
        {
            var api = _databaseContext.ApiNameLookups.FirstOrDefault(x => x.Id == tokenRecord.ApiLookupId);

            var apiEndpoint = _databaseContext.ApiEndpointNameLookups.FirstOrDefault(x => x.Id == tokenRecord.ApiEndpointNameLookupId);

            var consumerType = _databaseContext.ConsumerTypeLookups.FirstOrDefault(x => x.Id == tokenRecord.ConsumerTypeLookupId);

            return tokenRecord.ToDomain(apiEndpoint.ApiEndpointName, api.ApiName, consumerType.TypeName);
        }
    }
}
