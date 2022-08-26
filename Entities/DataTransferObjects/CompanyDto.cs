using Entities.Models;

namespace Entities.DataTransferObjects;

public class CompanyDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    public string Address { get; set; }
    public string Country { get; set; }
}