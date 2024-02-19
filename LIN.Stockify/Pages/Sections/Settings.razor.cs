using LIN.Access.Inventory.Controllers;
using LIN.Access.Inventory.Hubs;
using LIN.Access.Inventory;
using LIN.Types.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace LIN.Pages.Sections;

public partial class Settings
{


    /// <summary>
    /// Lista de modelos
    /// </summary>
    private List<IntegrantDataModel>? Modelos { get; set; } = null;








    protected override Task OnInitializedAsync()
    {
        Reload();
        return base.OnInitializedAsync();
    }



    /// <summary>
    /// Operacion de cargar
    /// </summary>
    public async void Reload()
    {

        // Rellena los datos
        var dataRes = await RetrieveData();

        StateHasChanged();
        // Comprueba si se rellenaron los datos
        switch (dataRes)
        {
            // Correcto
            case Responses.Success:
                break;

            // Sin permisos
            case Responses.Unauthorized:
                return;

            default:
                return;
        }


    }



    /// <summary>
    /// Obtiene informaci�n desde el servidor
    /// </summary>
    private async Task<Responses> RetrieveData()
    {

        //var response = await Inventories.GetIntegrants(Inventory.Select.ID, Session.Instance.Token, Session.Instance.AccountToken);

        //// An�lisis de respuesta
        //if (response.Response != Responses.Success)
        //    return response.Response;

        //// Rellena los items
        //Modelos = response.Models;

        return Responses.Success;

    }

}
