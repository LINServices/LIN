using LIN.Inventory.Shared.Services.Models;
using LIN.Inventory.Shared.Services.Observers;

namespace LIN.Components.Pages.Sections;

public partial class Products : IProduct, IDisposable
{


    /// <summary>
    /// Id.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;



    /// <summary>
    /// Esta cargando.
    /// </summary>
    private bool IsLoading = false;



    /// <summary>
    /// Producto seleccionado.
    /// </summary>
    public static ProductModel? Selected { get; set; } = null;



    /// <summary>
    /// Contexto del inventario.
    /// </summary>
    InventoryContextModel? Contexto { get; set; }



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
        Contexto = InventoryContext.Get(int.Parse(Id));

        // Evaluar el contexto.
        if (Contexto != null)
            Response = Contexto.Products;
        else
            Contexto = new()
            {
                Inventory = new()
                {
                    ID = int.Parse(Id),
                }
            };

        // Evaluar la respuesta.
        if (Response == null)
            GetData();

        ProductObserver.Add(Contexto?.Inventory.ID ?? 0, this);

        _ = Services.Realtime.InventoryAccessHub?.JoinInventory(int.Parse(Id));

        // Base.
        base.OnParametersSet();
    }



    /// <summary>
    /// Obtener la data.
    /// </summary>
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



    /// <summary>
    /// Renderizar.
    /// </summary>
    public void Render()
    {
        InvokeAsync(() =>
        {
            Response = Contexto?.Products;
            StateHasChanged();
        });
    }



    /// <summary>
    /// Evento Dispose.
    /// </summary>
    public void Dispose()
    {
        ProductObserver.Remove(this);
    }



    /// <summary>
    /// Evento después de renderizar.
    /// </summary>
    protected override void OnAfterRender(bool firstRender)
    {
        MainLayout.Configure(new()
        {
            OnCenterClick = GoCreate,
            Section = 1,
            DockIcon = 0
        });

        base.OnAfterRender(firstRender);
    }



    /// <summary>
    /// Abrir el producto.
    /// </summary>
    /// <param name="e">Modelo.</param>
    void Go(ProductModel e)
    {
        Selected = e;
        nav.NavigateTo("/product");
    }



    /// <summary>
    /// Abrir la entradas.
    /// </summary>
    void GoEntradas()
    {
        nav.NavigateTo($"/inflows/{Contexto?.Inventory.ID}");
    }



    /// <summary>
    /// Abrir integrantes.
    /// </summary>
    void GoMembers()
    {
        nav.NavigateTo($"/members/{Contexto?.Inventory.ID}");
    }



    /// <summary>
    /// Abrir las salidas.
    /// </summary>
    void GoSalidas()
    {
        nav.NavigateTo($"/outflows/{Contexto?.Inventory.ID}");
    }



    /// <summary>
    /// Abrir crear.
    /// </summary>
    void GoCreate()
    {
        nav.NavigateTo($"/new/product/{Contexto?.Inventory.ID}");
    }


}
