namespace Lyt.Quantics.Studio.Workflow.Run.Amplitudes;

using static Utilities;
using static ToolbarCommandMessage;

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

    protected override void OnViewLoaded()
    {
        // Note that this view gets loaded when we expand it for the very first time
        // At this moment, we may already have received model result updates, in such case 
        // we display them, Otherwise just display the "no data" view.
        if (this.quanticsStudioModel.QuComputer.IsComplete)
        {
            this.OnModelResultsUpdateMessage(new ModelResultsUpdateMessage());
        }
        else
        {
            this.OnModelStructureUpdateMessage(new ModelStructureUpdateMessage(false));
        }
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
        else if (message.CommandParameter is int rank)
        {
            if (message.Command == ToolbarCommand.ShowStage)
            {
                this.UpdateProbabilities(rank);
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
        Debug.WriteLine("Amplitudes: OnModelResultsUpdateMessage");
        this.UpdateProbabilities(this.quanticsStudioModel.QuComputer.Stages.Count);
    }

    private void OnModelStructureUpdateMessage(ModelStructureUpdateMessage _)
    {
        Debug.WriteLine("Amplitudes: OnModelStructureUpdateMessage");

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
            Margin = new Thickness(32),
            Foreground = brush,
        };

        this.View.AmplitudesGrid.Children.Add(textBlock);
    }

    private void UpdateProbabilities (int rank )
    {
        // Update probabilities 
        this.View.AmplitudesGrid.Children.Clear();
        var computer = this.quanticsStudioModel.QuComputer;
        if (computer.IsComplete)
        {
            QuRegister lastRegister = computer.Stages[rank-1].StageRegister;
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
}
