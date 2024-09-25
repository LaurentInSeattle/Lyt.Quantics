namespace Lyt.Quantics.Engine.Utilities;

public static class SerializationUtilities
{
    private static readonly JsonSerializerOptions jsonSerializerOptions;

    static SerializationUtilities()
    {
        jsonSerializerOptions = new JsonSerializerOptions { AllowTrailingCommas = true };
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public static string? GetFullResourceName(string name, Assembly assembly)
        => assembly.GetManifestResourceNames().Single(str => str.EndsWith(name));
    
    public static string LoadEmbeddedTextResource(string name)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string? resourceName = GetFullResourceName (name, assembly);
        if (!string.IsNullOrEmpty(resourceName))
        {
            var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is not null)
            {
                using (stream)
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        } 

        throw new Exception("Failed to load resource: " + name );
    }

    public static string Serialize<T>(T deserialized) where T : class
    {
        try
        {
            string serialized = JsonSerializer.Serialize(deserialized, typeof(T), jsonSerializerOptions);
            if (!string.IsNullOrWhiteSpace(serialized))
            {
                return serialized;
            }

            throw new Exception();
        }
        catch (Exception ex)
        {
            string msg = "Failed to serialize " + typeof(T).FullName + "\n" + ex.ToString();
            throw new Exception(msg, ex);
        }
    }

    public static T Deserialize<T>(string serialized) where T : class
    {
        try
        {
            object? deserialized = JsonSerializer.Deserialize(serialized, typeof(T), jsonSerializerOptions);
            if (deserialized is T deserializedOfT)
            {
                return deserializedOfT;
            }

            throw new Exception();
        }
        catch (Exception ex)
        {
            string msg = "Failed to deserialize " + typeof(T).FullName + "\n" + ex.ToString();
            throw new Exception(msg, ex);
        }
    }
}
