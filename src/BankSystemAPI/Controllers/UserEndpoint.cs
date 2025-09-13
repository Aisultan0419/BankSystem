using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Application.DTO;
using Domain.Enums;
using Domain.Models;
namespace BankSystemAPI.Controllers
{
    [ApiController]
    [Route("clientController/[controller]")]
    public class UserEndpoint : ControllerBase
    {
        private readonly IUserRegisterService _userRegisterService;
        public UserEndpoint(IUserRegisterService userRegisterService)
        {
            _userRegisterService = userRegisterService;
        }
        [HttpPost("RegisterEndpoint")]
        public async Task<ActionResult<RegistrationStatusDTO>> ClientRegisterEndpoint([FromBody] ClientCreateDTO clientDTO)
        {
            var result = await _userRegisterService.Register(clientDTO);
            if (result.KycStatus == KycStatus.Rejected.ToString())
            {
                return Conflict(result);
            }
            return CreatedAtAction(null, result);
        }
        [HttpDelete("DeleteEndpoint")]
        public async Task<IActionResult> DeleteEndpoint([FromQuery] string IIN)
        { 
            bool result = await _userRegisterService.DeleteClient(IIN);
            if (result == false)
            {
                return NotFound("Client was not found");
            }
            else
            {
                return NoContent();
            }
        }
        [HttpPost("AppUserRegisterEndpoint")]
        public async Task<ActionResult<RegistrationStatusDTO>> AppUserRegisterEndpoint([FromBody] AppUserCreateDTO appUserDTO)
        {
            var result = await _userRegisterService.RegisterAppUser(appUserDTO);
            if (result.VerificationStatus == VerificationStatus.Rejected.ToString())
            {
                return Conflict(result);
            }
            return CreatedAtAction(null, result);
        }
        [HttpPost("Login")]
        public async Task<ActionResult<LoginStatusDTO>> Login(string email, string password)
        {
            var result = await _userRegisterService.Login(email, password);
            if (result.VerificationStatus == VerificationStatus.Rejected.ToString())
            {
                return BadRequest(result);
            }
            else
            {
                Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(5)
                });
                return Ok(result);
            }
        }
        [HttpPost("Refresh-Token")]
        public async Task<ActionResult<LoginStatusDTO>> RefreshToken()
        {
            var result = await _userRegisterService.RefreshToken();
            if (result.VerificationStatus == VerificationStatus.Rejected.ToString())
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }
    }
}
