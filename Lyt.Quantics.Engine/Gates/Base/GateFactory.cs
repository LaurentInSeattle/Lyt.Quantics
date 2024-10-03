namespace Lyt.Quantics.Engine.Gates.Base;

using static Lyt.Quantics.Engine.Gates.Base.RotationGate;
using static RotationGate; 
public static class GateFactory
{
    static GateFactory()
    {
        static void AddGate(Gate? gate)
        {
            if (gate is not null)
            {
                var gateType = gate.GetType();
                var captionProperty =
                    gateType.GetProperty("CaptionKey", BindingFlags.Public | BindingFlags.Instance);
                if (captionProperty is not null &&
                    captionProperty.GetValue(gate) is string caption)
                {
                    AvailableProducts.Add(caption, gateType);
                }
            }
        }

        try
        {
            var gateTypes = ReflectionUtilities.DerivedFrom<Gate>();
            foreach (var gateType in gateTypes)
            {
                try
                {
                    object? gateObject = Activator.CreateInstance(gateType);
                    if (gateObject is Gate gate)
                    {
                        AddGate(gate);
                    }
                    else
                    {
                        throw new Exception("Failed to instantiate gate: " + gateType.FullName);
                    }
                }
                catch (MissingMethodException ex)
                {
                    Debug.WriteLine("First Chance Exception");
                    Debug.WriteLine(ex);
                    Debug.WriteLine(gateType.Name + " does not have a default parameterless constructor.");
                    continue;
                }
            }

            // Add all three Pi / 2 rotation gates 
            foreach (Axis axis in new Axis[] { Axis.X, Axis.Y, Axis.Z })
            {
                var gate = new RotationGate(axis, Math.PI / 2.0);
                AddGate(gate);
            }
        }
        catch ( Exception ex)
        {
            Debug.WriteLine("First Chance Exception");
            Debug.WriteLine(ex);
        }
    }

#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static Dictionary<string, Type> AvailableProducts = new(32);
#pragma warning restore CA2211 

    public static Gate Produce(string caption)
    {
        if (AvailableProducts.TryGetValue(caption, out Type? gateType) && gateType is not null)
        {
            try
            {
                object? instance = Activator.CreateInstance(gateType);
                if (instance is Gate gate)
                {
                    return gate;
                }
            }
            catch (MissingMethodException)
            {
                if (gateType.FullName == typeof(RotationGate).FullName)
                {
                    switch (caption)
                    {
                        case "Rx": return new RotationGate(Axis.X, Math.PI / 2.0);
                        case "Ry": return new RotationGate(Axis.Y, Math.PI / 2.0);
                        case "Rz": return new RotationGate(Axis.Z, Math.PI / 2.0);
                        default:
                            throw new NotSupportedException("Unsupported gate type: " + gateType.FullName);
                    }

                }

                throw new NotSupportedException("Unsupported gate type: " + gateType.FullName); 
            }
        }

        throw new Exception("No such gate type: " + caption);
    }
}
