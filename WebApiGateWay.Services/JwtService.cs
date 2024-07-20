using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebApiGateWay.Services
{
    public interface IJwtService
    {
        public string GenerateToken(Dictionary<string, object> claims);
        public ClaimsPrincipal ValidateToken(string token);

    }
    public class JwtService : IJwtService
    {
        private readonly string _secretKey = "Clve_Secreta";

        public string GenerateToken(Dictionary<string, object> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.Select(c => new Claim(c.Key, c.Value.ToString())).ToList()),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = creds,
                Issuer = "urn:example:issuer",
                Audience = "urn:example:audience",
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "urn:example:issuer",
                ValidAudience = "urn:example:audience",
                IssuerSigningKey = key,
            };

            return tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
        }
    }

    
}
