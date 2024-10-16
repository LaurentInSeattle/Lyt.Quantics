namespace Lyt.Quantics.Studio.Workflow.Run.Amplitudes;

using Lyt.Quantics.Engine.Utilities;
using static Lyt.Avalonia.Controls.Utilities;
using static Lyt.Quantics.Studio.Messaging.ToolbarCommandMessage;

public sealed class AmplitudesViewModel : Bindable<AmplitudesView>
{
    private readonly QuanticsStudioModel quanticsStudioModel;

    private HistogramViewModel? histogramViewModel;
    private List<HistogramEntry> histogramEntries;
    private bool showAll;
    private bool showByBitOrder;

    public AmplitudesViewModel()
    {
        // Do not use Injection directly as this is loaded programmatically by the RunView 
        this.quanticsStudioModel = App.GetRequiredService<QuanticsStudioModel>();
        this.histogramEntries = [];
        this.Messenger.Subscribe<ToolbarCommandMessage>(this.OnToolbarCommandMessage);
        this.Messenger.Subscribe<ModelResultsUpdateMessage>(this.OnModelResultsUpdateMessage);
        this.Messenger.Subscribe<ModelStructureUpdateMessage>(this.OnModelStructureUpdateMessage);
    }

    // TODO in toolbar : 
    // Show only above value ???? 

    protected override void OnViewLoaded()
    {
        this.OnModelStructureUpdateMessage(new ModelStructureUpdateMessage(false));
    }

    private void OnToolbarCommandMessage(ToolbarCommandMessage message)
    {
        if (message.CommandParameter is bool value)
        {
            switch (message.Command)
            {
                case ToolbarCommand.ShowAll: this.ShowAll(value); break;

                case ToolbarCommand.ShowByBitOrder: this.ShowByBitOrder(value); break;

                default:
                    break;
            }
        }
    }

    private void ShowByBitOrder(bool value)
    {
        this.showByBitOrder = value;
        this.FilterAndUpdate();
    }

    private void ShowAll(bool value)
    {
        this.showAll = value;
        this.FilterAndUpdate();
    }

    private void FilterAndUpdate()
    {
        if ((this.histogramViewModel is null) || (this.histogramEntries.Count == 0))
        {
            return;
        }

        List<HistogramEntry> filtered;
        if (this.showAll)
        {
            filtered = [.. this.histogramEntries];
        }
        else
        {
            // Non zero 
            filtered =
                [.. (from entry in this.histogramEntries
                 where entry.Value > MathUtilities.Epsilon
                 select entry)];
        }

        List<HistogramEntry> sorted;
        if (this.showByBitOrder)
        {
            sorted = [.. (from entry in filtered orderby entry.Label select entry)];
        }
        else
        {
            sorted = [.. (from entry in filtered orderby entry.Value descending select entry)];
        }

        this.histogramViewModel.Update(sorted);
    }

    private void OnModelResultsUpdateMessage(ModelResultsUpdateMessage _)
    {
        // Update probabilities 
        this.View.AmplitudesGrid.Children.Clear();
        var computer = this.quanticsStudioModel.QuComputer;
        if (computer.IsComplete)
        {
            QuRegister lastRegister = computer.Stages[^1].StageRegister;
            List<Tuple<string, double>> bitValuesProbabilities = lastRegister.BitValuesProbabilities();
            var vm = new HistogramViewModel();
            this.View.AmplitudesGrid.Children.Add(vm.CreateViewAndBind());
            this.histogramEntries = new List<HistogramEntry>(bitValuesProbabilities.Count);
            foreach (var bitValue in bitValuesProbabilities)
            {
                var entry = new HistogramEntry(bitValue.Item2, bitValue.Item1);
                this.histogramEntries.Add(entry);
            }

            this.histogramViewModel = vm;
            this.FilterAndUpdate();
        }
    }

    private void OnModelStructureUpdateMessage(ModelStructureUpdateMessage _)
    {
        // Clear the view: Show nothing, but a "no data" indication  
        this.histogramViewModel = null;
        this.histogramEntries = [];
        this.View.AmplitudesGrid.Children.Clear();
        TryFindResource("PastelOrchid_1_100", out SolidColorBrush? brush);
        if (brush is null)
        {
            throw new Exception("Failed to retrieve brush");
        }

        var textBlock = new TextBlock()
        {
            Text = "< No Data >",
            FontSize = 16,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(16),
            Foreground = brush,
        };

        this.View.AmplitudesGrid.Children.Add(textBlock);
    }
}
