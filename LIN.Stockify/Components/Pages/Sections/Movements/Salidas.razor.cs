using LIN.Access.Inventory.Controllers;


namespace LIN.Components.Pages.Sections.Movements;


public partial class Salidas : IOutflow, IDisposable
{


    /// <summary>
    /// Id del inventario.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;



    /// <summary>
    /// Esta cargando.
    /// </summary>
    private bool IsLoading = false;



    /// <summary>
    /// Contexto del inventario.
    /// </summary>
    Services.Models.InventoryContextModel? Contexto { get; set; }



    /// <summary>
    /// Respuesta.
    /// </summary>
    private ReadAllResponse<OutflowDataModel>? Response { get; set; } = null;



    /// <summary>
    /// Salida seleccionada.
    /// </summary>
    public static OutflowDataModel? Selected { get; set; } = null;





    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {

        // Obtener el contexto.
        Contexto = Services.InventoryContext.Get(int.Parse(Id));

        OutflowObserver.Add(Contexto?.Inventory.ID ?? 0, this);

        // Evaluar el contexto.
        if (Contexto != null)
            Response = Contexto.Outflows;

        // Evaluar la respuesta.
        if (Response == null)
            GetData();

        // Base.
        base.OnParametersSet();
    }





    /// <summary>
    /// Obtener la data.
    /// </summary>
    /// <param name="force">Es forzado.</param>
    private async void GetData(bool force = false)
    {

        // Validación.
        if ((!force && (Response != null)) || IsLoading)
            return;

        // Cambiar el estado.
        IsLoading = true;
        StateHasChanged();

        // Obtiene los dispositivos
        var result = await Outflows.ReadAll(Contexto?.Inventory.ID ?? 0, Session.Instance.Token);

        // Nuevos estados.
        IsLoading = false;
        Response = result;

        if (Contexto != null)
            Contexto.Outflows = Response;


        StateHasChanged();
    }


    /// <summary>
    /// Renderizar.
    /// </summary>
    public void Render()
    {
        InvokeAsync(StateHasChanged);
    }



    /// <summary>
    /// Evento Dispose.
    /// </summary>
    public void Dispose()
    {
        OutflowObserver.Remove(this);
    }



    /// <summary>
    /// Evento después de renderizar.
    /// </summary>
    protected override void OnAfterRender(bool firstRender)
    {
        MainLayout.Configure(new()
        {
            OnCenterClick = GoNew,
            Section = 1,
            DockIcon = 0
        });

        base.OnAfterRender(firstRender);
    }



    /// <summary>
    /// Abrir salida.
    /// </summary>
    /// <param name="e"></param>
    void Go(Types.Inventory.Models.OutflowDataModel e)
    {
        Selected = e;
        nav.NavigateTo($"/outflow/{e.ID}");
    }



    /// <summary>
    /// Abrir new.
    /// </summary>
    void GoNew()
    {
        nav.NavigateTo($"/new/outflow/{Contexto?.Inventory.ID}");
    }


}