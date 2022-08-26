namespace Entities.DataTransferObjects;

public class CompanyJoinEmployeeDto
{
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; }
    public string CompanyAddress { get; set; }
    public string CompanyCountry { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public int EmployeeAge { get; set; }
    public string EmployeePosition { get; set; }
}