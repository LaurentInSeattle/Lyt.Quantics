using Lyt.Quantics.Studio.Workflow.Run.Dialogs;

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

        // Run modal dialog 
        if (this.dialogService is DialogService modalService)
        {
            var gateViewModel = message.GateViewModel;
            var gate = gateViewModel.Gate;
            bool isUnary = gate.QuBitsTransformed == 1;
            bool isNotControlled = !gate.IsControlled;
            if (gate.HasAngleParameter)
            {
                modalService.RunModal<GateEditDialog, GateViewModel>(
                    this.View.ToasterHost, new GateEditDialogModel(),
                    this.OnGateEditClose, gateViewModel);
            } 
            else if (isUnary && isNotControlled) 
            {
                this.LaunchGateEditControlDialog(gateViewModel);
            }
            else
            {
                // Cannot edit: should never happen 
                throw new Exception("Cannot edit: should never happen.");
            }
        }
    }

    private void OnGateEditClose(object sender, bool save)
    {
        if ((!save) || (sender is not GateEditDialogModel gateEditDialogModel))
        {
            // Dismissed or problem...
            return;
        }

        if (gateEditDialogModel.IsMakeControlled)
        {
            this.dialogService.Dismiss();
            Schedule.OnUiThread(20, ()=>
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
            stage.AddGateAt(gateViewModel.QubitIndex, newGate);
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
            // Grab the data 
            GateViewModel gateViewModel = gateEditControlDialogModel.GateViewModel;
            Gate oldGate = gateViewModel.Gate;

            // Gate view likely needs an update 
            // gateViewModel.Update(gate); 

            // Update the model 
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Gate Edit Controlled: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }
}
