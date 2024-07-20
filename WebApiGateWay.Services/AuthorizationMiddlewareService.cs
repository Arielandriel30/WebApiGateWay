using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiGateWay.Entidades.Context;
using Microsoft.Extensions.Configuration;// importamos para utilizar la conexion con settings json

namespace WebApiGateWay.Services
{
    public interface IAuthorizationMiddlewareService
    {
        public Task InvokeAsync(HttpContext context);
        

    }
    public class AuthorizationMiddlewareService : IAuthorizationMiddlewareService
    {
        private readonly RequestDelegate _next;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly UltimaMilla2Context _context; 

        public AuthorizationMiddlewareService(RequestDelegate next, IConfiguration configuration, UltimaMilla2Context context)
        {
            _next = next;
            _secretKey = configuration["Jwt:SecretKey"];
            _issuer = configuration["Jwt:Issuer"];
            _audience = configuration["Jwt:Audience"];
            _context = context;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/auth/login" || context.Request.Path == "/auth/loginTest")
            {
                await _next(context);
                return;
            }

            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("No está autorizado");
                return;
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                var claims = principal.Claims.ToDictionary(c => c.Type, c => c.Value);

                if (!await ValidatePayloadAsync(claims, context))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("No está autorizado");
                    return;
                }

                var customerUrl = await _context.Customers
                    .Where(c => c.Tag == claims["customer_tag"])
                    .Select(c => c.Url)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(customerUrl))
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("Error conectándose al servidor");
                    return;
                }

                context.Items["customer_baseurl"] = customerUrl;
                context.Items["payload"] = claims;
                context.Items["token"] = token;

                await _next(context);
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("No está autorizado");
            }
        }

        private async Task<bool> ValidatePayloadAsync(Dictionary<string, string> claims, HttpContext context)
        {
            var userId = ulong.Parse(claims["user_id"]);
            var customerTag = claims["customer_tag"];
            var device = claims["device"];
            var now = DateTime.UtcNow;

            var user = await _context.Users
                .Include(u => u.Customer)
                .Where(u => u.UserId == userId && u.Customer.Tag == customerTag && u.Device == device && u.Active == true)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return false; // El usuario no existe o no está activo
            }

            var tokenIssuedAt = long.Parse(claims["iat"]);
            if (user.TokensValidSince.HasValue && user.TokensValidSince.Value > DateTime.UtcNow)
            {
                return false; // Token emitido después de la última validación de tokens
            }

            return true;
        }
    }
}
