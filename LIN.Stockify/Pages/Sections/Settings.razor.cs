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
    private ReadAllResponse<IntegrantDataModel>? Response { get; set; } = null;



    bool IsLoading = false;





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
        await RetrieveData();


    }



    /// <summary>
    /// Obtiene informaci�n desde el servidor
    /// </summary>
    private async Task RetrieveData(bool force = false)
    {

        // Validación.
        if ((!force && (Response != null)) || IsLoading)
            return;

        IsLoading = true;
        StateHasChanged();

        var response = await Inventories.GetIntegrants(int.Parse(Id), Session.Instance.Token, Session.Instance.AccountToken);

        IsLoading = false;
        StateHasChanged();

        // Rellena los items
        Response = response;
        StateHasChanged();

    }

}
