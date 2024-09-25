namespace Lyt.Quantics.Engine.Gates;

public static class GateFactory
{
    static GateFactory()
    {
        var gateTypes = ReflectionUtilities.DerivedFrom<Gate>();
        foreach (var gateType in gateTypes)
        {
            object? gate; 
            try
            {
                gate = Activator.CreateInstance(gateType);
            } 
            catch (MissingMethodException ex) 
            {
                Debug.WriteLine(ex);
                Debug.WriteLine(gateType.Name + " does not have a default constructor.");
                continue;
            }

            if (gate is not null)
            {
                var captionProperty =
                    gateType.GetProperty("CaptionKey", BindingFlags.Public | BindingFlags.Instance);
                if ((captionProperty is not null) &&
                    (captionProperty.GetValue(gate) is string caption))
                {
                    GateFactory.AvailableProducts.Add(caption, gateType);
                }
            }
        }
    }

    public static Dictionary<string, Type> AvailableProducts = new(32);

    public static Gate Produce(string caption)
    {
        if ((AvailableProducts.TryGetValue(caption, out Type? gateType)) && (gateType is not null))
        {
            object? instance = Activator.CreateInstance(gateType);
            if (instance is Gate gate)
            {
                return gate;
            }
        }

        throw new Exception("No such gate type: " + caption);
    }
}
