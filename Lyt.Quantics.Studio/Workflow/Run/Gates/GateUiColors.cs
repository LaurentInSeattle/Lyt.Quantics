namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

using static Lyt.Avalonia.Controls.Utilities;

public static class GateUiColors
{
    public const string BlueBrushName = "LightAqua_0_100";
    public const string OrangeBrushName = "OrangePeel_2_100";

    public static readonly SolidColorBrush BlueBrush;
    public static readonly SolidColorBrush OrangeBrush;
    public static readonly SolidColorBrush BackgroundBrush;
    public static readonly SolidColorBrush TransparentBrush;

    static GateUiColors()
    {
        BackgroundBrush = new SolidColorBrush(color: 0x30406080);
        TransparentBrush = new SolidColorBrush(color: 0);
        TryFindResource(BlueBrushName, out SolidColorBrush? maybeBlueBrush);
        if (maybeBlueBrush is null)
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            throw new ArgumentNullException("Could not find resource: " + BlueBrushName);
#pragma warning restore CA2208 
        }
        else
        {
            BlueBrush = maybeBlueBrush;
        }

        TryFindResource(OrangeBrushName, out SolidColorBrush? maybeOrangeBrush);
        if (maybeOrangeBrush is null)
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            throw new ArgumentNullException("Could not find resource: " + OrangeBrushName);
#pragma warning restore CA2208 
        }
        else
        {
            OrangeBrush = maybeOrangeBrush;
        }
    }

    public static IBrush GateCategoryToBrush(GateCategory gateCategory)
        => gateCategory switch
        {
            GateCategory.HadamardAndT => Brushes.DarkOrange,
            GateCategory.Pauli => Brushes.DodgerBlue,
            GateCategory.Phase => Brushes.MediumAquamarine,
            GateCategory.Rotation => Brushes.DarkOrchid,
            GateCategory.BinaryControlled => Brushes.DarkGreen,
            GateCategory.Other => Brushes.DarkGray,
            GateCategory.TernaryControlled => Brushes.MediumPurple,
            /* default */
            _ => Brushes.DarkRed,
        };

}
