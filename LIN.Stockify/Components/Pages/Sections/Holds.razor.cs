using LIN.Inventory.Realtime.Manager.Models;
using System.Threading.Tasks;

namespace LIN.Components.Pages.Sections;
public partial class Holds
{

    /// <summary>
    /// Id del inventario de contexto.
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
    InventoryContext? Contexto { get; set; }

    /// <summary>
    /// Popup de eliminar.
    /// </summary>
    private DeletePopup DeletePopup = null!;

    /// <summary>
    /// Respuesta.
    /// </summary>
    private ReadAllResponse<HoldModel>? Response { get; set; } = null;

    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {
        // Obtener el contexto.
        Contexto = InventoryManager.Get(int.Parse(Id));
        GetData();
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
        var result = await Access.Inventory.Controllers.Holds.ReadAll(Contexto?.Inventory?.Id ?? 0, Session.Instance.Token);

        // Nuevos estados.
        IsLoading = false;
        Response = result;

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
    /// Abrir crear.
    /// </summary>
    void GoCreate()
    {
        nav.NavigateTo($"/new/product/{Contexto?.Inventory.Id}");
    }


    /// <summary>
    /// Realizar la devolución de una reserva.
    /// </summary>
    /// <param name="id">Id del grupo de reserva.</param>
    private async Task HoldBack(int id)
    {
        var response = await Access.Inventory.Controllers.Holds.Return(id, Session.Instance.Token);

        if (response.Response == Responses.Success)
        {
            Response?.Models.RemoveAll(t => t.GroupId == id);
            StateHasChanged();
        }
    }


    /// <summary>
    /// Mostrar el popup de eliminar.
    /// </summary>
    public void ShowDelete(int id)
    {
        DeletePopup.OnAccept = async () =>
        {
            await HoldBack(id);
        };

        DeletePopup.Show();
    }

}