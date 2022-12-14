using System.Dynamic;
using System.Linq.Dynamic.Core;
using CompanyEmployees.Extensions;
using Contracts;
using Entities;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
    public CompanyRepository(RepositoryContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Company>> GetAllCompanies(CompanyParameters companyParameters, bool trackChanges) =>
        await FindAll(trackChanges)
            .FilterEmployees(companyParameters)
            .Sort(companyParameters.OrderBy)
            .Search(companyParameters.SearchTerm)
            .Skip((companyParameters.pageNumber - 1) * companyParameters.PageSize)
            .Take(companyParameters.PageSize)
            .ToListAsync();

    public async Task<Company> GetCompany(Guid companyId, bool trackChanges)
        => await FindByCondition(c => c.Id.Equals(companyId), trackChanges).SingleOrDefaultAsync();

    public async Task<IEnumerable<CompanyJoinEmployeeDto>> GetCompaniesWithEmployees()
        => await _context.Companies.Join(_context.Employees, c => c.Id, e => e.CompanyId, (company, employee) => new
            CompanyJoinEmployeeDto
            {
                CompanyId = company.Id,
                CompanyName = company.Address,
                CompanyAddress = company.Address,
                CompanyCountry = company.Country,
                EmployeeId = employee.Id,
                EmployeeName = employee.Name,
                EmployeeAge = employee.Age,
                EmployeePosition = employee.Position
            }).ToListAsync();

    public void CreateCompany(Company company)
        => Create(company);

    public async Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
        => await FindByCondition(x => ids.Contains(x.Id), trackChanges).ToListAsync();

    public void DeleteCompany(Company company)
    {
        Delete(company);
    }
}