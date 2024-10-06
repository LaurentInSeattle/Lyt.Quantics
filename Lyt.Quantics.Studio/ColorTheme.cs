namespace Lyt.Quantics.Studio;

public static class ColorTheme
{
    public enum Style
    {
        Default,
        Translucent,
    }

    public static Brush LeftBackground { get; private set; }
    public static Brush LeftForeground { get; private set; }
    public static Brush RightBackground { get; private set; }
    public static Brush RightForeground { get; private set; }

    public static Brush Background { get; private set; }
    public static Brush UiText { get; private set; }
    public static Brush ValidUiText { get; private set; }
    public static Brush Text { get; private set; }
    public static Brush TextAbsent { get; private set; }
    public static Brush BoxBorder { get; private set; }
    public static Brush BoxUnknown { get; private set; }
    public static Brush BoxAbsent { get; private set; }
    public static Brush BoxPresent { get; private set; }
    public static Brush BoxExact { get; private set; }

    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    static ColorTheme() => ColorTheme.Set(Style.Translucent);
    #pragma warning restore CS8618 

    public static void Set(Style style)
    {
        // MUST use SolidColorBrush for all themes
        switch (style)
        {
            default:
            case Style.Default:
                Background = new SolidColorBrush(Colors.Black);
                UiText = new SolidColorBrush(Colors.DarkOrange);
                Text = new SolidColorBrush(Colors.LavenderBlush);
                TextAbsent = new SolidColorBrush(Colors.SlateGray);
                BoxBorder = new SolidColorBrush(Colors.Lavender);
                BoxUnknown = new SolidColorBrush(Colors.DarkSlateBlue);
                BoxPresent = new SolidColorBrush(Colors.DarkOrange);
                BoxAbsent = new SolidColorBrush(Colors.DarkSlateGray);
                BoxExact = new SolidColorBrush(Colors.MediumSeaGreen);
                ValidUiText = new SolidColorBrush(Colors.MediumSeaGreen);

                LeftBackground = new SolidColorBrush(Colors.DodgerBlue);
                LeftForeground = new SolidColorBrush(Colors.DarkBlue);
                RightBackground = new SolidColorBrush(Colors.LightSalmon);
                RightForeground = new SolidColorBrush(Colors.Firebrick);

                break;

            case Style.Translucent:
                Background = new SolidColorBrush(Colors.Black);
                UiText = new SolidColorBrush(Colors.LightCoral);
                Text = new SolidColorBrush(Colors.LavenderBlush);
                TextAbsent = new SolidColorBrush(Colors.SlateGray);
                BoxBorder = new SolidColorBrush(Colors.PowderBlue);
                BoxUnknown = new SolidColorBrush(Color.FromArgb(0x22, 0x48, 0x3D, 0x9B));// Brushes.DarkSlateBlue;
                BoxPresent = new SolidColorBrush(Color.FromArgb(0xD0, 0xFF, 0xB0, 0x10));
                BoxAbsent = new SolidColorBrush(Color.FromArgb(0xC0, 0x20, 0x30, 0x50));// Brushes.DarkSlateGray;
                BoxExact = new SolidColorBrush(Color.FromArgb(0xC0, 0x2F, 0xA0, 0x5F));
                ValidUiText = new SolidColorBrush(Color.FromArgb(0xFF, 0x2F, 0xB0, 0x5F));

                LeftBackground = new SolidColorBrush(Colors.DodgerBlue);
                LeftForeground = new SolidColorBrush(Colors.LightSkyBlue);
                RightBackground = new SolidColorBrush(Colors.LightSalmon);
                RightForeground = new SolidColorBrush(Colors.Firebrick);

                break;
        }
    }
}
