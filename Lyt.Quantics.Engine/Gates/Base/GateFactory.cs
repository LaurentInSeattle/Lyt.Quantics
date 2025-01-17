namespace Lyt.Quantics.Engine.Gates.Base;

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

            GateParameters defaultGateParameters = new();
            // Add all three rotation gates with a Pi / 2 angle 
            foreach (Axis axis in new Axis[] { Axis.X, Axis.Y, Axis.Z })
            {
                defaultGateParameters.Axis = axis;
                AddGate(new RotationGate(defaultGateParameters));
            }

            // Add the phase gate with a Pi / 2 angle 
            AddGate(new PhaseGate(defaultGateParameters));

            // Add a controlled hadamard gate so that its key is in the dictionary 
            AddGate(new ControlledGate(new HadamardGate()));
        }
        catch (Exception ex)
        {
            Debug.WriteLine("First Chance Exception");
            Debug.WriteLine(ex);
        }
    }

#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static Dictionary<string, Type> AvailableProducts = new(32);
#pragma warning restore CA2211

    public static Gate Produce(string caption, GateParameters? gateParameters = null)
    {
        if (string.IsNullOrWhiteSpace(caption))
        {
            throw new ArgumentException("No caption specified");
        }

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
                ArgumentNullException.ThrowIfNull(gateParameters);

                if (gateType == typeof(RotationGate))
                {
                    return new RotationGate(gateParameters);
                }

                if (gateType == typeof(PhaseGate))
                {
                    return new PhaseGate(gateParameters);
                }

                if (gateType == typeof(ControlledGate))
                {
                    var baseGate = Produce(gateParameters.BaseGateKey, gateParameters);
                    return new ControlledGate(baseGate);
                }

                if (gateType == typeof(FlippedControlledGate))
                {
                    var baseGate = Produce(gateParameters.BaseGateKey, gateParameters);
                    return new FlippedControlledGate(baseGate);
                }

                throw new NotSupportedException("Unsupported gate type: " + gateType.FullName);
            }
        }

        throw new Exception("No such gate type: " + caption);
    }
}
