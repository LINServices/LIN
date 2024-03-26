using LIN.Access.Inventory.Controllers;

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
    async void Create()
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


        List<int> notificationList = new();
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



}