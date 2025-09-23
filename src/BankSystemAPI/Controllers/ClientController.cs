using Application.DTO;
using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BankSystemAPI.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }
        [HttpPost]
        public async Task<ActionResult<RegistrationStatusDTO>> CreateClient([FromBody] ClientCreateDTO clientDTO)
        {
            var result = await _clientService.Register(clientDTO);
            if (result.KycStatus == KycStatus.Rejected.ToString())
            {
                return Conflict(result);
            }
            return CreatedAtAction(null, result);
        }
        [HttpDelete("{iin}")]
        public async Task<IActionResult> Delete([FromQuery] string IIN)
        {
            bool result = await _clientService.DeleteClient(IIN);
            if (result == false)
            {
                return NotFound("Client was not found");
            }
            else
            {
                return NoContent();
            }
        }
    }
}
