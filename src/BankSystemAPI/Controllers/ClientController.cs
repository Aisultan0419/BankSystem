using Application.DTO.ClientDTO;
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
        private readonly ILogger<ClientController> _logger;
        public ClientController(IClientService clientService, ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }
        [HttpPost]
        public async Task<ActionResult<RegistrationStatusDTO>> CreateClient([FromBody] ClientCreateDTO clientDTO)
        {
            _logger.LogInformation("CreateClient endpoint has started...");
            _logger.LogInformation("Creating new client... {full name}", clientDTO.FullName);
            var result = await _clientService.Register(clientDTO);
            if (result.KycStatus == KycStatus.Rejected.ToString())
            {
                _logger.LogWarning("Client {full name} could not register - {info}", clientDTO.FullName, result.Message);
                return Conflict(result);
            }
            _logger.LogInformation("Client created successfully - {full name}", clientDTO.FullName);
            return CreatedAtAction(null, result);
        }
        [HttpDelete("{iin}")]
        public async Task<IActionResult> Delete([FromQuery] IINDTO iinDTO)
        {
            _logger.LogInformation("Delete endpoint has started...");
            _logger.LogInformation("Attempting to delete client with IIN: {iin}", iinDTO.IIN);
            bool result = await _clientService.DeleteClient(iinDTO.IIN);
            if (result == false)
            {
                _logger.LogWarning("Client with IIN: {iin} was not found", iinDTO.IIN);
                return NotFound("Client was not found");
            }
            else
            {
                _logger.LogInformation("Client with IIN: {iin} deleted successfully", iinDTO.IIN);
                return NoContent();
            }
        }
    }
}
