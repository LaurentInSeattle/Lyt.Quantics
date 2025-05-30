﻿namespace Lyt.Quantics.Studio.Shell;

using Lyt.Mvvm;
using static MessagingExtensions;
using static ViewActivationMessage;

public sealed partial class ShellViewModel : ViewModel<ShellView>
{
    private readonly IToaster toaster;
    private readonly IDialogService dialogService;
    private readonly QsModel quanticsStudioModel;

    [ObservableProperty]
    private GridLength titleBarHeight;

    [ObservableProperty]
    private bool isTitleBarVisible;

    public ShellViewModel(
        IDialogService dialogService, IToaster toaster, QsModel quanticsStudioModel)
    {
        this.toaster = toaster;
        this.dialogService = dialogService;
        this.quanticsStudioModel = quanticsStudioModel;
        this.Messenger.Subscribe<ViewActivationMessage>(this.OnViewActivation);
        this.Messenger.Subscribe<ShowTitleBarMessage>(this.OnShowTitleBar);
    }

    public override void OnViewLoaded()
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

    /// <summary> Invoked when closing from the application Close X button </summary>
    /// <returns> True to close immediately </returns>
    public bool CanClose()
    {
        var keyboard = App.GetRequiredService<Keyboard>();
        if (keyboard.Modifiers.HasFlag(KeyModifiers.Shift))
        {
            // Do not check for dirtiness when "shifting" 
            return true;
        }

        if (this.quanticsStudioModel.IsDirty)
        {
            if (this.dialogService.IsModal)
            {
                this.dialogService.Dismiss();
            }

            Schedule.OnUiThread(50,
                () =>
                {
                    var confirmActionParameters = new ConfirmActionParameters
                    {
                        Title = "Unsaved Changes!",
                        Message = "This quantum computer has been modified and your latest changes have not been saved.",
                        ActionVerb = "Discard Changes",
                        OnConfirm = this.OnDiscardChangesConfirmed,
                        InformationLevel = InformationLevel.Warning,
                    };

                    this.dialogService.Confirm(this.View.ToasterHost, confirmActionParameters);
                }, DispatcherPriority.Normal);
            return false;
        }

        return true;
    }

    private void OnDiscardChangesConfirmed(bool confirmed)
    {
        if (!confirmed)
        {
            // Run modal dialog to save computer model - no parameters, no closing action  
            if (this.dialogService is DialogService modalService)
            {
                modalService.Dismiss();
                modalService.RunViewModelModal(this.View.ToasterHost, new SaveDialogModel());
            }

            return;
        }

        // changes will be lost
        ActivateView(ActivatedView.Exit);
    }

    private void OnShowTitleBar(ShowTitleBarMessage message)
    {
        this.TitleBarHeight = new GridLength(message.Show ? 42.0 : 0.0);
        this.IsTitleBarVisible = message.Show;
    }

    private void OnViewActivation(ViewActivationMessage message)
        => this.OnViewActivation(message.View, message.ActivationParameter, false);

    private void OnViewActivation(ActivatedView activatedView, object? parameter = null, bool isFirstActivation = false)
    {
        if (activatedView == ActivatedView.Exit)
        {
            ShellViewModel.OnExit();
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
        }
    }

    private async static void OnExit()
    {
        var application = App.GetRequiredService<IApplicationBase>();
        await application.Shutdown();
    }

    private void Activate<TViewModel, TControl>(bool isFirstActivation, object? activationParameters)
        where TViewModel : ViewModel<TControl>
        where TControl : Control, IView , new()
    {
        if (this.View is null)
        {
            throw new Exception("No view: Failed to startup...");
        }

        object? currentView = this.View.ShellViewContent.Content;
        if (currentView is Control control && 
            control.DataContext is ViewModel currentViewModel)
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
        else
        {
            this.LoadSwapData(); 
        }
    }

    private static void SetupWorkflow()
    {
        App.GetRequiredService<IntroViewModel>().CreateViewAndBind();
        App.GetRequiredService<LoadViewModel>().CreateViewAndBind();
        App.GetRequiredService<RunViewModel>().CreateViewAndBind();
    }

    private void LoadSwapData()
    {
        Task.Run(() => 
        {
            try
            {
                SwapData.Poke();
                Dispatch.OnUiThread(() =>
                {
                    this.Profiler.MemorySnapshot("Swap Data Loaded");
                } ) ;
            }
            catch (Exception ex)
            {
                this.Logger.Fatal("Failed to load swap data.\n" + ex.ToString());
            }
        }); 
    }
}
