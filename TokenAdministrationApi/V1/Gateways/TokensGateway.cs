using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
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

        public TokensGateway(TokenDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
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
            catch (DbUpdateException ex)
            {
                //23503 error code = foreign_key_violation
                if (ex.InnerException.GetType() == typeof(Npgsql.PostgresException) && (((Npgsql.PostgresException) ex.InnerException).SqlState == "23503"))
                {
                    throw new LookupValueDoesNotExistException(ex.InnerException.Message);
                }
            }
            return tokenToInsert.Id;
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
