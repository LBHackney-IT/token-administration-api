using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using TokenAdministrationApi.V1.Boundary.Requests;
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

        public int GenerateToken(TokenRequestObject tokenRequestObject)
        {
            var tokenToInsert = new AuthTokens
            {
                ApiEndpointNameLookupId = tokenRequestObject.ApiEndpoint,
                ApiLookupId = tokenRequestObject.ApiName,
                HttpMethodType = tokenRequestObject.HttpMethodType.ToUpper(CultureInfo.InvariantCulture),
                ConsumerName = tokenRequestObject.Consumer,
                ConsumerTypeLookupId = tokenRequestObject.ConsumerType,
                Environment = tokenRequestObject.Environment,
                AuthorizedBy = tokenRequestObject.AuthorizedBy,
                RequestedBy = tokenRequestObject.RequestedBy,
                DateCreated = DateTime.Now,
                ExpirationDate = tokenRequestObject.ExpiresAt,
                Enabled = true
            };

            _databaseContext.Tokens.Add(tokenToInsert);
            _databaseContext.SaveChanges();

            return tokenToInsert.Id;
        }
    }
}
