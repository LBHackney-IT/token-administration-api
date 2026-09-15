using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TokenAdministrationApi.V1.Boundary.Requests;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.Factories;
using TokenAdministrationApi.V1.Infrastructure;
using TokenAdministrationApi.V1.Domain.Exceptions;

namespace TokenAdministrationApi.V1.Gateways
{
    public class TokensGateway : ITokensGateway
    {
        private readonly TokenDatabaseContext _databaseContext;
        private readonly ILogger<TokensGateway> _logger;

        public TokensGateway(TokenDatabaseContext databaseContext, ILogger<TokensGateway> logger)
        {
            _databaseContext = databaseContext;
            _logger = logger;
        }

        public List<AuthToken> GetAllTokens(int limit, int cursor, bool? enabled)
        {
            var tokenRecords = _databaseContext.Tokens
                .Where(x => enabled == null || x.Enabled == enabled)
                .Where(x => x.Id > cursor)
                .Include(x => x.ApiLookup)
                .Include(x => x.ApiEndpointNameLookup)
                .Include(x => x.ConsumerTypeLookup)
                .OrderBy(x => x.Id)
                .Take(limit)
                .ToList();

            return tokenRecords
                .Select(t => t.ToDomain())
                .ToList();
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

            try
            {
                _databaseContext.Tokens.Add(tokenToInsert);
                _databaseContext.SaveChanges();
            }
            catch (DbUpdateException ex) when
                (ex.InnerException is Npgsql.PostgresException postgresException && postgresException.SqlState == "23503")
            {
                //23503 error code = foreign_key_violation
                _logger.LogWarning(ex, "Token creation failed because a configuration selection does not exist.");
                throw new LookupValueDoesNotExistException(GetInvalidSelectionMessage(postgresException.ConstraintName));
            }
            return tokenToInsert.Id;
        }

        private static string GetInvalidSelectionMessage(string constraintName)
        {
            switch (constraintName)
            {
                case "FK_tokens_api_lookup_api_lookup_id":
                    return "The selected API does not exist.";
                case "FK_tokens_api_endpoint_lookup_api_endpoint_lookup_id":
                    return "The selected API endpoint does not exist.";
                case "FK_tokens_consumer_type_lookup_consumer_type_lookup":
                    return "The selected consumer type does not exist.";
                default:
                    return "One or more selected configuration values do not exist.";
            }
        }

        public int? UpdateToken(int tokenId, bool enabled)
        {
            var token = _databaseContext.Tokens.Find(tokenId);
            if (token == null) return null;
            token.Enabled = enabled;
            _databaseContext.SaveChanges();
            return token.Id;
        }

    }
}
