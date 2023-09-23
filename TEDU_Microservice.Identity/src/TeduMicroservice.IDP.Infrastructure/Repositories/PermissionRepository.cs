using Dapper;
using Microsoft.AspNetCore.Identity;
using System.Data;
using TeduMicroservice.IDP.Infrastructure.Common.Domains;
using TeduMicroservice.IDP.Infrastructure.Entities;
using TeduMicroservice.IDP.Infrastructure.ViewModels;
using TeduMicroservice.IDP.Persistence;
using DataTable = System.Data.DataTable;
using DbType = System.Data.DbType;
using AutoMapper;

namespace TeduMicroservice.IDP.Infrastructure.Common.Repositories;

public class PermissionRepository : RepositoryBase<Permission, int>, IPermissionRepository
{

    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    public PermissionRepository(TeduIdentityContext context, IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper) : base(context, unitOfWork)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PermissionViewModel>> GetPermissionByRole(string roleId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId);
        var result = await QueryAsync<PermissionViewModel>("Get_Permission_ByRoleId", parameters);
        return result;
    }

    public async Task<PermissionViewModel?> CreatePermission(string roleId, PermissionAddModel model)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@function", model.Function, DbType.String);
        parameters.Add("@command", model.Command, DbType.String);
        parameters.Add("@newId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        var result = await ExecuteAsync("Create_Permission", parameters);
        if (result <= 0) return null;
        var newId = parameters.Get<int>("@newId");
        return new PermissionViewModel
        {
            Id = newId,
            RoleId = roleId,
            Function = model.Function,
            Command = model.Command
        };
    }

    public Task UpdatePermission(string roleId, IEnumerable<PermissionAddModel> permisstionCollection)
    {
        var dt = new DataTable();
        dt.Columns.Add("RoleId", typeof(string));
        dt.Columns.Add("Function", typeof(string));
        dt.Columns.Add("Command", typeof(string));
        foreach( var item in permisstionCollection)
        {
            dt.Rows.Add(roleId, item.Function, item.Command);
        }

        var parameters = new DynamicParameters();

        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@permissions", dt.AsTableValuedParameter("dbo.Permission"));
        return ExecuteAsync("Update_Permissions_ByRole", parameters);
    }

    public Task DeletePermission(string roleId, string function, string command)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@roleId", roleId, DbType.String);
        parameters.Add("@function", function, DbType.String);
        parameters.Add("@command", command, DbType.String);


        return ExecuteAsync("Delete_Permission", parameters);
    }

    public async Task<IEnumerable<PermissionUserViewModel>> GetPermissionByUser(User user)
    {
        //Current user's roles: Admin, Customer, Visitor
        var currentUserRoles = await _userManager.GetRolesAsync(user);
        //load all permissions => 30 permissions
        var query = FindAll()
            .Where(x => currentUserRoles.Contains(x.RoleId))
            .Select(x => new Permission(x.Function, x.Command, x.RoleId))
            .AsEnumerable();

        var result = _mapper.Map<IEnumerable<PermissionUserViewModel>>(query);
        return result;
    }
}
