using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TokenAdministrationApi.V1.Domain;
using TokenAdministrationApi.V1.UseCase.Interfaces;

namespace TokenAdministrationApi.V1.UseCase
{
    public class GenerateJwtUseCase : IGenerateJwtUseCase
    {
        public string GenerateJwtToken(GenerateJwtRequest jwtRequestObject)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("jwtSecret"));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("id", jwtRequestObject.Id.ToString(CultureInfo.InvariantCulture)),
                    new Claim("consumerName", jwtRequestObject.ConsumerName),
                    new Claim("consumerType", jwtRequestObject.ConsumerType.ToString(CultureInfo.InvariantCulture)),
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Expires = jwtRequestObject.ExpiresAt != null ? jwtRequestObject.ExpiresAt : DateTime.Now.AddYears(10)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
