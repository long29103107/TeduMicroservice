using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using TeduMicroservice.IDP.Infrastructure.Exceptions;
using TeduMicroservice.IDP.Persistence;
using IDbTransaction = System.Data.IDbTransaction;
using CommandType = System.Data.CommandType;

namespace TeduMicroservice.IDP.Infrastructure.Common.Domains;


public class RepositoryBase<T, K> : IRepositoryBase<T, K>
    where T : EntityBase<K>
{
    private readonly TeduIdentityContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public RepositoryBase(TeduIdentityContext context, IUnitOfWork unitOfWork)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<K> CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Unchanged)
            return;
        T exist = _context.Set<T>().Find(entity.Id);
        _context.Entry(exist).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateListAsync(IEnumerable<T> entities)
    {
        await _context.Set<T>().AddRangeAsync(entities);
        await SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await SaveChangesAsync();
    }

    public async Task DeleteListAsync(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
        await SaveChangesAsync();
    }

    public Task<int> SaveChangesAsync()
    {
        return _unitOfWork.CommitAsync();
    }

    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return _context.Database.BeginTransactionAsync();
    }

    public async Task EndTransactionAsync()
    {
        await SaveChangesAsync();
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }

    public IQueryable<T> FindAll(bool trackChanges = false)
    {
        return !trackChanges ? _context.Set<T>().AsNoTracking() : _context.Set<T>();
    }

    public IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        var items = FindAll(trackChanges);

        items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
        return items;
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false)
    {
        return !trackChanges ? _context.Set<T>().Where(expression).AsNoTracking() : _context.Set<T>().Where(expression);
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
    {
        var items = FindByCondition(expression, trackChanges);
        items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
        return items;
    }

    public async Task<T?> GetByIdAsync(K id)
    {
        return await FindByCondition(x => x.Id.Equals(id)).FirstOrDefaultAsync();
    }

    public async Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties)
    {
        return await FindByCondition(x => x.Id.Equals(id), false, includeProperties).FirstOrDefaultAsync();
    }


    #region
    public async Task<IReadOnlyList<TModel>> QueryAsync<TModel>(string sql, object? param, 
        System.Data.CommandType? commandType = System.Data.CommandType.StoredProcedure, System.Data.IDbTransaction? transaction = null, int? commandTimeOut = 30) where TModel : EntityBase<K>
    {
        return (await _context.Connection.QueryAsync<TModel>(sql, param, transaction, commandTimeOut, commandType)).AsList();
    }

    public async Task<TModel> QueryFirstOrDefaultAsync<TModel>(string sql, object? param, System.Data.CommandType? commandType = System.Data.CommandType.StoredProcedure, System.Data.IDbTransaction? transaction = null, int? commandTimeOut = 30) where TModel : EntityBase<K>
    {
        var entity = await _context.Connection.QueryFirstOrDefaultAsync<TModel>(sql, param, transaction, commandTimeOut, commandType);
        if (entity == null)
            throw new EntityNotFoundException();
        return entity;
    }

    public async Task<TModel> QuerySingleAsync<TModel>(string sql, object? param, System.Data.CommandType? commandType = System.Data.CommandType.StoredProcedure, System.Data.IDbTransaction? transaction = null, int? commandTimeOut = 30) where TModel : EntityBase<K>
    {
        return (await _context.Connection.QuerySingleAsync<TModel>(sql, param, transaction, commandTimeOut, commandType));
    }

    public async Task<int> ExecuteAsync(string sql, object? param,
        CommandType? commandType = CommandType.StoredProcedure, IDbTransaction? transaction = null, int? commandTimeout = 30)
    {
        return await _context.Connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
    }
    #endregion
}