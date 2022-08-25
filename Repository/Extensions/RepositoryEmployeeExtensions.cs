using System.Reflection;
using System.Text;
using Entities.Models;
using Entities.RequestFeatures;
using System.Linq.Dynamic.Core;
using CompanyEmployees.Extensions.Utilities;

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

    public static IQueryable<Employee> Sort(this IQueryable<Employee> employees, string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
        {
            return employees.OrderBy(e => e.Name);
        }

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Employee>(orderByQueryString);
        if (string.IsNullOrWhiteSpace(orderQuery))
            return employees.OrderBy(e => e.Name);
        return employees.OrderBy(orderQuery);
    }
}