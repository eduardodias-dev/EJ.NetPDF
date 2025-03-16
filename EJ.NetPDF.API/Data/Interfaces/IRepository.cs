using EJ.NetPDF.API.Models;

namespace EJ.NetPDF.API.Data.Interfaces;

public interface IRepository<T> where T : Entity
{
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync(int offset, int limit);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}