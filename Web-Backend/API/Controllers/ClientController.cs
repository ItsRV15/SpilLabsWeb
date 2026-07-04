using API.Models;
using Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientRepository _clientRepository;

    public ClientsController(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllClients()
    {
        var clients = await _clientRepository.GetAllAsync();
        var result = clients.Select(client => new ClientDto
        {
            Id = client.ClientId,
            Name = client.CustomerName,
            Address1 = client.Address1,
            Address2 = client.Address2,
            Address3 = client.Address3,
            Suburb = client.Suburb,
            State = client.State,
            PostCode = client.PostCode,
            Phone = client.Phone,
            Email = client.Email
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClientById(int id)
    {
        var client = await _clientRepository.GetByIdAsync(id);

        if (client == null)
            return NotFound();

        var result = new ClientDto
        {
            Id = client.ClientId,
            Name = client.CustomerName,
            Address1 = client.Address1,
            Address2 = client.Address2,
            Address3 = client.Address3,
            Suburb = client.Suburb,
            State = client.State,
            PostCode = client.PostCode,
            Phone = client.Phone,
            Email = client.Email
        };

        return Ok(result);
    }
}