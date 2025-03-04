namespace Lyt.Quantics.Studio
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called:
        // things aren't initialized yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) 
            => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()                
                .LogToTrace();

        //  Win32PlatformOption could accelerate rendering when doing drag and drop...
        //  But sadly, no gain on "my machine"... 
        //
        //  public static AppBuilder BuildAvaloniaApp()
        //      => AppBuilder.Configure<App>()
        //      .UsePlatformDetect()
        //      .WithInterFont()
        //      .With(new Win32PlatformOptions { CompositionMode = [Win32CompositionMode.LowLatencyDxgiSwapChain] })
        //      .LogToTrace();
    }
}
