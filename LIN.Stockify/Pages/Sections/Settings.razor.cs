using LIN.Access.Inventory.Controllers;

namespace LIN.Pages.Sections;

public partial class Settings
{

    /// <summary>
    /// Parámetro Id.
    /// </summary>

    [Parameter]
    public string Id { get; set; } = string.Empty;



    /// <summary>
    /// Esta cargando.
    /// </summary>
    private bool IsLoading = false;



    /// <summary>
    /// Lista de modelos.
    /// </summary>
    private ReadAllResponse<IntegrantDataModel>? Response { get; set; } = null;



    /// <summary>
    /// Evento al inicializar.
    /// </summary>
    protected override Task OnInitializedAsync()
    {
        Reload();
        return base.OnInitializedAsync();
    }



    /// <summary>
    /// Operación de cargar.
    /// </summary>
    public async void Reload()
    {
        // Rellena los datos
        await RetrieveData();
    }



    /// <summary>
    /// Obtiene información desde el servidor
    /// </summary>
    private async Task RetrieveData(bool force = false)
    {

        // Validación.
        if ((!force && (Response != null)) || IsLoading)
            return;

        IsLoading = true;
        StateHasChanged();

        var response = await InventoryAccess.GetMembers(int.Parse(Id), Session.Instance.Token, Session.Instance.AccountToken);

        IsLoading = false;
        StateHasChanged();

        // Rellena los items
        Response = response;
        StateHasChanged();

    }


}