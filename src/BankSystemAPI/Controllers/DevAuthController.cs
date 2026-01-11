using Application.Interfaces.Services.Auths;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankSystemAPI.Controllers
{
    [ApiController]
    [Route("api/dev-auth")]
    [ApiExplorerSettings(IgnoreApi = true)] 
    public class DevAuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _env;

        public DevAuthController(IAuthService authService, IWebHostEnvironment env)
        {
            _authService = authService;
            _env = env;
        }

        [HttpGet("swagger-login")]
        public async Task<IActionResult> SwaggerLogin()
        {
            if (!_env.IsDevelopment())
                return Forbid();

            var email = "ais@gmail.com";
            var pin = "0185";

            var result = await _authService.LoginPin(email, pin);
            if (result.VerificationStatus != VerificationStatus.Verified.ToString())
                return BadRequest(result);

            return Ok(new { token = result.Token });
        }
    }

}
