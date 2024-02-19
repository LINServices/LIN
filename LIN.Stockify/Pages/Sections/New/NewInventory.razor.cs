using LIN.Access.Inventory;
using LIN.Access.Inventory.Controllers;
using LIN.Access.Inventory.Hubs;
using LIN.Types.Inventory.Enumerations;
using LIN.Types.Inventory.Models;
using System.Xml.Linq;

namespace LIN.Pages.Sections.New;

public partial class NewInventory
{



    async void Create()
    {

        section = 3;
        StateHasChanged();

        // Creacion del modelo
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


        if (response.Response == Responses.Success)
        {
            section = 1;
            StateHasChanged();
            await Task.Delay(4000);
            nav.NavigateTo("/inventory");
            return;
        }

        section = 0;

        StateHasChanged();


    }



}