﻿using SILF.Script;

namespace LIN.Pages.Sections.Viewer;


public partial class Salida
{


    
    /// <summary>
    /// Id de la entrada.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;



    /// <summary>
    /// Modelo.
    /// </summary>
    private OutflowDataModel? Modelo { get; set; } = new();



    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        // Obtener el Contexto.
        var inventoryContext = (from context in Services.InventoryContext.Dictionary
                                where (context.Value.Outflows ?? new()).Models.Any(t => t.ID == int.Parse(Id))
                                select context.Value).FirstOrDefault();

        // Validar.
        if (inventoryContext == null)
        {
            // Obtener los detalles.
            var outflowDetails = await LIN.Access.Inventory.Controllers.Outflows.Read(int.Parse(Id), LIN.Access.Inventory.Session.Instance.Token, true);

            // Validar respuesta.
            if (outflowDetails.Response == Responses.Success)
                Modelo = outflowDetails.Model;

            return;
        }


        // Obtener la salida.
        var outflow = (from outflowModel in (inventoryContext.Outflows ?? new()).Models
                       where outflowModel.ID == int.Parse(Id)
                       select outflowModel).FirstOrDefault();

        // Si no hay detalles.
        if (outflow?.Details.Count <= 0)
        {
            // Obtener los detalles.
            var outflowDetails = await Access.Inventory.Controllers.Outflows.Read(outflow.ID, Session.Instance.Token, true);

            if (outflowDetails.Response == Responses.Success)
                outflow.Details = outflowDetails.Model.Details;
        }

        // Establecer el modelo.
        Modelo = outflow;
        await base.OnParametersSetAsync();
    }



    /// <summary>
    /// Evento al iniciar.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {

        MainLayout.Configure(new()
        {
            OnCenterClick = Send,
            Section = 1,
            DockIcon = 2
        });

        await base.OnInitializedAsync();
    }


    /// <summary>
    /// Enviar el comando al selector.
    /// </summary>
    void Send()
    {
        // Nuevo onInvoque.
        MainLayout.DevicesSelector.OnInvoke = (e) =>
        {
            Services.Realtime.InventoryAccessHub.SendToDevice(e.Id, new()
            {
                Command = $"viewOutflow({Modelo?.ID})"
            });
        };

        Components.Layout.MainLayout.DevicesSelector.Show();
    }



   


    public static void Show(int id)
    {
        MainLayout.Navigate($"/outflow/{id}");
    }

}
