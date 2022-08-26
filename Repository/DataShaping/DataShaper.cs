using System.Dynamic;
using System.Reflection;
using Contracts;

namespace Repository.DataShaping;

public class DataShaper<T>:IDataShaper<T> where T:class
{
    public PropertyInfo [] Properties { get; set; }

    public DataShaper()
    {
        Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }

    public IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string fieldsString)
    {
        return FetchData(entities, GetRequiredProperties(fieldsString));
    }

    public ExpandoObject ShapeData(T entity, string fieldsString)
    {
        return FetchDataForEntity(entity, GetRequiredProperties(fieldsString));
    }

    private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
    {
        var requiredProperties = new List<PropertyInfo>();
        if (string.IsNullOrWhiteSpace(fieldsString))
        {
            return Properties.ToList();
        }
        else
        {
            var fields = fieldsString.Split(",", StringSplitOptions.RemoveEmptyEntries);
            foreach (var field in fields)
            {
                var property = Properties.FirstOrDefault(p =>
                    p.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));
                if (property == null)
                {
                    continue;
                }
                else
                {
                    requiredProperties.Add(property);
                }
            }
        }

        return requiredProperties;
    }

    private IEnumerable<ExpandoObject> FetchData(IEnumerable<T> entities, IEnumerable<PropertyInfo> requiredProperties)
    {
        var shapeData = new List<ExpandoObject>();
        foreach (var entity in entities)
        {
            var shapedObject = FetchDataForEntity(entity,requiredProperties);
            shapeData.Add(shapedObject);
        }
        return shapeData;
    }

    private ExpandoObject FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
    {
        var shapedObject = new ExpandoObject();
        foreach (var requiredProperty in requiredProperties)
        {
            var ObjectValueProperty = requiredProperty.GetValue(entity);
            shapedObject.TryAdd(requiredProperty.Name, ObjectValueProperty);
        }

        return shapedObject;
    }
}