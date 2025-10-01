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
        public AppUserController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }
        [HttpPost]
        public async Task<ActionResult<RegistrationStatusDTO>> CreateAppUser([FromBody] AppUserCreateDTO appUserDTO)
        {
            var result = await _appUserService.RegisterAppUser(appUserDTO);
            if (result.VerificationStatus == VerificationStatus.Rejected.ToString())
            {
                return Conflict(result);
            }
            return CreatedAtAction(null, result);
        }
    }
}
