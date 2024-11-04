﻿namespace Lyt.Quantics.Studio.Behaviors; 

public sealed class DisabledOnModal : BehaviorBase<Bindable>
{
    private IMessenger? messenger;

    protected override void OnAttached()
    {
        if (this.AssociatedObject is null)
        {
            return;
        }

        // Debug.WriteLine("On attched to: " + this.AssociatedObject.GetType().Name);
        this.messenger = App.GetRequiredService<IMessenger>();
        this.messenger.Subscribe<ModalMessage>(this.OnModalChanged);     
    }

    protected override void OnDetaching()
    {
        this.messenger?.Unregister(this);
        this.messenger = null;
        if ((this.AssociatedObject is not null) &&
            (this.AssociatedObject.Control is Control control))
        {
            control.IsEnabled = true;
            control.Opacity = 1.0; 
        }
    }

    private void OnModalChanged(ModalMessage message)
    {
        if ((this.AssociatedObject is not null) && 
            (this.AssociatedObject.Control is Control control))
        {
            control.IsEnabled = message.State == ModalMessage.Modal.Leave;
            control.Opacity = message.State == ModalMessage.Modal.Leave ? 1.0: 0.5;
        }
    }
}