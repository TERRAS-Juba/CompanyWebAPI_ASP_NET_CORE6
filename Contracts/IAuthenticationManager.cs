using Entities.DataTransferObjects;
using Entities.Models;

namespace Contracts;

public interface IAuthenticationManager
{
    Task<bool> ValidateUser(UserForAuthentificationDto userForAuthentificationDto);
    Task<string> CreateToken();
}