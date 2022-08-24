using Contracts;
using Entities;

namespace Repository;

public class RepositoryManager : IRepositoryManager
{
    private RepositoryContext _repositoryContext;
    private ICompanyRepository? _companyRepository { get; set; }
    private IEmployeeRepository? _employeeRepository { get; set; }

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
    }

    public ICompanyRepository Company
    {
        get
        {
            if (_companyRepository == null)
                _companyRepository = new CompanyRepository(_repositoryContext);
            return _companyRepository;
        }
    }

    public IEmployeeRepository Employee
    {
        get
        {
            if (_employeeRepository == null)
                _employeeRepository = new EmployeeRepository(_repositoryContext);
            return _employeeRepository;
        }
    }

    public Task SaveChanges() => _repositoryContext.SaveChangesAsync();
}