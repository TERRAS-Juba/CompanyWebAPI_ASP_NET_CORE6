namespace Entities.DataTransferObjects;

public class CompanyForUpdateDto
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string Country { get; set; }
    public ICollection<EmployeeForCreationDto> Employees { get; set; }
}