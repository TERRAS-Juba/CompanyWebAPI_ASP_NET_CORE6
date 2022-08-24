using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Employee>> GetAllEmployees(Guid companyId, bool trackChanges)
        => await FindByCondition(c => c.CompanyId.Equals(companyId), trackChanges).OrderBy(e => e.Id).ToListAsync();

    public async Task<Employee> GetEmployee(Guid companyId, Guid employeeId, bool trackChanges)
        => await FindByCondition(c => c.CompanyId.Equals(companyId) && c.Id == employeeId, trackChanges).SingleOrDefaultAsync();

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