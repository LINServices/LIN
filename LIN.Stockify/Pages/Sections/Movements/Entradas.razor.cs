using LIN.Access.Inventory.Controllers;
using LIN.Types.Inventory.Models;

namespace LIN.Pages.Sections.Movements;

public partial class Entradas
{


    [Parameter]
    public string Id { get; set; } = string.Empty;



    Services.Models.InventoryContextModel? Contexto { get; set; }



    /// <summary>
    /// Respuesta.
    /// </summary>
    private static ReadAllResponse<InflowDataModel>? Response { get; set; } = null;




    public static InflowDataModel? Selected { get; set; } = null;







    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {

        // Obtener el contexto.
        Contexto = Services.InventoryContext.Get(int.Parse(Id));

        // Evaluar el contexto.
        if (Contexto != null)
            Response = Contexto.Inflows;

        // Evaluar la respuesta.
        if (Response == null)
            GetData();

        // Base.
        base.OnParametersSet();
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
        var result = await Inflows.ReadAll(Contexto?.Inventory.ID ?? 0, Session.Instance.Token);

        // Nuevos estados.
        IsLoading = false;
        Response = result;

        if (Contexto != null)
            Contexto.Inflows = Response;

        StateHasChanged();
    }



}
