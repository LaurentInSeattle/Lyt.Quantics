namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public sealed partial class ComputerViewModel : Bindable<ComputerView>
{
    private readonly IDialogService dialogService;

    private void OnGateEditMessage(GateEditMessage message)
    {
        if (message.GateViewModel is null)
        {
            return;
        }

        // Pick up a modal dialog to run when clicking a gate
        if (this.dialogService is DialogService)
        {
            if (this.dialogService.IsModal)
            {
                // Prevents multiple clicks 
                return; 
            } 

            var gateViewModel = message.GateViewModel;
            var gate = gateViewModel.Gate;
            bool isUnary = gate.QuBitsTransformed == 1;
            bool isBinary = gate.QuBitsTransformed == 2;
            bool isTernary = gate.QuBitsTransformed == 3;
            bool isNotControlled = !gate.IsControlled;
            bool launchQubitEditor =
                // Edit Targets for swap 
                (isBinary && gate.TargetQuBits == 2) ||
                // Edit control and target for binary 
                (isBinary && !isNotControlled) ||
                // All ternary gates are controlled or can be edited 
                isTernary;
            if (gate.HasAngleParameter)
            {
                // Special click: Go direct to the controlled gate dialog, if possible 
                if (message.WithModifier && isUnary && isNotControlled)
                {
                    this.LaunchGateEditControlDialog(gateViewModel);
                }
                else
                {
                    // Edit angle
                    this.LaunchGateEditAngleDialog(gateViewModel);
                }
            }
            else if (isUnary && isNotControlled)
            {
                // Create a controlled gate
                this.LaunchGateEditControlDialog(gateViewModel);
            }
            else if (launchQubitEditor)
            {
                // Change qubits inputs
                this.LaunchEditQubitsDialog(gateViewModel);
            }
            else
            {
                // Cannot edit: should never happen 
                throw new Exception("Cannot edit: should never happen.");
            }
        }
    }

    private void LaunchGateEditAngleDialog(GateViewModel gateViewModel)
    {
        // Run modal dialog to phase or rotation angle
        if (this.dialogService is DialogService modalService)
        {
            modalService.RunModal<GateEditAngleDialog, GateViewModel>(
                this.View.ToasterHost, new GateEditAngleDialogModel(),
                this.OnGateEditAngleClose, gateViewModel);
        }
    }

    private void OnGateEditAngleClose(object sender, bool save)
    {
        if ((!save) || (sender is not GateEditAngleDialogModel gateEditDialogModel))
        {
            // Dismissed or problem...
            return;
        }

        if (gateEditDialogModel.IsMakeControlled)
        {
            this.dialogService.Dismiss();
            Schedule.OnUiThread(20, () =>
            {
                this.LaunchGateEditControlDialog(gateEditDialogModel.GateViewModel);
            }, DispatcherPriority.Background);
            return;
        }

        try
        {
            // Grab the data 
            GateViewModel gateViewModel = gateEditDialogModel.GateViewModel;
            Gate oldGate = gateViewModel.Gate;
            if ((oldGate is not RotationGate) && (oldGate is not PhaseGate))
            {
                throw new Exception("Incorrect gate type.");
            }

            Gate? newGate = null;
            var gateParameters = gateEditDialogModel.GateParameters;
            if (oldGate is RotationGate rotationGate)
            {

                newGate = new RotationGate(gateParameters);
            }
            else // (oldGate is PhaseGate)
            {
                newGate = new PhaseGate(gateParameters);
            }

            if (newGate is null)
            {
                throw new Exception("Failed to create a new gate.");
            }

            // Update the model. The gate view likely needs an update.
            // It will be done later when the model will be messaging.
            // No need to update the UI here
            int stageIndex = gateViewModel.StageIndex;
            var stage = this.Stages[stageIndex];
            stage.AddGateAt(gateViewModel.QubitsIndices, newGate);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Gate Edit: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    private void LaunchGateEditControlDialog(GateViewModel gateViewModel)
    {
        // Run modal dialog to construct a controlled gate from the existing one
        if (this.dialogService is DialogService modalService)
        {
            modalService.RunModal<GateEditControlDialog, GateViewModel>(
                this.View.ToasterHost, new GateEditControlDialogModel(),
                this.OnGateEditControlClose, gateViewModel);
        }
    }

    private void OnGateEditControlClose(object sender, bool save)
    {
        if ((!save) || (sender is not GateEditControlDialogModel gateEditControlDialogModel))
        {
            // Dismissed or problem...
            return;
        }

        try
        {
            // TODO !!! 

            // Grab the data 
            GateViewModel gateViewModel = gateEditControlDialogModel.GateViewModel;
            Gate oldGate = gateViewModel.Gate;

            // Gate view likely needs an update 
            // gateViewModel.Update(gate); 

            // Update the model 

            // TODO !!! 
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Gate Edit Controlled: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    private void LaunchEditQubitsDialog(GateViewModel gateViewModel)
    {
        // Run modal dialog to edit Targets for swap or edit control and target for binary gates 
        if (this.dialogService is DialogService modalService)
        {
            modalService.RunModal<GateEditQubitsDialog, GateViewModel>(
                this.View.ToasterHost, new GateEditQubitsDialogModel(),
                this.OnEditQubitsClose, gateViewModel);
        }
    }

    private void OnEditQubitsClose(object sender, bool save)
    {
        if ((!save) || (sender is not GateEditControlDialogModel gateEditControlDialogModel))
        {
            // Dismissed or problem...
            return;
        }

        try
        {
            // TODO !!! 

            // Grab the data 
            GateViewModel gateViewModel = gateEditControlDialogModel.GateViewModel;
            Gate oldGate = gateViewModel.Gate;

            // Gate view likely needs an update 
            // gateViewModel.Update(gate); 

            // Update the model 

            // TODO !!! 
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Gate Edit Controlled: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }
}
