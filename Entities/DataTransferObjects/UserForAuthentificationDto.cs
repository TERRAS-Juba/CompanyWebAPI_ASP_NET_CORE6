using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects;

public class UserForAuthentificationDto
{
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password name is required")]
    public string Password { get; set; }
}