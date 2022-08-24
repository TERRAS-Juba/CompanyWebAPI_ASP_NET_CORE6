using Contracts;
using Entities;
using Entities.Models;

namespace Repository;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext context) : base(context)
    {
    }

    public IEnumerable<Employee> GetAllEmployees(Guid companyId, bool trackChanges)
        => FindByCondition(c => c.CompanyId.Equals(companyId), trackChanges).OrderBy(e => e.Id);

    public Employee GetEmployee(Guid companyId, Guid employeeId, bool trackChanges)
        => FindByCondition(c => c.CompanyId.Equals(companyId) && c.Id == employeeId, trackChanges).SingleOrDefault();

    public void CreateEmployee(Guid companyId, Employee employee)
    {
        employee.CompanyId = companyId;
        Create(employee);
    }

    public void DeleteEmployee(Employee employee)
    {
        Delete(employee);
    }
}