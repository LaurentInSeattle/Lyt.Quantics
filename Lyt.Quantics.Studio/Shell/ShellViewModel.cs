namespace Lyt.Quantics.Studio.Shell;

using static MessagingExtensions;

public sealed partial class ShellViewModel : ViewModel<ShellView>
{
    private readonly IToaster toaster;
    private readonly IDialogService dialogService;
    private readonly QsModel quanticsStudioModel;

    private ViewSelector<ActivatedView>? viewSelector;
    public bool isFirstActivation;

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
        this.SetupWorkflow();
        this.Logger.Debug("OnViewLoaded SetupWorkflow complete");

        this.isFirstActivation = true;
        Select(ActivatedView.Intro);

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

    public async static void OnExit()
    {
        var application = App.GetRequiredService<IApplicationBase>();
        await application.Shutdown();
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
        OnExit();
    }

    private void OnShowTitleBar(ShowTitleBarMessage message)
    {
        this.TitleBarHeight = new GridLength(message.Show ? 42.0 : 0.0);
        this.IsTitleBarVisible = message.Show;
    }

    private void SetupWorkflow()
    {
        if (this.View is not ShellView view)
        {
            throw new Exception("No view: Failed to startup...");
        }

        var selectableViews = new List<SelectableView<ActivatedView>>();

        void SetupNoToolbar<TViewModel, TControl>(
                ActivatedView activatedView, Control? control=null)
            where TViewModel : ViewModel<TControl>
            where TControl : Control, IView, new()
        {
            var vm = App.GetRequiredService<TViewModel>();
            vm.CreateViewAndBind();
            selectableViews.Add(
                new SelectableView<ActivatedView>(activatedView, vm, control, null));
        }

        SetupNoToolbar<IntroViewModel, IntroView>(ActivatedView.Intro);
        SetupNoToolbar<LoadViewModel, LoadView>(ActivatedView.Load);
        SetupNoToolbar<RunViewModel, RunView>(ActivatedView.Run);
        
        // Needs to be kept alive as a class member, or else callbacks will die (and wont work) 
        this.viewSelector =
            new ViewSelector<ActivatedView>(
                this.Messenger,
                this.View.ShellViewContent,
                null, null,
                selectableViews, this.OnViewSelected);
    }

    private void OnViewSelected(ActivatedView view)
    {
        if (this.isFirstActivation)
        {
            this.LoadSwapData();
        }
        else
        {
            this.Profiler.MemorySnapshot(this.ViewBase!.GetType().Name + ":  Activated");
        }

        this.isFirstActivation = false;
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
