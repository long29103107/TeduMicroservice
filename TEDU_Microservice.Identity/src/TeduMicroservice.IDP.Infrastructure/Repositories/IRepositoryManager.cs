using Microsoft.EntityFrameworkCore.Storage;
using TeduMicroservice.IDP.Infrastructure.Entities;

namespace TeduMicroservice.IDP.Infrastructure.Common.Repositories;

public interface IRepositoryManager
{
    IPermissionRepository Permission { get; }
    Task<int> SaveAsync();
    Task<IDbContextTransaction> BeginTransaction();
    Task EndTransactionAsync();
    void RollbackTransaction();

}
