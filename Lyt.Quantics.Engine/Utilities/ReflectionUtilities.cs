namespace Lyt.Quantics.Engine.Utilities; 

public static class ReflectionUtilities
{
    public static List<Type> DerivedFrom<TType>  ( ) where TType : class
    {
        var allTypes = Assembly.GetExecutingAssembly ().GetTypes();
        var types =
            (from t in allTypes
             where t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(TType))
             select t); 
        return types.ToList();
    }

    public static T CreateAndCopyPropertiesFrom<T>(T source) where T : class, new()
    {
        T clone = new T();
        var allProperties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        List<PropertyInfo> copyProperties = new(allProperties.Length);
        for (int i = 0; i < allProperties.Length; ++i)
        {
            var property = allProperties[i];
            object[] attributes = property.GetCustomAttributes(typeof(JsonRequiredAttribute), true);
            if (attributes.Length > 0)
            {
                copyProperties.Add(property);
            }
        }

        foreach (PropertyInfo property in copyProperties)
        {
            object? value = property.GetValue(source, null);
            property.SetValue(clone, value, null);
        }

        return clone;
    }
}
