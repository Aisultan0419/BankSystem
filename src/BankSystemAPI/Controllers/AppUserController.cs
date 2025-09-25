using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Application.DTO;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
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
