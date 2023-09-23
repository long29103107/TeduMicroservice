using TeduMicroservice.IDP.Infrastructure.Common.Domains;

namespace TeduMicroservice.IDP.Infrastructure.ViewModels;
public class PermissionViewModel : EntityBase<int>
{
    public string Function { get; set; }
    public string RoleId { get; set; }
    public string Command { get; set; }
}
