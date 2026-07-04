using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IClientRepository
{
    Task<IEnumerable<Client>> GetAllAsync();
    Task<Client?> GetByIdAsync(int id);
}