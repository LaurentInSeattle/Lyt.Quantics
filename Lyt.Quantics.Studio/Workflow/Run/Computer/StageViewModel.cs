namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public sealed class StageViewModel : Bindable<StageView>
{
    public readonly int stageIndex;
    public readonly QuanticsStudioModel quanticsStudioModel; 
    public readonly IToaster toaster;

    public StageViewModel(int stageIndex, QuanticsStudioModel quanticsStudioModel)
    {
        this.stageIndex = stageIndex;
        this.quanticsStudioModel = quanticsStudioModel;
        this.Gates = new GateViewModel?[ComputerViewModel.MaxQubits];
        this.Name = (1+stageIndex).ToString();
        this.IsMarkerVisible = true; 
        this.toaster = App.GetRequiredService<IToaster>();
    }

    public GateViewModel? [ ] Gates { get; private set; }

    protected override void OnViewLoaded()
    {
        this.CreateAmplitudeMinibars ();
    }

    private void CreateAmplitudeMinibars()
    {
        this.View.MinibarsGrid.Children.Clear();
        for (int i = 0; i < ComputerViewModel.MaxQubits; i++)
        {
            bool visible = i < this.quanticsStudioModel.QuComputer.QuBitsCount; 
            var vm = new AmplitudeMinibarViewModel (i*0.1, visible);
            vm.CreateViewAndBind();
            var view = vm.View; 
            view.SetValue(Grid.RowProperty, i);
            this.View.MinibarsGrid.Children.Add (view);
        }
    }

    public bool CanDrop(Point point, GateViewModel gateViewModel)
    {
        if (!gateViewModel.IsToolbox)
        {
            return false;
        }

        // 600 pixels for 10 qubits ~ Magic numbers !
        double offset = point.Y / 60.0; 
        if ( ( offset < 0.0 ) || ( offset >= 10.0 )) 
        {
            // outside qubit area: reject
            return false;
        }

        return true;
    }

    public void OnDrop(Point point, GateViewModel gateViewModel)
    {
        if (!gateViewModel.IsToolbox)
        {
            return;
        }

        // 600 pixels for 10 qubits ~ Magic numbers !
        double offset = point.Y / 60.0;
        if ((offset < 0.0) || (offset >= 10.0))
        {
            // outside qubit area: reject
            return;
        }

        int qubitIndex = (int)Math.Floor(offset);
        this.AddGateAt(qubitIndex, gateViewModel.Gate);
    }

    private void AddGateAt(int qubitIndex, Gate gate)
    {
        if (!this.quanticsStudioModel.AddGate(this.stageIndex, qubitIndex, gate, out string message))
        {
            this.toaster.Show("Failed to Add Gate!", message, 4_000, InformationLevel.Error);
            return;
        }

        var gateViewModel = new GateViewModel(gate, isToolbox: false);
        gateViewModel.CreateViewAndBind();
        this.Gates[qubitIndex] = gateViewModel;
        this.UpdateUiGates();
    }

    private void UpdateUiGates ()
    {
        this.View.GatesGrid.Children.Clear();
        for (int i = 0; i < ComputerViewModel.MaxQubits; i++)
        {
            var viewModel = this.Gates[i];
            if (viewModel is null)
            {
                continue;
            } 

            var view = viewModel.View;
            view.SetValue(Grid.RowProperty, i);
            this.View.GatesGrid.Children.Add(view);
        }
    }

    public string Name { get => this.Get<string>()!; set => this.Set(value); } 

    public bool IsMarkerVisible { get => this.Get<bool>(); set => this.Set(value); }

}
