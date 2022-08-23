using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Company
{
    [Column("CompanyId")] public Guid Id { get; set; }

    [Required(ErrorMessage = "Company name is required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the name is 60 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Company adress is required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the adress is 60 characters.")]
    public string Address { get; set; }

    public string Country { get; set; }
    private ICollection<Employee> Employees { get; set; }
}