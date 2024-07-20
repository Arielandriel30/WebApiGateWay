using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiGateWay.Entidades.Context;
using WebApiGateWay.Services;

namespace WebApiGateWay.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly JwtService _jwtService;
        private readonly UltimaMilla2Context _context;

        public AuthController(JwtService jwtService, UltimaMilla2Context context)
        {
            _jwtService = jwtService;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            ////////
            return Ok();
        }
    }
}
