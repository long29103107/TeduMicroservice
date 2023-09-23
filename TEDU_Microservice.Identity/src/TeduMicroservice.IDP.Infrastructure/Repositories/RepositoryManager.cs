using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using TeduMicroservice.IDP.Infrastructure.Common.Domains;
using TeduMicroservice.IDP.Infrastructure.Entities;
using TeduMicroservice.IDP.Persistence;

namespace TeduMicroservice.IDP.Infrastructure.Common.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly TeduIdentityContext _context;
    private readonly Lazy<IPermissionRepository> _permissionRepository;
    private readonly IMapper _mapper;
    public UserManager<User> UserManager { get; }
    public RoleManager<IdentityRole> RoleManager { get; }
    public IPermissionRepository Permission => _permissionRepository.Value;

    public RepositoryManager(TeduIdentityContext context, IUnitOfWork unitOfWork, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _permissionRepository = new Lazy<IPermissionRepository>(() => new PermissionRepository(_context, _unitOfWork, userManager, mapper));
        UserManager = userManager;
        RoleManager = roleManager;
        _mapper = mapper;
    }
    public Task<IDbContextTransaction> BeginTransaction()
    {
        return _context.Database.BeginTransactionAsync();
    }

    public Task EndTransactionAsync()
    {
        return _context.Database.CommitTransactionAsync();
    }

    public void RollbackTransaction()
    {
        _context.Database.RollbackTransactionAsync();
    }

    public Task<int> SaveAsync()
    {
        return _unitOfWork.CommitAsync();
    }
}
