﻿using TeduMicroservice.IDP.Persistence;

namespace TeduMicroservice.IDP.Infrastructure.Common.Domains;

public class UnitOfWork : IUnitOfWork
{
    private readonly TeduIdentityContext _context;

    public UnitOfWork(TeduIdentityContext context)
    {
        _context = context;
    }

    public Task<int> CommitAsync()
    {
        return _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
