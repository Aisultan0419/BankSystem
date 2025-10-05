using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;
using Application.DTO.AppUserDTO;
using Application.DTO.ClientDTO;
namespace BankSystemAPI.Controllers
{
    [ApiController]
    [Route("api/app-users")]
    public class AppUserController : ControllerBase
    {
        private readonly IAppUserService _appUserService;
        private readonly ILogger<AppUserController> _logger;
        public AppUserController(IAppUserService appUserService, ILogger<AppUserController> logger)
        {
            _appUserService = appUserService;
            _logger = logger;
        }
        [HttpPost]
        public async Task<ActionResult<RegistrationStatusDTO>> CreateAppUser([FromBody] AppUserCreateDTO appUserDTO)
        {
            _logger.LogInformation("Creating new app-user... {email}", appUserDTO.Email);
            var result = await _appUserService.RegisterAppUser(appUserDTO);
            if (result.VerificationStatus == VerificationStatus.Rejected.ToString())
            {
                _logger.LogWarning("App-user {email} could not register - {info}", appUserDTO.Email, result.Message);
                return Conflict(result);
            }
            _logger.LogInformation("App-user created successfully - {email}", appUserDTO.Email);    
            return CreatedAtAction(null, result);
        }
    }
}
