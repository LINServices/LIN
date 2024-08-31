using LIN.Access.Inventory.Controllers;
using LIN.Inventory.Shared.Services.Models;
using LIN.Types.Inventory.Enumerations;

namespace LIN.Components.Pages.Sections;

public partial class Settings
{


    /// <summary>
    /// Drawer de integrantes.
    /// </summary>
    private DrawerPeople Drawer = null!;



    /// <summary>
    /// Popup del integrante.
    /// </summary>
    private MemberPopup MemberPopup { get; set; } = null!;



    /// <summary>
    /// Lista de participantes
    /// </summary>
    private readonly List<Types.Cloud.Identity.Abstracts.SessionModel<LIN.Types.Inventory.Models.ProfileModel>> Participantes = new();




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
    InventoryContextModel? InventoryContext { get; set; }


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

        InventoryContext = LIN.Inventory.Shared.Services.InventoryContext.Get(int.Parse(Id));


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

        if (Participantes.Count > 0)
            await SaveParticipants();

        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description))
            return;


        var response = await LIN.Access.Inventory.Controllers.Inventories.Update(int.Parse(Id), Name, Description, Session.Instance.Token);


        if (InventoryContext == null || response.Response != Responses.Success)
            return;

        InventoryContext.Inventory.Nombre = Name;
        InventoryContext.Inventory.Direction = Description;
        StateHasChanged();
    }



    async Task SaveParticipants()
    {


        foreach (var e in Participantes)
        {
            var model = new InventoryAcessDataModel
            {
                Inventario = int.Parse(Id),
                ProfileID = e.Profile.ID,
                Rol = InventoryRoles.Member
            };
            await LIN.Access.Inventory.Controllers.InventoryAccess.Create(model, Session.Instance.Token);
        }


    }


    void OpenMember(IntegrantDataModel member)
    {
        MemberPopup.Show(member);
    }



    void OnDelete(int id)
    {
        this.InvokeAsync(() =>
        {
            Response?.Models.RemoveAll(t => t.AccessID == id);
            StateHasChanged();
        });

    }


}