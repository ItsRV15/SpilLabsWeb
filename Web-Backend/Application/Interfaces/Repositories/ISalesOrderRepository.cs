using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ISalesOrderRepository
{
    Task<IEnumerable<SalesOrder>> GetAllAsync();

    Task<SalesOrder?> GetByIdAsync(int id);

    Task AddAsync(SalesOrder order);

    Task UpdateAsync(SalesOrder order);

    Task DeleteAsync(int id);

    Task SaveChangesAsync();
}