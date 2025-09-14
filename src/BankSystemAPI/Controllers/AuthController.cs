using Application.DTO;
using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BankSystemAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<ActionResult<LoginStatusDTO>> Login(string email, string password)
        {
            var result = await _authService.Login(email, password);
            if (result.VerificationStatus == VerificationStatus.Rejected.ToString())
            {
                return BadRequest(result);
            }
            else
            {
                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(5)
                });
                return Ok(result);
            }
        }
        [HttpPost("refresh")]
        public async Task<ActionResult<LoginStatusDTO>> RefreshToken()
        {
            var result = await _authService.RefreshToken();
            if (result.VerificationStatus == VerificationStatus.Rejected.ToString())
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }
    }
}
