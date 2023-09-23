namespace TeduMicroservice.IDP.Infrastructure.Common.Domains;

public interface IEntityBase<T>
{
    T Id { get; set; }  
}
