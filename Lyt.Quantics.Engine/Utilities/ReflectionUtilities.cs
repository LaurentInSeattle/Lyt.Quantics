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
}
