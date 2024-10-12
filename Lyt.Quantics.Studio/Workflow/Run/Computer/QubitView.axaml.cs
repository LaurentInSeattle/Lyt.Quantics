namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public partial class QubitView : UserControl
{
    public QubitView()
    {
        this.InitializeComponent();
        this.DataContextChanged += this.OnQubitViewDataContextChanged;
    }

    private void OnQubitViewDataContextChanged(object? sender, EventArgs e) 
    {
        if (this.DataContext is QubitViewModel qubitViewModel)
        {
            qubitViewModel.BindOnDataContextChanged(this);
        }
    }
}