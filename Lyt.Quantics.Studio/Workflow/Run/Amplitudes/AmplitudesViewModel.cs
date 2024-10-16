namespace Lyt.Quantics.Studio.Workflow.Run.Amplitudes;

using static Lyt.Avalonia.Controls.Utilities;

public sealed class AmplitudesViewModel : Bindable<AmplitudesView>
{
    private readonly QuanticsStudioModel quanticsStudioModel;

    private List<HistogramEntry> histogramEntries; 

    public AmplitudesViewModel()
    {
        // Do not use Injection directly as this is loaded programmatically by the RunView 
        this.quanticsStudioModel = App.GetRequiredService<QuanticsStudioModel>();
        this.histogramEntries = [];
        this.Messenger.Subscribe<ModelResultsUpdateMessage>(this.OnModelResultsUpdateMessage);
        this.Messenger.Subscribe<ModelStructureUpdateMessage>(this.OnModelStructureUpdateMessage);
    }

    // TODO in toolbar : 
    // Show only non zero / show all 
    // Show only above value 
    // Order by decreasing probability / by bit order 

    protected override void OnViewLoaded()
    {
        this.OnModelStructureUpdateMessage(new ModelStructureUpdateMessage(false));
    }

    private void OnModelResultsUpdateMessage(ModelResultsUpdateMessage _)
    {
        // Update probabilities 
        this.View.AmplitudesGrid.Children.Clear();
        var computer = this.quanticsStudioModel.QuComputer;
        if ( computer.IsComplete)
        {
            QuRegister lastRegister = computer.Stages[^1].StageRegister;
            List<Tuple<string, double>> bitValuesProbabilities = lastRegister.BitValuesProbabilities();
            var vm = new HistogramViewModel();
            this.View.AmplitudesGrid.Children.Add(vm.CreateViewAndBind());
            this.histogramEntries = new List<HistogramEntry>(bitValuesProbabilities.Count);
            foreach ( var bitValue in bitValuesProbabilities )
            {
                var entry = new HistogramEntry(bitValue.Item2, bitValue.Item1);
                this.histogramEntries.Add(entry);
            }

            vm.Update(this.histogramEntries);
        }
    }

    private void OnModelStructureUpdateMessage(ModelStructureUpdateMessage _)
    {
        // Clear the view: Show nothing 
        this.histogramEntries = [];
        this.View.AmplitudesGrid.Children.Clear();
        TryFindResource("PastelOrchid_1_100", out SolidColorBrush? brush);
        if ( brush is null )
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
