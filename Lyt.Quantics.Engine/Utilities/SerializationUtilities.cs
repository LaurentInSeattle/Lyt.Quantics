namespace Lyt.Quantics.Engine.Utilities;

public static class SerializationUtilities
{
    private static readonly JsonSerializerOptions jsonSerializerOptions;

    static SerializationUtilities()
    {
        jsonSerializerOptions =
            new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                AllowOutOfOrderMetadataProperties = true,
                WriteIndented = true,
                IndentSize = 4,
                ReadCommentHandling = JsonCommentHandling.Skip,
                RespectRequiredConstructorParameters = true,
                RespectNullableAnnotations= true,
            };
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public static string? GetFullResourceName(string name, Assembly assembly)
        => assembly.GetManifestResourceNames().Single(str => str.EndsWith(name));

    public static string LoadEmbeddedTextResource(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();
        string? resourceName = SerializationUtilities.GetFullResourceName(name, assembly);
        if (!string.IsNullOrEmpty(resourceName))
        {
            var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is not null)
            {
                using (stream)
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        throw new Exception("Failed to load resource: " + name);
    }

    public static string Serialize<T>(T binaryObject) where T : class
    {
        try
        {
            string serialized = JsonSerializer.Serialize(binaryObject, jsonSerializerOptions);
            if (!string.IsNullOrWhiteSpace(serialized))
            {
                return serialized;
            }

            throw new Exception("Serialized as null or empty string.");
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
            object? deserialized = JsonSerializer.Deserialize<T>(serialized, jsonSerializerOptions);
            if (deserialized is T binaryObject)
            {
                return binaryObject;
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
