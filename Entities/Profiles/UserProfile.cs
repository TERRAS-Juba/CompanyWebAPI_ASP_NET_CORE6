using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

namespace Entities.Profiles;

public class UserProfile:Profile
{
    public UserProfile()
    {
        CreateMap<UserForCreationDto, User>();
    }
}