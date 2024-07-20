using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebApiGateWay.Entidades.Context;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace WebApiGateWay.Services
{
    public interface IAuthorizationMiddlewareService
    {
        public Task InvokeAsync(HttpContext context);
    }

    public class AuthorizationMiddlewareService : IAuthorizationMiddlewareService
    {
        private readonly RequestDelegate _next;
        private readonly UltimaMilla2Context _context;
        private readonly IJwtService _jwtService;

        public AuthorizationMiddlewareService(RequestDelegate next, IConfiguration configuration, UltimaMilla2Context context, IJwtService jwtService)
        {
            _next = next;
            _context = context;
            _jwtService = jwtService;
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

            try
            {
                var principal = _jwtService.ValidateToken(token);
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
