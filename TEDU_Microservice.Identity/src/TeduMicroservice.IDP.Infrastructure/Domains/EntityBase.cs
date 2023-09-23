namespace TeduMicroservice.IDP.Infrastructure.Common.Domains;

public abstract class EntityBase<TKey> : IEntityBase<TKey>
{
    public TKey Id { get; set; }
}
