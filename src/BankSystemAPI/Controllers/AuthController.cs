using Application.DTO.AuthDTO;
using Application.Interfaces.Services.Auths;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;


namespace BankSystemAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        [HttpPost("login-pin")]
        public async Task<ActionResult<LoginStatusDTO>> LoginViaPin([FromBody] LoginViaPinDTO loginViaPinDTO)
        {
            _logger.LogInformation("LoginViaPin endpoint has started...");
            _logger.LogInformation("App-user attempting to log in via PIN... {email}", loginViaPinDTO.Email);
            var result = await _authService.LoginPin(loginViaPinDTO.Email!, loginViaPinDTO.PinCode);
            if (result.VerificationStatus == VerificationStatus.Rejected.ToString())
            {
                _logger.LogWarning("App-user {email} could not log in via PIN - {info}", loginViaPinDTO.Email, result.Message);
                return BadRequest(result);
            }
            else
            {
                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(5)
                });
                _logger.LogInformation("App-user logged in successfully via PIN - {email}", loginViaPinDTO.Email);
                return Ok(result);
            }
        }
        [HttpPost("login-password")]
        public async Task<ActionResult<LoginStatusDTO>> LoginViaPassword([FromBody] LoginViaPasswordDTO loginViaPasswordDTO)
        {
            _logger.LogInformation("LoginViaPassword endpoint has started...");
            _logger.LogInformation("App-user attempting to log in via Password... {email}", loginViaPasswordDTO.Email);
            var result = await _authService.LoginPassword(loginViaPasswordDTO.Email!, loginViaPasswordDTO.Password);
            if (result.VerificationStatus == VerificationStatus.Rejected.ToString())
            {
                _logger.LogWarning("App-user {email} could not log in via Password - {info}", loginViaPasswordDTO.Email, result.Message);
                return BadRequest(result);
            }
            else
            {
                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(5)
                });
                _logger.LogInformation("App-user logged in successfully via Password - {email}", loginViaPasswordDTO.Email);
                return Ok(result);
            }
        }
        [HttpPost("refresh")]
        public async Task<ActionResult<LoginStatusDTO>> RefreshToken()
        {
            _logger.LogInformation("App-user attempting to refresh token...");
            var result = await _authService.RefreshToken();
            if (result.VerificationStatus == VerificationStatus.Rejected.ToString())
            {
                _logger.LogWarning("App-user could not refresh token - {info}", result.Message);
                return Unauthorized(result);
            }
            _logger.LogInformation("App-user refreshed token successfully");
            return Ok(result);
        }
    }
}
