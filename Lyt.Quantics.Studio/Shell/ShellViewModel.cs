﻿namespace Lyt.Quantics.Studio.Shell;

using static Lyt.Quantics.Studio.Messaging.ViewActivationMessage;

public sealed class ShellViewModel : Bindable<ShellView>
{
    private readonly IToaster toaster;

    public ShellViewModel(IToaster toaster)
    {
        this.toaster = toaster;
        this.Messenger.Subscribe<ViewActivationMessage>(this.OnViewActivation);
        this.Messenger.Subscribe<ShowTitleBarMessage>(this.OnShowTitleBar);
    }

    protected override void OnViewLoaded()
    {
        this.Logger.Debug("OnViewLoaded begins");

        base.OnViewLoaded();
        if (this.View is null)
        {
            throw new Exception("Failed to startup...");
        }

        this.Logger.Debug("OnViewLoaded language loaded");

        // Create all statics views and bind them 
        ShellViewModel.SetupWorkflow();
        this.Logger.Debug("OnViewLoaded SetupWorkflow complete");

        this.Logger.Debug("OnViewLoaded BindGroupIcons complete");

        this.OnViewActivation(ActivatedView.Intro, parameter: null, isFirstActivation: true);
        this.Logger.Debug("OnViewLoaded OnViewActivation complete");

        // Ready 
        this.OnShowTitleBar(new ShowTitleBarMessage());
        this.toaster.Host = this.View.ToasterHost;
        if (this.toaster.View is Control control)
        {
            // Set the location of the toasts at top left at startup, will change later 
            control.HorizontalAlignment = HorizontalAlignment.Left;
            control.VerticalAlignment = VerticalAlignment.Top;
        }

        this.toaster.Show(
            "Welcome to Quantics Studio!",
            "An interactive playground for Quantum Computing...",
            4_000, InformationLevel.Info);
        this.Logger.Debug("OnViewLoaded complete");
    }

    private void OnShowTitleBar(ShowTitleBarMessage message)
    {
        this.TitleBarHeight = new GridLength(message.Show ? 32.0 : 0.0);
        this.IsTitleBarVisible = message.Show;
    } 

    private void OnModelUpdated(ModelUpdateMessage message)
    {
        string msgProp = string.IsNullOrWhiteSpace(message.PropertyName) ? "<unknown>" : message.PropertyName;
        string msgMethod = string.IsNullOrWhiteSpace(message.MethodName) ? "<unknown>" : message.MethodName;
        this.Logger.Debug("Model update, property: " + msgProp + " method: " + msgMethod);

        //if (message.PropertyName != nameof( < some model property > ))
        //{
        //}
    }

    private void OnViewActivation(ViewActivationMessage message)
        => this.OnViewActivation(message.View, message.ActivationParameter, false);

    private void OnViewActivation(ActivatedView activatedView, object? parameter = null, bool isFirstActivation = false)
    {
        if (activatedView == ActivatedView.Exit)
        {
            this.OnExit(null);
        }

        if (activatedView == ActivatedView.GoBack)
        {
            // We always go back to the Intro View 
            activatedView = ActivatedView.Intro;
        }

        switch (activatedView)
        {
            default:
            case ActivatedView.Intro:
                this.Activate<IntroViewModel, IntroView>(isFirstActivation, null);
                break;

            case ActivatedView.Load:
                this.Activate<LoadViewModel, LoadView>(isFirstActivation, null);
                break;

            case ActivatedView.Run:
                this.Activate<RunViewModel, RunView>(isFirstActivation, parameter);
                break;

            case ActivatedView.Save:
                break;
        }
    }

    private async void OnExit(object? _)
    {
        var application = App.GetRequiredService<IApplicationBase>();
        await application.Shutdown();
    }

    private void Activate<TViewModel, TControl>(bool isFirstActivation, object? activationParameters)
        where TViewModel : Bindable<TControl>
        where TControl : Control, new()
    {
        if (this.View is null)
        {
            throw new Exception("No view: Failed to startup...");
        }

        object? currentView = this.View.ShellViewContent.Content;
        if (currentView is Control control && control.DataContext is Bindable currentViewModel)
        {
            currentViewModel.Deactivate();
        }

        var newViewModel = App.GetRequiredService<TViewModel>();
        newViewModel.Activate(activationParameters);
        this.View.ShellViewContent.Content = newViewModel.View;
        if (!isFirstActivation)
        {
            this.Profiler.MemorySnapshot(newViewModel.View.GetType().Name + ":  Activated");
        }
    }

    private static void SetupWorkflow()
    {
        static void CreateAndBind<TViewModel, TControl>()
             where TViewModel : Bindable<TControl>
             where TControl : Control, new()
        {
            var vm = App.GetRequiredService<TViewModel>();
            vm.CreateViewAndBind();
        }

        CreateAndBind<IntroViewModel, IntroView>();
        CreateAndBind<SaveViewModel, SaveView>();
        CreateAndBind<LoadViewModel, LoadView>();
        CreateAndBind<RunViewModel, RunView>();
    }

    public GridLength TitleBarHeight { get => this.Get<GridLength>(); set => this.Set(value); }

    public bool IsTitleBarVisible{ get => this.Get<bool>(); set => this.Set(value); }
}
