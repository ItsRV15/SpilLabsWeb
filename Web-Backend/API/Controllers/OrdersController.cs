using API.Models;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ISalesOrderRepository _salesOrderRepository;
    private readonly IItemRepository _itemRepository;

    public OrdersController(ISalesOrderRepository salesOrderRepository, IItemRepository itemRepository)
    {
        _salesOrderRepository = salesOrderRepository;
        _itemRepository = itemRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _salesOrderRepository.GetAllAsync();
        var result = orders.Select(order => new SalesOrderDto
        {
            Id = order.OrderId,
            CustomerId = order.ClientId,
            CustomerName = order.Client?.CustomerName ?? string.Empty,
            Address1 = order.Client?.Address1,
            Address2 = order.Client?.Address2,
            Address3 = order.Client?.Address3,
            Suburb = order.Client?.Suburb,
            State = order.Client?.State,
            PostCode = order.Client?.PostCode,
            InvoiceNo = order.InvoiceNo,
            InvoiceDate = order.InvoiceDate,
            ReferenceNo = order.ReferenceNo,
            Note = order.Note,
            TotalExcl = order.TotalExcl,
            TotalTax = order.TotalTax,
            TotalIncl = order.TotalIncl,
            CreatedAt = order.InvoiceDate,
            Items = order.SalesOrderDetails.Select(detail => new SalesOrderItemDto
            {
                ItemCode = detail.Item?.ItemCode,
                Description = detail.Item?.Description,
                Note = detail.Note,
                Quantity = detail.Quantity,
                Price = detail.Price,
                TaxRate = detail.TaxRate,
                ExclAmount = detail.ExclAmount,
                TaxAmount = detail.TaxAmount,
                InclAmount = detail.InclAmount
            }).ToList()
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _salesOrderRepository.GetByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        var result = new SalesOrderDto
        {
            Id = order.OrderId,
            CustomerId = order.ClientId,
            CustomerName = order.Client?.CustomerName ?? string.Empty,
            Address1 = order.Client?.Address1,
            Address2 = order.Client?.Address2,
            Address3 = order.Client?.Address3,
            Suburb = order.Client?.Suburb,
            State = order.Client?.State,
            PostCode = order.Client?.PostCode,
            InvoiceNo = order.InvoiceNo,
            InvoiceDate = order.InvoiceDate,
            ReferenceNo = order.ReferenceNo,
            Note = order.Note,
            TotalExcl = order.TotalExcl,
            TotalTax = order.TotalTax,
            TotalIncl = order.TotalIncl,
            CreatedAt = order.InvoiceDate,
            Items = order.SalesOrderDetails.Select(detail => new SalesOrderItemDto
            {
                ItemCode = detail.Item?.ItemCode,
                Description = detail.Item?.Description,
                Note = detail.Note,
                Quantity = detail.Quantity,
                Price = detail.Price,
                TaxRate = detail.TaxRate,
                ExclAmount = detail.ExclAmount,
                TaxAmount = detail.TaxAmount,
                InclAmount = detail.InclAmount
            }).ToList()
        };

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateSalesOrderRequest request)
    {
        var allItems = (await _itemRepository.GetAllAsync()).ToList();

        var order = new SalesOrder
        {
            ClientId = request.CustomerId,
            InvoiceNo = request.InvoiceNo,
            InvoiceDate = request.InvoiceDate,
            ReferenceNo = request.ReferenceNo,
            Note = request.Note,
            TotalExcl = request.TotalExcl,
            TotalTax = request.TotalTax,
            TotalIncl = request.TotalIncl,
            SalesOrderDetails = request.Items.Select(detail =>
            {
                var matchedItem = allItems.FirstOrDefault(item =>
                    string.Equals(item.ItemCode, detail.ItemCode, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(item.Description, detail.Description, StringComparison.OrdinalIgnoreCase));

                return new SalesOrderDetail
                {
                    ItemId = matchedItem?.ItemId ?? 0,
                    Note = detail.Note,
                    Quantity = detail.Quantity,
                    Price = detail.Price,
                    TaxRate = detail.TaxRate,
                    ExclAmount = detail.ExclAmount,
                    TaxAmount = detail.TaxAmount,
                    InclAmount = detail.InclAmount
                };
            }).ToList()
        };

        await _salesOrderRepository.AddAsync(order);
        await _salesOrderRepository.SaveChangesAsync();

        var createdOrder = new SalesOrderDto
        {
            Id = order.OrderId,
            CustomerId = order.ClientId,
            CustomerName = string.Empty,
            InvoiceNo = order.InvoiceNo,
            InvoiceDate = order.InvoiceDate,
            ReferenceNo = order.ReferenceNo,
            Note = order.Note,
            TotalExcl = order.TotalExcl,
            TotalTax = order.TotalTax,
            TotalIncl = order.TotalIncl,
            CreatedAt = order.InvoiceDate,
            Items = order.SalesOrderDetails.Select(detail => new SalesOrderItemDto
            {
                ItemCode = detail.Item?.ItemCode,
                Description = detail.Item?.Description,
                Note = detail.Note,
                Quantity = detail.Quantity,
                Price = detail.Price,
                TaxRate = detail.TaxRate,
                ExclAmount = detail.ExclAmount,
                TaxAmount = detail.TaxAmount,
                InclAmount = detail.InclAmount
            }).ToList()
        };

        return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, createdOrder);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] CreateSalesOrderRequest request)
    {
        var existing = await _salesOrderRepository.GetByIdAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        existing.ClientId = request.CustomerId;
        existing.InvoiceNo = request.InvoiceNo;
        existing.InvoiceDate = request.InvoiceDate;
        existing.ReferenceNo = request.ReferenceNo;
        existing.Note = request.Note;
        existing.TotalExcl = request.TotalExcl;
        existing.TotalTax = request.TotalTax;
        existing.TotalIncl = request.TotalIncl;

        var allItems = (await _itemRepository.GetAllAsync()).ToList();
        existing.SalesOrderDetails = request.Items.Select(detail =>
        {
            var matchedItem = allItems.FirstOrDefault(item =>
                string.Equals(item.ItemCode, detail.ItemCode, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(item.Description, detail.Description, StringComparison.OrdinalIgnoreCase));

            return new SalesOrderDetail
            {
                ItemId = matchedItem?.ItemId ?? 0,
                Note = detail.Note,
                Quantity = detail.Quantity,
                Price = detail.Price,
                TaxRate = detail.TaxRate,
                ExclAmount = detail.ExclAmount,
                TaxAmount = detail.TaxAmount,
                InclAmount = detail.InclAmount
            };
        }).ToList();

        await _salesOrderRepository.UpdateAsync(existing);
        await _salesOrderRepository.SaveChangesAsync();

        return Ok(request);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        await _salesOrderRepository.DeleteAsync(id);
        await _salesOrderRepository.SaveChangesAsync();
        return NoContent();
    }
}
