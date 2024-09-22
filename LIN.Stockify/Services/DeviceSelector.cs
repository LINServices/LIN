using LIN.Inventory.Realtime.Manager;
using LIN.Inventory.Shared.Interfaces;

namespace LIN.Services;

public class DeviceSelector(IDeviceManager deviceManager) : IDeviceSelector
{

    public void Send(string command)
    {
        // Nuevo onInvoque.
        MainLayout.DevicesSelector.OnInvoke = (e) =>
            {
                deviceManager.SendToDevice(command, e.Id);
            };

        // Abrir el selector de dispositivos.
        MainLayout.DevicesSelector.Show();
    }

}