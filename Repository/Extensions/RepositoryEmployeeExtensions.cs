using Entities.Models;
using Entities.RequestFeatures;

namespace CompanyEmployees.Extensions;

public static class RepositoryEmployeeExtensions
{
    public static IQueryable<Employee> FilterEmployees(this IQueryable<Employee>
        employees, EmployeeParameters employeeParameters) =>
        employees.Where(e =>
            e.Age >= employeeParameters.MinAge && e.Age <= employeeParameters.MaxAge &&
            (employeeParameters.Position != null ? employeeParameters.Position == e.Position : true));

    public static IQueryable<Employee> Search(this IQueryable<Employee> employees,
        string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return employees;
        var lowerCaseTerm = searchTerm.Trim().ToLower();
        return employees.Where(e =>
            e.Name.ToLower().Contains(lowerCaseTerm) || e.Position.ToLower().Contains(lowerCaseTerm));
    }
}