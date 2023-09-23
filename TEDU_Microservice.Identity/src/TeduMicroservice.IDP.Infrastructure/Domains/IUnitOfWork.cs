namespace TeduMicroservice.IDP.Infrastructure.Common.Domains;

public interface IUnitOfWork : IDisposable
{
    Task<int> CommitAsync();
}
