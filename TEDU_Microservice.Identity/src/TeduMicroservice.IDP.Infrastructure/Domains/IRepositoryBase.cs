using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Linq.Expressions;

namespace TeduMicroservice.IDP.Infrastructure.Common.Domains;

public interface IRepositoryBase<T, K> where T : EntityBase<K>
{
    #region Query
    IQueryable<T> FindAll(bool trackChanges = false);
    IQueryable<T> FindAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false);
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges = false,
        params Expression<Func<T, object>>[] includeProperties);

    Task<T?> GetByIdAsync(K id);
    Task<T?> GetByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties);
    #endregion

    #region Action
    Task<K> CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task UpdateListAsync(IEnumerable<T> entities);
    Task DeleteAsync(T entity);
    Task DeleteListAsync(IEnumerable<T> entities);
    #endregion

    Task<int> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task EndTransactionAsync();
    Task RollbackTransactionAsync();

    #region Dapper
    Task<IReadOnlyList<TModel>> QueryAsync<TModel>(string sql, object? param,
        CommandType? commandType, IDbTransaction? transaction, int? commandTimeOut) where TModel : EntityBase<K>;
    Task<TModel> QueryFirstOrDefaultAsync<TModel>(string sql, object? param,
       CommandType? commandType, IDbTransaction? transaction, int? commandTimeOut) where TModel : EntityBase<K>;
    Task<TModel> QuerySingleAsync<TModel>(string sql, object? param,
       CommandType? commandType, IDbTransaction? transaction, int? commandTimeOut) where TModel : EntityBase<K>;
    Task<int> ExecuteAsync(string sql, object? param,
        CommandType? commandType, IDbTransaction? transaction, int? commandTimeOut);
    #endregion 
}
