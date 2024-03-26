using LIN.Services.RealTime;
using LIN.Types.Inventory.Models;

namespace LIN.Pages.Sections;

public partial class Products : IProduct, IDisposable
{


    [Parameter]
    public string Id { get; set; } = string.Empty;



    Services.Models.InventoryContextModel? Contexto { get; set; }


    /// <summary>
    /// Respuesta.
    /// </summary>
    private ReadAllResponse<ProductModel>? Response { get; set; } = null;




    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {

        // Obtener el contexto.
        Contexto = Services.InventoryContext.Get(int.Parse(Id));

        // Evaluar el contexto.
        if (Contexto != null)
            Response = Contexto.Products;

        // Evaluar la respuesta.
        if (Response == null)
            GetData();

        ProductObserver.Add(Contexto!.Inventory.ID, this);

        _ = Services.Realtime.InventoryAccess.JoinInventory(int.Parse(Id));

        // Base.
        base.OnParametersSet();
    }





    public static ProductModel? Selected { get; set; } = null;














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
        var result = await Access.Inventory.Controllers.Product.ReadAll(Contexto?.Inventory.ID ?? 0, Session.Instance.Token);

        // Nuevos estados.
        IsLoading = false;
        Response = result;

        if (Contexto != null)
            Contexto.Products = Response;

        StateHasChanged();
    }

    public void Render()
    {
        InvokeAsync(() =>
        {
            Response = Contexto.Products;
            StateHasChanged();
        });
    }


    public void Dispose()
    {
        ProductObserver.Remove(this);
    }
}
