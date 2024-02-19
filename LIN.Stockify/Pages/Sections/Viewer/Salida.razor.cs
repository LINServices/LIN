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
using LIN.Pages.Sections.Movements;

namespace LIN.Pages.Sections.Viewer;

public partial class Salida
{


    /// <summary>
    /// Lista de modelos
    /// </summary>
    private OutflowDataModel? Modelo { get; set; } = new();




    protected override Task OnInitializedAsync()
    {

        Modelo = Salidas.Selected;
        return base.OnInitializedAsync();
    }


}
