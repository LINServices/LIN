using LIN.Inventory.Realtime.Manager.Models;

namespace LIN.Components.Pages.Sections;

public partial class Products : IInventoryModelObserver, IDisposable
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
    private InventoryContext? Contexto { get; set; }


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
        Contexto = InventoryManager.Get(int.Parse(Id));

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

        ProductObserver.Add(Contexto?.Inventory?.ID ?? 0, this);
        deviceManager.JoinInventory(int.Parse(Id));

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
        var result = await Access.Inventory.Controllers.Product.ReadAll(Contexto?.Inventory?.ID ?? 0, Session.Instance.Token);

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
            Response?.Models.RemoveAll(t => t.Statement == Types.Inventory.Enumerations.ProductBaseStatements.Deleted);
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
    private void Go(ProductModel e)
    {
        Selected = e;
        nav.NavigateTo("/product");
    }


    /// <summary>
    /// Abrir la entradas.
    /// </summary>
    private void GoEntradas()
    {
        nav.NavigateTo($"/inflows/{Contexto?.Inventory?.ID}");
    }


    /// <summary>
    /// Abrir integrantes.
    /// </summary>
    private void GoMembers()
    {
        nav.NavigateTo($"/members/{Contexto?.Inventory?.ID}");
    }


    /// <summary>
    /// Abrir las salidas.
    /// </summary>
    private void GoSalidas()
    {
        nav.NavigateTo($"/outflows/{Contexto?.Inventory?.ID}");
    }



    /// <summary>
    /// Abrir los reportes.
    /// </summary>
    private void GoReports()
    {
        nav.NavigateTo($"/reports/{Contexto?.Inventory?.ID}");
    }


    /// <summary>
    /// Abrir crear.
    /// </summary>
    private void GoCreate()
    {
        nav.NavigateTo($"/new/product/{Contexto?.Inventory?.ID}");
    }

}