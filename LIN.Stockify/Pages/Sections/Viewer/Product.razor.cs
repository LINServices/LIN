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

namespace LIN.Pages.Sections.Viewer;

public partial class Product
{


    /// <summary>
    /// Lista de modelos
    /// </summary>
    private ProductModel? Modelo { get; set; } = new();




    protected override Task OnInitializedAsync()
    {

        Modelo = Products.Selected;
        return base.OnInitializedAsync();
    }


}
