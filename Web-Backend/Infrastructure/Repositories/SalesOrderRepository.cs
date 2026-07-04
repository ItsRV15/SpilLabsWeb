using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SalesOrderRepository : ISalesOrderRepository
{
    private readonly ApplicationDbContext _context;

    public SalesOrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SalesOrder>> GetAllAsync()
    {
        return await _context.SalesOrders
            .AsNoTracking()
            .Include(o => o.Client)
            .Include(o => o.SalesOrderDetails)
            .ThenInclude(d => d.Item)
            .OrderByDescending(o => o.OrderId)
            .ToListAsync();
    }

    public async Task<SalesOrder?> GetByIdAsync(int id)
    {
        return await _context.SalesOrders
            .AsNoTracking()
            .Include(o => o.Client)
            .Include(o => o.SalesOrderDetails)
            .ThenInclude(d => d.Item)
            .FirstOrDefaultAsync(o => o.OrderId == id);
    }

    public async Task AddAsync(SalesOrder order)
    {
        await _context.SalesOrders.AddAsync(order);
    }

    public async Task UpdateAsync(SalesOrder order)
    {
        _context.SalesOrders.Update(order);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var existing = await _context.SalesOrders.FirstOrDefaultAsync(o => o.OrderId == id);
        if (existing != null)
        {
            _context.SalesOrders.Remove(existing);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
