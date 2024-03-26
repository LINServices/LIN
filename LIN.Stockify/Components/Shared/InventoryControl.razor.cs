using LIN.Types.Inventory.Models;

namespace LIN.Components.Shared;

public partial class InventoryControl
{
    /// <summary>
    /// Modelo de inventario.
    /// </summary>
    [Parameter]
    public InventoryDataModel? Model { get; set; }



    /// <summary>
    /// Evento al hacer click.
    /// </summary>
    [Parameter]
    public Action<InventoryDataModel?>? OnClick { get; set; }



    /// <summary>
    /// Enviar el evento.
    /// </summary>
    private void SendEvent()
    {
        OnClick?.Invoke(Model);
    }

}
