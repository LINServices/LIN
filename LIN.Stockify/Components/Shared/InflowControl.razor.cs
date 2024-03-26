using LIN.Types.Inventory.Models;

namespace LIN.Components.Shared;


public partial class InflowControl
{


    /// <summary>
    /// Modelo del producto.
    /// </summary>
    [Parameter]
    public InflowDataModel? Model { get; set; }



    /// <summary>
    /// Evento al hacer click.
    /// </summary>
    [Parameter]
    public Action<InflowDataModel?>? OnClick { get; set; }



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
            case Types.Inventory.Enumerations.InflowsTypes.Compra:
                return "./img/Products/inflows/cart.png";
            case Types.Inventory.Enumerations.InflowsTypes.Devolucion:
                return "./img/Products/inflows/return.png";
            case Types.Inventory.Enumerations.InflowsTypes.Regalo:
                return "./img/Products/inflows/gift.png";
            case Types.Inventory.Enumerations.InflowsTypes.Ajuste:
                return "./img/Products/inflows/setting.png";
            default:
                return "./img/Products/packages.png";
        }

    }

}