using System.ComponentModel.DataAnnotations;
using Entities.Models;

namespace Entities.DataTransferObjects;

public class UserForManipulationDto
{
    [Required(ErrorMessage = "FirstName is required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the FirstName is 60 characters.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "LastName is required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the LastName is 60 characters.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "UserName is required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the UserName is 60 characters.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is required field.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Email is required field.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "PhoneNumber is required field.")]
    public string PhoneNumber { get; set; }
    [Required(ErrorMessage = "Sex is required field.")]
    public char Sex { get; set; }

    public ICollection<string> Roles { get; set; }
}