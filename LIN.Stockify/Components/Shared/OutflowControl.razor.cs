using LIN.Types.Inventory.Models;

namespace LIN.Components.Shared;


public partial class OutflowControl
{


    /// <summary>
    /// Modelo del producto.
    /// </summary>
    [Parameter]
    public OutflowDataModel? Model { get; set; }



    /// <summary>
    /// Evento al hacer click.
    /// </summary>
    [Parameter]
    public Action<OutflowDataModel?>? OnClick { get; set; }



    /// <summary>
    /// Enviar el evento.
    /// </summary>
    private void SendEvent()
    {
        OnClick?.Invoke(Model);
    }



    private string GetImage()
    {



        switch (Model.Type)
        {
            case Types.Inventory.Enumerations.OutflowsTypes.Consumo:
                return "./img/Products/outflows/seller.png";
            case Types.Inventory.Enumerations.OutflowsTypes.Donacion:
                return "./img/Products/outflows/donate.png";
            case Types.Inventory.Enumerations.OutflowsTypes.Fraude:
                return "./img/Products/outflows/criminal.png";
            case Types.Inventory.Enumerations.OutflowsTypes.Venta:
                return "./img/Products/outflows/shop.png";
            case Types.Inventory.Enumerations.OutflowsTypes.Perdida:
                return "./img/Products/outflows/lost.png";
            case Types.Inventory.Enumerations.OutflowsTypes.Caducidad:
                return "./img/Products/outflows/expired.png";
            default:
                return "./img/Products/packages.png";
        }

    }

}