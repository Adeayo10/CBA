using AutoMapper;
using CBA.Models;

namespace CBA.Mapping;
public class AutomapperProfile: Profile
{
    public AutomapperProfile()
    {
        CreateMap<ApplicationUser, UserProfileDTO>();
        CreateMap<UserProfileDTO, ApplicationUser>();
        CreateMap<UserUpdateDTO, ApplicationUser>();
        CreateMap<ApplicationUser, UserUpdateDTO>();
    }
}