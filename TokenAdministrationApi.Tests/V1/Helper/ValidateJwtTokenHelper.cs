using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Bogus;
using Microsoft.IdentityModel.Tokens;
using TokenAdministrationApi.V1.Domain;

namespace TokenAdministrationApi.Tests.V1.Helper
{
    public static class ValidateJwtTokenHelper
    {
        public static List<Claim> GetJwtClaims(string token, string secret)
        {
            var key = Encoding.ASCII.GetBytes(secret);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            var claims = handler.ValidateToken(token, validations, out var tokenSecure);
            return claims.Claims.ToList();
        }

        public static JwtSecurityToken GetToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(token);
        }
        public static GenerateJwtRequest GenerateJwtRequestObject(DateTime? expiresAt = null)
        {
            var faker = new Faker();
            return new GenerateJwtRequest
            {
                ConsumerName = faker.Name.FullName(),
                ConsumerType = faker.Random.Int(1, 2),
                ExpiresAt = expiresAt != null ? expiresAt : null,
                Id = faker.Random.Int(1, 100)
            };
        }
    }
}
