using LIN.Access.Inventory.Controllers;
using LIN.Inventory.Realtime.Manager.Models;
using LIN.Types.Inventory.Enumerations;

namespace LIN.Components.Pages.Sections;

public partial class Settings
{


    string user = string.Empty;
    string password = string.Empty;
    string tokenMercado = string.Empty;

    bool Mode = false;

    /// <summary>
    /// Contexto del inventario.
    /// </summary>
    OpenStoreSettings SettingsA { get; set; } = new();

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
    private readonly List<Types.Cloud.Identity.Abstracts.SessionModel<Types.Inventory.Models.ProfileModel>> Participantes = new();




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
    InventoryContext? InventoryContext { get; set; }


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

        InventoryContext = InventoryManager.Get(int.Parse(Id));


        if (InventoryContext == null)
            return;

        Name = InventoryContext.Inventory.Name;
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
        if (!force && Response != null || IsLoading)
            return;

        IsLoading = true;
        StateHasChanged();

        var response = await InventoryAccess.GetMembers(int.Parse(Id), Session.Instance.Token, Session.Instance.AccountToken);

        IsLoading = false;
        StateHasChanged();

        // Rellena los items
        Response = response;
        StateHasChanged();


        // Obtener la información de OpenStore.
        var ss = await Access.Inventory.Controllers.OpenStore.Read(Session.Instance.Token, int.Parse(Id));

        InventoryContext.Inventory.OpenStoreSettings = ss.Model;

        if (ss.Response == Responses.NotRows)
        {
            Mode = true;
        }
        else if (ss.Response != Responses.Success)
        {
            Mode = false;
        }
        else
        {
            SettingsA = ss.Model;
            Mode = false;
        }

        StateHasChanged();

    }



    async void Save()
    {

        if (Participantes.Count > 0)
            await SaveParticipants();

        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description))
            return;


        var response = await Inventories.Update(int.Parse(Id), Name, Description, Session.Instance.Token);


        if (InventoryContext == null || response.Response != Responses.Success)
            return;

        InventoryContext.Inventory.Name = Name;
        InventoryContext.Inventory.Direction = Description;
        StateHasChanged();
    }



    async Task SaveParticipants()
    {


        foreach (var e in Participantes)
        {
            var model = new InventoryAccessDataModel
            {
                InventoryId = int.Parse(Id),
                ProfileId = e.Profile.Id,
                Rol = InventoryRoles.Member
            };
            await InventoryAccess.Create(model, Session.Instance.Token);
        }


    }


    void OpenMember(IntegrantDataModel member)
    {
        MemberPopup.Show(member);
    }



    void OnDelete(int id)
    {
        InvokeAsync(() =>
        {
            Response?.Models.RemoveAll(t => t.AccessId == id);
            StateHasChanged();
        });

    }


    CreateResponse? ResponseToken;


    async Task Create()
    {

        ResponseToken = new(Responses.IsLoading);
        StateHasChanged();
        var create = await LIN.Access.Inventory.Controllers.OpenStore.Create(Session.Instance.Token, tokenMercado, int.Parse(Id), user, password);

        if (create.Response == Responses.Success)
        {
            await RetrieveData(true);
            return;
        }
        ResponseToken = create;
        StateHasChanged();
    }

    void BackCreate()
    {
        ResponseToken = null;
        StateHasChanged();
    }

}