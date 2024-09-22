using LIN.Access.Inventory.Controllers;

namespace LIN.Components.Pages;

public partial class Inventory
{

    /// <summary>
    /// Esta cargando.
    /// </summary>
    private bool IsLoading = false;


    /// <summary>
    /// Instancia.
    /// </summary>
    public static Inventory? Instance { get; set; }


    /// <summary>
    /// Respuesta.
    /// </summary>
    private static ReadAllResponse<InventoryDataModel>? Response { get; set; } = null;


    /// <summary>
    /// Evento al inicializar.
    /// </summary>
    protected override void OnInitialized()
    {
        Instance = this;
        GetData();
        base.OnInitialized();
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
        var result = await Inventories.ReadAll(Session.Instance.Token);

        // Render.
        foreach (var item in result.Models)
            InventoryManager.PostAndReplace(item);

        // Nuevos estados.
        IsLoading = false;
        Response = result;
        StateHasChanged();
    }


    /// <summary>
    /// Abrir un producto.
    /// </summary>
    /// <param name="e">Modelo.</param>
    private void Go(InventoryDataModel? e)
    {
        if (e == null)
            return;

        NavigationManager.NavigateTo($"/products/{e.ID}");
    }


    /// <summary>
    /// Evento al renderizar.
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
    /// Abrir pantalla de crear.
    /// </summary>
    private void GoCreate()
    {
        NavigationManager.NavigateTo("/new/inventory");
    }


    /// <summary>
    /// Limpiar.
    /// </summary>
    public static void Clean()
    {
        Response = null;
    }


    /// <summary>
    /// Agregar modelo.
    /// </summary>
    /// <param name="model">Modelo de inventario.</param>
    /// 
    public void AddData(InventoryDataModel model)
    {
        if (Response?.Response != Responses.Success)
            return;

        InventoryManager.Post(model);
        Response.Models.Add(model);

    }

}