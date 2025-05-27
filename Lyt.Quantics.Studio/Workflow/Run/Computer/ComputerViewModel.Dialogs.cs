namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public sealed partial class ComputerViewModel : ViewModel<ComputerView>
{
    private readonly IDialogService dialogService;

    private void OnGateEditMessage(GateEditMessage message)
    {
        if (message.GateInfoProvider is null)
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

            var gateInfoProvider = message.GateInfoProvider;
            var gate = gateInfoProvider.Gate;
            bool isBinary = gate.QuBitsTransformed == 2;
            bool isTernary = gate.QuBitsTransformed == 3;
            bool isMutable = gate.IsMutable;
            bool launchQubitEditor =
                // Edit Targets for swap 
                (isBinary && gate.TargetQuBits == 2) ||
                // Edit control and target for binary 
                (isBinary && !isMutable) ||
                // All ternary gates are controlled or can be edited 
                isTernary;
            if (gate.HasAngleParameter)
            {
                // Special click: Go direct to the controlled gate dialog, if possible 
                if (message.WithModifier && isMutable)
                {
                    this.LaunchGateEditControlDialog(gateInfoProvider);
                }
                else
                {
                    // Edit angle
                    this.LaunchGateEditAngleDialog(gateInfoProvider);
                }
            }
            else if (isMutable)
            {
                // Create a controlled gate
                this.LaunchGateEditControlDialog(gateInfoProvider);
            }
            else if (launchQubitEditor)
            {
                // Change qubits inputs
                this.LaunchEditQubitsDialog(gateInfoProvider);
            }
            else
            {
                // Cannot edit: should rarely happen 
                this.Logger.Warning("Cannot edit: " + gate.CaptionKey);
            }
        }
    }

    #region Editing Angle for Phase and Rotation 

    private void LaunchGateEditAngleDialog(IGateInfoProvider gateInfoProvider)
    {
        // Run modal dialog to phase or rotation angle
        if (this.dialogService is DialogService modalService)
        {
            modalService.RunModal<GateEditAngleDialog, IGateInfoProvider>(
                this.View.ToasterHost, new GateEditAngleDialogModel(),
                this.OnGateEditAngleClose, gateInfoProvider);
        }
    }

    private void OnGateEditAngleClose(object sender, bool save)
    {
        if ((!save) || (sender is not GateEditAngleDialogModel gateEditAngleDialogModel))
        {
            // Dismissed or problem...
            return;
        }

        if (gateEditAngleDialogModel.IsMakeControlled)
        {
            // Replace the current modal dialog with the one to construct a
            // controlled gate from the existing one.
            if (this.dialogService is DialogService modalService)
            {
                // Assumes that the host panel has not been changed 
                modalService.ReplaceRunModal<GateEditControlDialog, IGateInfoProvider>(
                    new GateEditControlDialogModel(),
                    this.OnGateEditControlClose, 
                    gateEditAngleDialogModel.GateInfoProvider);
            }

            return;
        }

        try
        {
            // Grab the data 
            IGateInfoProvider gateInfoProvider = gateEditAngleDialogModel.GateInfoProvider;
            Gate oldGate = gateInfoProvider.Gate;
            if ((oldGate is not RotationGate) && (oldGate is not PhaseGate))
            {
                throw new Exception("Incorrect gate type.");
            }

            Gate? newGate = null;
            var gateParameters = gateEditAngleDialogModel.GateParameters;
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
            int stageIndex = gateInfoProvider.StageIndex;
            var stage = this.Stages[stageIndex];
            stage.AddGateAt(gateInfoProvider.QubitsIndices, newGate, isDrop: false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Gate Edit: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    #endregion Editing Angle for Phase and Rotation 

    #region Mutating a unary into a controlled gate  

    private void LaunchGateEditControlDialog(IGateInfoProvider gateInfoProvider)
    {
        // Run modal dialog to construct a controlled gate from the existing one
        if (this.dialogService is DialogService modalService)
        {
            modalService.RunModal<GateEditControlDialog, IGateInfoProvider>(
                this.View.ToasterHost, new GateEditControlDialogModel(),
                this.OnGateEditControlClose, gateInfoProvider);
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
            IGateInfoProvider gateInfoProvider = gateEditControlDialogModel.GateInfoProvider;
            int stageIndex = gateInfoProvider.StageIndex;
            var computer = this.quanticsStudioModel.QuComputer;
            var computerStage = computer.Stages[stageIndex];

            // Original qubits indices 
            var stageOperator = computerStage.StageOperatorAt(gateInfoProvider.QubitsIndices);
            var gateParameters = stageOperator.GateParameters;
            Gate oldGate = gateInfoProvider.Gate;
            var newGate = new ControlledGate(oldGate);

            // Update the model. The gate view likely needs an update.
            // It will be done later when the model will be messaging.
            // No need to update the UI here
            var uiStage = this.Stages[stageIndex];

            // Here use modified qubits indices 
            uiStage.AddGateAt(gateEditControlDialogModel.QubitsIndices, newGate, isDrop: false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Gate Edit Controlled: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    #endregion Mutating a unary into a controlled gate  

    #region Modifying control and target qubits for a binary or ternary gate

    private void LaunchEditQubitsDialog(IGateInfoProvider gateInfoProvider)
    {
        // Run modal dialog to edit Targets for swap or edit control and target for binary gates 
        if (this.dialogService is DialogService modalService)
        {
            modalService.RunModal<GateEditQubitsDialog, IGateInfoProvider>(
                this.View.ToasterHost, new GateEditQubitsDialogModel(),
                this.OnEditQubitsClose, gateInfoProvider);
        }
    }

    private void OnEditQubitsClose(object sender, bool save)
    {
        if ((!save) || (sender is not GateEditQubitsDialogModel gateEditQubitsDialogModel))
        {
            // Dismissed or problem...
            return;
        }

        try
        {
            // Grab the data 
            IGateInfoProvider gateInfoProvider = gateEditQubitsDialogModel.GateInfoProvider;
            int stageIndex = gateInfoProvider.StageIndex;
            var computer = this.quanticsStudioModel.QuComputer;
            var computerStage = computer.Stages[stageIndex];

            // Use original qubits indices 
            var stageOperator = computerStage.StageOperatorAt(gateInfoProvider.QubitsIndices);
            var gateParameters = stageOperator.GateParameters;
            Gate oldGate = gateInfoProvider.Gate;
            var newGate = GateFactory.Produce(oldGate.CaptionKey, gateParameters);

            // Update the model. The gate view likely needs an update.
            // It will be done later when the model will be messaging.
            // No need to update the UI here
            var uiStage = this.Stages[stageIndex];

            // Here use modified qubits indices 
            uiStage.AddGateAt(gateEditQubitsDialogModel.QubitsIndices, newGate, isDrop: false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Gate Edit Controlled: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    #endregion Modifying control and target qubits for a binary or ternary gate
}
