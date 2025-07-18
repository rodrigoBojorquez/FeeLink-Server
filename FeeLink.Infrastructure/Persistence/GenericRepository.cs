using System.Linq.Expressions;
using System.Reflection;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FeeLink.Infrastructure.Persistence;

public class GenericRepository<T>(FeeLinkDbContext context) : IRepository<T>
    where T : class
{
    protected readonly FeeLinkDbContext Context = context;
    private readonly DbSet<T> _set = context.Set<T>();


    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await Context.Set<T>().FindAsync(id);
    }

    public async Task<ListResult<T>> ListAllAsync()
    {
        var total = await _set.CountAsync();
        var data = await _set.ToListAsync();

        return new ListResult<T>(Page: 1, PageSize: total, TotalItems: total, Items: data);
    }

    public async Task<ListResult<T>> ListAsync(int page = 1, int pageSize = 10,
        Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = _set;

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        var total = await query.CountAsync();
        var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new ListResult<T>(Page:page, PageSize:pageSize, TotalItems:total, Items:data, TotalPages:(int)Math.Ceiling(total / (double)pageSize));
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await Context.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task InsertAsync(T entity)
    {
        await Context.Set<T>().AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T obj)
    {
        Context.Set<T>().Attach(obj);
        Context.Entry(obj).State = EntityState.Modified;
        await Context.SaveChangesAsync();
        await Context.Entry(obj).ReloadAsync();
    }

    public async Task HardDeleteAsync(Guid id)
    {
        T? existing = await Context.Set<T>().FindAsync(id) ?? null;

        if (existing is not null)
        {
            Context.Set<T>().Remove(existing);
            await Context.SaveChangesAsync();
        }
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        T? existing = await Context.Set<T>().FindAsync(id) ?? null;

        if (existing is not null)
        {
            PropertyInfo? isDeletedProp = existing.GetType().GetProperty("IsDeleted");
            PropertyInfo? enabledProp = existing.GetType().GetProperty("Enabled");
            PropertyInfo? deletedDateProp = existing.GetType().GetProperty("DeleteDate");

            if (isDeletedProp != null && isDeletedProp.PropertyType == typeof(bool) && deletedDateProp != null &&
                enabledProp != null)
            {
                isDeletedProp.SetValue(existing, true);
                enabledProp.SetValue(existing, false);
                deletedDateProp.SetValue(existing, DateTime.UtcNow);
                await Context.SaveChangesAsync();
            }
        }
    }
}