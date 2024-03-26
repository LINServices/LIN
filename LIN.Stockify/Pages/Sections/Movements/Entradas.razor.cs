using LIN.Access.Inventory.Controllers;
using LIN.Components.Layout;
using LIN.Services.RealTime;
using LIN.Types.Inventory.Models;

namespace LIN.Pages.Sections.Movements;

public partial class Entradas : IInflow, IDisposable
{


    [Parameter]
    public string Id { get; set; } = string.Empty;



    Services.Models.InventoryContextModel? Contexto { get; set; }



    /// <summary>
    /// Respuesta.
    /// </summary>
    private ReadAllResponse<InflowDataModel>? Response => Contexto.Inflows;




    public static InflowDataModel? Selected { get; set; } = null;







    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {

        // Obtener el contexto.
        Contexto = Services.InventoryContext.Get(int.Parse(Id));

        InflowObserver.Add(Contexto.Inventory.ID, this);

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
        Contexto.Inflows = result;

        if (Contexto != null)
            Contexto.Inflows = Response;

        StateHasChanged();
    }


    public void Render()
    {
        InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }

    public void Dispose()
    {
        InflowObserver.Remove(this);
    }


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



    void Go(Types.Inventory.Models.InflowDataModel e)
    {
        Selected = e;
        nav.NavigateTo($"/inflow/{e.ID}");
    }

    void GoNew()
    {
        nav.NavigateTo($"/new/inflow/{Contexto?.Inventory.ID}");
    }






}
