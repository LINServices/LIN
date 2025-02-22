using LIN.Access.Inventory.Controllers;
using LIN.Types.Inventory.Enumerations;

namespace LIN.Components.Pages.Sections.New;

public partial class NewInventory
{

    /// <summary>
    /// Nombre.
    /// </summary>
    private string Name { get; set; } = string.Empty;


    /// <summary>
    /// Dirección.
    /// </summary>
    private string Direction { get; set; } = string.Empty;


    /// <summary>
    /// Sección.
    /// </summary>
    private int section = 0;


    /// <summary>
    /// Drawer de integrantes.
    /// </summary>
    private DrawerPeople Drawer = null!;


    /// <summary>
    /// Lista de participantes
    /// </summary>
    private readonly List<Types.Cloud.Identity.Abstracts.SessionModel<LIN.Types.Inventory.Models.ProfileModel>> Participantes = new();


    /// <summary>
    /// Crear inventario.
    /// </summary>
    private async void Create()
    {

        // Sección.
        section = 3;
        StateHasChanged();

        // Creación del modelo
        var modelo = new InventoryDataModel()
        {
            Nombre = Name,
            Direction = Direction,
            Creador = Session.Instance.Information.Id
        };


        List<int> notificationList = [];

        // Selected
        {

            // Acceso del usuario creador.
            modelo.UsersAccess.Add(new()
            {

                ProfileId = Session.Instance.Information.Id
            });

            // Otros participantes
            foreach (var user in Participantes)
            {
                notificationList.Add(user.Profile.Id);
                modelo.UsersAccess.Add(new()
                {
                    ProfileId = user.Profile.Id,
                    Rol = InventoryRoles.Member
                });
            }
        }

        // Respuesta del controlador
        var response = await Inventories.Create(modelo, Session.Instance.Token);

        // Correcto.
        if (response.Response != Responses.Success)
        {
            // Sección.
            section = 0;
            StateHasChanged();
        }

        var x = await LIN.Access.Inventory.Controllers.Inventories.Read(response.LastID, Session.Instance.Token);





        if (x.Response == Responses.Success)
        {
            Pages.Inventory.Instance?.AddData(x.Model);
        }



        section = 1;
        StateHasChanged();
        await Task.Delay(3000);


        NavigationManager.NavigateTo("/inventory");
        return;

    }

}