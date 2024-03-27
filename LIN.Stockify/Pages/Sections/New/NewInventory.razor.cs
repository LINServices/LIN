using LIN.Access.Inventory.Controllers;
using LIN.Services;

namespace LIN.Pages.Sections.New;


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
            Creador = Session.Instance.Informacion.ID
        };


        List<int> notificationList = [];

        // Selected
        {

            // Acceso del usuario creador.
            modelo.UsersAccess.Add(new()
            {

                ProfileID = Session.Instance.Informacion.ID
            });

            // Otros participantes
            foreach (var user in Participantes)
            {
                notificationList.Add(user.Profile.ID);
                modelo.UsersAccess.Add(new()
                {
                    ProfileID = user.Profile.ID,
                    Rol = InventoryRoles.Member
                });
            }
        }

        // Respuesta del controlador
        var response = await Inventories.Create(modelo, Session.Instance.Token);

        // Correcto.
        if (response.Response == Responses.Success)
        {
            _ = Notification(response.LastID);
            section = 1;
            StateHasChanged();
            await Task.Delay(3000);
            nav.NavigateTo("/inventory");
            return;
        }

        // Sección.
        section = 0;
        StateHasChanged();

    }


    /// <summary>
    /// Notificar a los nuevos integrantes.
    /// </summary>
    /// <param name="id">Id.</param>
    private static async Task Notification(int id)
    {
        // Si es null.
        if (Realtime.InventoryAccessHub == null)
            return;

        await Realtime.InventoryAccessHub.Notification(id);
    }


}