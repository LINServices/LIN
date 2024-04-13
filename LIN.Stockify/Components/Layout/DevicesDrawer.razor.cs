using LIN.Services;

namespace LIN.Components.Layout;


public partial class DevicesDrawer
{


    /// <summary>
    /// ID del elemento Html.
    /// </summary>
    public string _id = $"element-{Guid.NewGuid()}";


    /// <summary>
    /// Lista de dispositivos.
    /// </summary>
    [Parameter]
    public static ReadAllResponse<DeviceModel> DevicesList { get; set; } = null!;


    /// <summary>
    /// Evento onclick.
    /// </summary>
    [Parameter]
    public Action<Types.Inventory.Models.DeviceModel> OnInvoke { get; set; } = (d) => { };


    /// <summary>
    /// Es la primer abierta?
    /// </summary>
    public bool FirstShow { get; set; } = true;


    /// <summary>
    /// Abrir el elemento.
    /// </summary>
    public async void Show()
    {

        // Abrir el elemento.
        await JS.InvokeVoidAsync("ShowDrawer", _id, DotNetObjectReference.Create(this), $"btn-close-{_id}", "close-all-all");


        // Si es el primer open.
        if (FirstShow)
        {
            _ = GetDevices();
            FirstShow = false;
        }
        StateHasChanged();
    }



    /// <summary>
    /// Obtener los dispositivos.
    /// </summary>
    private async Task<bool> GetDevices()
    {

        // Items
        var items = await Access.Inventory.Controllers.Profile.ReadDevices(Access.Inventory.Session.Instance.Token);

        // Rellena los items
        DevicesList = items;

        // Eliminar dispositivo local.
        items.Models.RemoveAll(t => t.LocalId == Realtime.DeviceKey);

        StateHasChanged();
        return true;

    }


}