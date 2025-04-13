using LIN.Inventory.Realtime.Manager.Models;

namespace LIN.Components.Pages.Sections;

public partial class Payments
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
    InventoryContext? Contexto { get; set; }



    /// <summary>
    /// Respuesta.
    /// </summary>
    private ReadAllResponse<Types.Payments.Models.PayModel>? Response { get; set; } = null;



    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {

        // Obtener el contexto.
        Contexto = InventoryManager.Get(int.Parse(Id));

        // Evaluar el contexto.
        if (Contexto != null)
            Response = Contexto.Payments;
        else
            Contexto = new()
            {
                Inventory = new()
                {
                    Id = int.Parse(Id),
                }
            };

        GetData();

        // Base.
        base.OnParametersSet();
    }



    /// <summary>
    /// Obtener la data.
    /// </summary>
    private async void GetData(bool force = false)
    {

        // Validación.
        if (!force && Response != null || IsLoading)
            return;

        // Cambiar el estado.
        IsLoading = true;
        StateHasChanged();

        // Obtiene los dispositivos
        var result = await Access.Inventory.Controllers.OpenStore.Payments(Session.Instance.Token, Contexto?.Inventory.Id ?? 0);

        // Nuevos estados.
        IsLoading = false;
        Response = result;

        if (Contexto != null)
            Contexto.Payments = Response;

        StateHasChanged();
    }



    /// <summary>
    /// Renderizar.
    /// </summary>
    public void Render()
    {
        InvokeAsync(() =>
        {
            StateHasChanged();
        });
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
    /// <param name="e">Model.</param>
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
        nav.NavigateTo($"/inflows/{Contexto?.Inventory.Id}");
    }



    /// <summary>
    /// Abrir integrantes.
    /// </summary>
    void GoMembers()
    {
        nav.NavigateTo($"/members/{Contexto?.Inventory.Id}");
    }



    /// <summary>
    /// Abrir las salidas.
    /// </summary>
    void GoSalidas()
    {
        nav.NavigateTo($"/outflows/{Contexto?.Inventory.Id}");
    }



    /// <summary>
    /// Abrir crear.
    /// </summary>
    void GoCreate()
    {
        nav.NavigateTo($"/new/product/{Contexto?.Inventory.Id}");
    }

    /// <summary>
    /// Abrir crear.
    /// </summary>
    void GoOpenStore()
    {
        nav.NavigateTo($"/openStore/{Contexto?.Inventory.Id}");
    }

}
