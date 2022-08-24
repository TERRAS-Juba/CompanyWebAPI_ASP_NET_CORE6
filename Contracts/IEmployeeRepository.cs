using Entities.Models;

namespace Contracts;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllEmployees(Guid companyId, bool trackChanges);
    Task<Employee> GetEmployee(Guid companyId, Guid employeeId, bool trackChanges);
    void CreateEmployee(Guid companyId, Employee employee);
    void DeleteEmployee(Employee employee);
}