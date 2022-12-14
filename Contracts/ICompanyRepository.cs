using System.Dynamic;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;

namespace Contracts;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> GetAllCompanies(CompanyParameters companyParameters, bool trackChanges);
    Task<Company> GetCompany(Guid companyId, bool trackChanges);
    Task<IEnumerable<CompanyJoinEmployeeDto>> GetCompaniesWithEmployees();
    void CreateCompany(Company company);
    Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
    void DeleteCompany(Company company);
}