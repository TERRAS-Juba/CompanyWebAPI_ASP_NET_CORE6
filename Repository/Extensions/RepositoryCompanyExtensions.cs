using CompanyEmployees.Extensions.Utilities;
using Entities.Models;
using Entities.RequestFeatures;
using System.Linq.Dynamic.Core;
namespace CompanyEmployees.Extensions;

public static class RepositoryCompanyExtensions
{
    public static IQueryable<Company> FilterEmployees(this IQueryable<Company>
        companies, CompanyParameters companyParameters) =>
        companies.Where(e =>
            (companyParameters.Country != null ? companyParameters.Country == e.Country : true));

    public static IQueryable<Company> Search(this IQueryable<Company> companies,
        string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return companies;
        var lowerCaseTerm = searchTerm.Trim().ToLower();
        return companies.Where(e =>
            e.Country.ToLower().Contains(lowerCaseTerm) || e.Name.ToLower().Contains(lowerCaseTerm)
                                                        || e.Address.ToLower().Contains(lowerCaseTerm));
    }

    public static IQueryable<Company> Sort(this IQueryable<Company> companies, string orderByQueryString)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
        {
            return companies.OrderBy(e => e.Name);
        }

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Company>(orderByQueryString);
        if (string.IsNullOrWhiteSpace(orderQuery))
            return companies.OrderBy(e => e.Name);
        return companies.OrderBy(orderQuery);
    }
}