using API.Models;
using Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IItemRepository _itemRepository;

    public ItemsController(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllItems()
    {
        var items = await _itemRepository.GetAllAsync();
        var result = items.Select(item => new ItemDto
        {
            Id = item.ItemId,
            Code = item.ItemCode,
            Description = item.Description,
            Price = item.Price
        });

        return Ok(result);
    }
}
