using LIN.Components.Layout;
using LIN.Types.Inventory.Models;

namespace LIN.Pages;

public partial class Account
{




    async void SendCo(string devices)
    {
        Services.Realtime.InventoryAccess.SendToDevice(devices, new()
        {
            Command = "updateCt()"
        });
    }

    async void Close()
    {
        LIN.Access.Inventory.Session.CloseSession();
        await new LIN.LocalDataBase.Data.UserDB().DeleteUsers();

        nav.NavigateTo("/", true, true);

    }

    List<DeviceModel> Devices = null;
    private async Task<bool> GetDevices()
    {


        // Items
        var items = await LIN.Access.Inventory.Controllers.Profile.ReadDevices(LIN.Access.Inventory.Session.Instance.Token);

        // Analisis de respuesta
        if (items.Response != Responses.Success)
            return false;

        // Rellena los items
        Devices = items.Models.ToList();
        return true;

    }


    protected override void OnInitialized()
    {

        MainLayout.Configure(new()
        {
            OnCenterClick = () => { },
            Section = 3,
            DockIcon = 0
        });

        base.OnInitialized();
    }

}
