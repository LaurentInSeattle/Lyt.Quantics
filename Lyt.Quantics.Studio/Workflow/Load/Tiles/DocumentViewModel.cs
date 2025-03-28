﻿namespace Lyt.Quantics.Studio.Workflow.Load.Tiles;

using static ViewActivationMessage;
using static ToolbarCommandMessage; 
using static MessagingExtensions;

public sealed class DocumentViewModel : Bindable<DocumentView>
{
    private readonly QuComputer quComputer; 

    public DocumentViewModel(QuComputer quComputer)
    {
        base.DisablePropertyChangedLogging = true;
        base.DisableAutomaticBindingsLogging = true;

        this.quComputer = quComputer;
        this.Name = quComputer.Name;
        this.Description = quComputer.Description;
        var lastOpened = quComputer.LastOpened;
        this.LastOpened = lastOpened; 
        this.Opened = 
            string.Concat (lastOpened.ToShortDateString(), "  ", lastOpened.ToShortTimeString());
        this.IsRecent = (DateTime.Now - this.quComputer.LastOpened) < TimeSpan.FromDays(3);
    }

    public QuComputer QuComputer => this.quComputer;

    public bool IsRecent { get; private set; }

    // Replicates the model property for sorting purpose 
    public DateTime LastOpened { get; private set; }

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members

    private void OnOpen(object? _)
        => ActivateView(
            ActivatedView.Run,
            new ComputerActivationParameter(
                ComputerActivationParameter.Kind.Document, string.Empty, this.QuComputer));

    private void OnDelete(object? _)
        => Command(ToolbarCommand.DeleteDocument, this);

    #endregion Methods invoked by the Framework using reflection 
#pragma warning restore IDE0051 // Remove unused private members

    public ICommand OpenCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand DeleteCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public string Opened { get => this.Get<string>()!; set => this.Set(value); }

    public string Description { get => this.Get<string>()!; set => this.Set(value); }

}
