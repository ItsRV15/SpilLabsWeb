using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService; //nstead of creating the service ourselves, 
    //ASP.NET Core provides it automatically because we registered it in Program.cs.

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllClients()
    {
        var clients = await _clientService.GetAllAsync();

        return Ok(clients);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClientById(int id)
    {
        var client = await _clientService.GetByIdAsync(id);

        if (client == null)
            return NotFound();

        return Ok(client);
    }
}