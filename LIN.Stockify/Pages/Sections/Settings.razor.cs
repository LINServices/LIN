using LIN.Access.Inventory.Controllers;

namespace LIN.Pages.Sections;

public partial class Settings
{

    /// <summary>
    /// Parámetro Id.
    /// </summary>

    [Parameter]
    public string Id { get; set; } = string.Empty;



    string Name = "";
    string Description = "";



    /// <summary>
    /// Esta cargando.
    /// </summary>
    private bool IsLoading = false;



    /// <summary>
    /// Lista de modelos.
    /// </summary>
    private ReadAllResponse<IntegrantDataModel>? Response { get; set; } = null;



    /// <summary>
    /// Contexto de inventario.
    /// </summary>
    Services.Models.InventoryContextModel? InventoryContext { get; set; }


    /// <summary>
    /// Evento al inicializar.
    /// </summary>
    protected override Task OnInitializedAsync()
    {
        MainLayout.Configure(new()
        {
            OnCenterClick = Save,
            Section = 1,
            DockIcon = 3
        });

        Reload();
        return base.OnInitializedAsync();
    }



    /// <summary>
    /// Operación de cargar.
    /// </summary>
    public async void Reload()
    {

        InventoryContext = Services.InventoryContext.Get(int.Parse(Id));


        if (InventoryContext == null)
            return;

        Name = InventoryContext.Inventory.Nombre;
        Description = InventoryContext.Inventory.Direction;

        // Rellena los datos
        await RetrieveData();

        StateHasChanged();
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



    async void Save()
    {


        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description))
            return;


        var response = await LIN.Access.Inventory.Controllers.Inventories.Update(int.Parse(Id), Name, Description, Session.Instance.Token);


        if (InventoryContext == null || response.Response != Responses.Success)
            return;

        InventoryContext.Inventory.Nombre = Name;
        InventoryContext.Inventory.Direction = Description;
        StateHasChanged();
    }



}