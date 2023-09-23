using TeduMicroservice.IDP.Infrastructure.Common.Domains;
using TeduMicroservice.IDP.Infrastructure.Entities;
using TeduMicroservice.IDP.Infrastructure.ViewModels;

namespace TeduMicroservice.IDP.Infrastructure.Common.Repositories;

public interface IPermissionRepository : IRepositoryBase<Permission, int>
{
    Task<IReadOnlyList<PermissionViewModel>> GetPermissionByRole(string roleId);

    Task<PermissionViewModel?> CreatePermission(string roleId, PermissionAddModel model); 

    Task UpdatePermission(string roleId, IEnumerable<PermissionAddModel> permisstionCollection);

    Task DeletePermission(string roleId, string function, string command);

    Task<IEnumerable<PermissionUserViewModel>> GetPermissionByUser(User user);
}
