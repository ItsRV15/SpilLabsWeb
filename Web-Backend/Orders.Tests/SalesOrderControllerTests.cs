using API.Controllers;
using API.Models;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Orders.Tests;

public class SalesOrderControllerTests
{
    [Fact]
    public async Task CreateOrder_ShouldAssignItemId_FromMatchingItemCode()
    {
        var salesOrderRepository = new FakeSalesOrderRepository();
        var itemRepository = new FakeItemRepository(new List<Item>
        {
            new() { ItemId = 7, ItemCode = "ITM001", Description = "Dell 24\" Monitor", Price = 45000m }
        });

        var controller = new OrdersController(salesOrderRepository, itemRepository);

        var request = new CreateSalesOrderRequest
        {
            CustomerId = 1,
            InvoiceNo = "INV-9001",
            InvoiceDate = new DateTime(2026, 7, 4),
            ReferenceNo = "REF-9001",
            Note = "Test order",
            TotalExcl = 100m,
            TotalTax = 15m,
            TotalIncl = 115m,
            Items = new List<SalesOrderItemDto>
            {
                new()
                {
                    ItemCode = "ITM001",
                    Description = "Dell 24\" Monitor",
                    Quantity = 1,
                    Price = 100m,
                    TaxRate = 15m,
                    ExclAmount = 100m,
                    TaxAmount = 15m,
                    InclAmount = 115m
                }
            }
        };

        var result = await controller.CreateOrder(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var createdOrder = Assert.IsType<SalesOrderDto>(createdResult.Value);

        Assert.Equal("INV-9001", createdOrder.InvoiceNo);
        Assert.NotEmpty(salesOrderRepository.Orders);
        Assert.Equal(7, salesOrderRepository.Orders.Single().SalesOrderDetails.Single().ItemId);
    }

    private sealed class FakeSalesOrderRepository : ISalesOrderRepository
    {
        public List<SalesOrder> Orders { get; } = new();

        public Task<IEnumerable<SalesOrder>> GetAllAsync() => Task.FromResult(Orders.AsEnumerable());

        public Task<SalesOrder?> GetByIdAsync(int id) => Task.FromResult(Orders.FirstOrDefault(o => o.OrderId == id));

        public Task AddAsync(SalesOrder order)
        {
            order.OrderId = Orders.Count + 1;
            Orders.Add(order);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(SalesOrder order)
        {
            var index = Orders.FindIndex(o => o.OrderId == order.OrderId);
            if (index >= 0)
            {
                Orders[index] = order;
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            Orders.RemoveAll(o => o.OrderId == id);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    private sealed class FakeItemRepository : IItemRepository
    {
        private readonly IReadOnlyList<Item> _items;

        public FakeItemRepository(IReadOnlyList<Item> items)
        {
            _items = items;
        }

        public Task<IEnumerable<Item>> GetAllAsync() => Task.FromResult(_items.AsEnumerable());

        public Task<Item?> GetByIdAsync(int id) => Task.FromResult(_items.FirstOrDefault(i => i.ItemId == id));
    }
}
