using LIN.Inventory.Shared.Interfaces;

namespace LIN.Services;

public class DeviceSelector : IDeviceSelector
{

    public void Send(string command)
    {
        // Nuevo onInvoque.
        MainLayout.DevicesSelector.OnInvoke = (e) =>
            {
                Services.Realtime.InventoryAccessHub.SendToDevice(e.Id, new()
                {
                    Command = command
                });
            };

        Components.Layout.MainLayout.DevicesSelector.Show();
    }

}