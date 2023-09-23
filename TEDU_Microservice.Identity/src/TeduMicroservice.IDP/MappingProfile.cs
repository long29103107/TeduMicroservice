using AutoMapper;
using TeduMicroservice.IDP.Infrastructure.Entities;
using TeduMicroservice.IDP.Infrastructure.ViewModels;

namespace TeduMicroservice.IDP;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Permission, PermissionUserViewModel>();
    }
}
