using System.Linq.Expressions;
using FeeLink.Application.Common.Results;

namespace FeeLink.Application.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<ListResult<T>> ListAllAsync();
    Task<ListResult<T>> ListAsync(int page = 1, int pageSize = 10, Expression<Func<T, bool>>? filter = null);
    Task InsertAsync(T entity);
    Task UpdateAsync(T entity);
    Task HardDeleteAsync(Guid id);
    Task SoftDeleteAsync(Guid id);
}