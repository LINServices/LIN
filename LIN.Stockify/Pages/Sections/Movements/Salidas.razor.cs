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
using LIN.Services.RealTime;

namespace LIN.Pages.Sections.Movements;

public partial class Salidas: IOutflow, IDisposable
{


    [Parameter]
    public string Id { get; set; } = string.Empty;



    Services.Models.InventoryContextModel? Contexto { get; set; }



    /// <summary>
    /// Respuesta.
    /// </summary>
    private ReadAllResponse<OutflowDataModel>? Response { get; set; } = null;





    public static OutflowDataModel? Selected { get; set; } = null;


  


    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {

        // Obtener el contexto.
        Contexto = Services.InventoryContext.Get(int.Parse(Id));

        OutflowObserver.Add(Contexto.Inventory.ID, this);

        // Evaluar el contexto.
        if (Contexto != null)
            Response = Contexto.Outflows;

        // Evaluar la respuesta.
        if (Response == null)
            GetData();

        // Base.
        base.OnParametersSet();
    }





    bool IsLoading = false;

    private async void GetData(bool force = false)
    {

        // Validación.
        if ((!force && (Response != null)) || IsLoading)
            return;

        // Cambiar el estado.
        IsLoading = true;
        StateHasChanged();

        // Obtiene los dispositivos
        var result = await Outflows.ReadAll(Contexto?.Inventory.ID ?? 0, Session.Instance.Token);

        // Nuevos estados.
        IsLoading = false;
        Response = result;

        if (Contexto != null)
            Contexto.Outflows = Response;


        StateHasChanged();
    }


    public void Render()
    {
        InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }

    public void Dispose()
    {
        OutflowObserver.Remove(this);
    }




}
