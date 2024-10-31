namespace Lyt.Quantics.Engine.Utilities;

public static class ReflectionUtilities
{
    public static List<Type> DerivedFrom<TType>() where TType : class
        => (from t in Assembly.GetExecutingAssembly().GetTypes()
             where t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(TType))
             select t).ToList();

    public static T CreateAndCopyPropertiesFrom<T>(T source) where T : class, new()
    {
        T clone = new();
        var allProperties =
            source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (PropertyInfo property in allProperties)
        {
            // Need Both Read and write 
            if (!(property.CanRead || property.CanWrite))
            {
                continue;
            }

            // Need Value or string 
            var type = property.PropertyType;
            bool isString = type == typeof(string);
            bool isValue = type.IsValueType;
            if (!(isString || isValue))
            {
                continue;
            }

            object? value = property.GetValue(source, null);
            property.SetValue(clone, value, null);
        }

        return clone;
    }
}
