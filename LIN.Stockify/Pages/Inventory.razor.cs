using LIN.Access.Inventory.Controllers;
using LIN.Access.Inventory.Hubs;
using LIN.Types.Inventory.Models;

namespace LIN.Pages;

public partial class Inventory
{


    /// <summary>
    /// Respuesta.
    /// </summary>
    private static ReadAllResponse<InventoryDataModel>? Response { get; set; } = null;










    private InventoryAccessHub? ActualHub { get; set; }





    protected override void OnInitialized()
    {
        base.OnInitialized();
        GetData();
    }







    bool IsLoading = false;

    private async void GetData(bool force = false)
    {

        // Validación.
        if ((!force && (Response != null)) || IsLoading)
            return;

        // Cambiar el estado.
        IsLoading = true;
        StateHasChanged();

        // Obtiene los dispositivos
        var result = await Inventories.ReadAll(Session.Instance.Token);


        foreach (var item in result.Models)
            Services.InventoryContext.Post(item);
        
        // Nuevos estados.
        IsLoading = false;
        Response = result;
        StateHasChanged();
    }



}