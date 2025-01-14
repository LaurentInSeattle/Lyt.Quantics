namespace Lyt.Quantics.Engine.Utilities;

public static class ReflectionUtilities
{
    public static List<Type> DerivedFrom<TType>() where TType : class
        => (from t in Assembly.GetExecutingAssembly().GetTypes()
            where t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(TType))
            select t).ToList();

    /// <summary> Creates a partially cloned object.</summary>
    /// <remarks> Copies only public instance RW value or string properties. </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"> Object to clone </param>
    /// <returns> A new partially cloned object of type T </returns>
    public static T CreateAndCopyPropertiesFrom<T>(this T source) where T : class, new()
    {
        if (source is string _)
        {
            throw new InvalidOperationException("Do not use this for strings");
        }

        // CONSIDER 
        //  Call MemberwiseClone through reflection to create the clone
        //  Could have some nasty side effects...
        // 
        //     MethodInfo memberwiseClone
        //          = source.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
        //     T clone = (T)memberwiseClone.Invoke(obj, null);

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
