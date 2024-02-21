using LIN.Types.Inventory.Enumerations;
using LIN.Types.Inventory.Models;
using SILF.Script;

namespace LIN.Pages.Sections.New;

public partial class NewOutflow
{


    Services.Models.InventoryContextModel? Contexto { get; set; }



    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {

        // Obtener el contexto.
        Contexto = Services.InventoryContext.Get(int.Parse(Id));

        // Base.
        base.OnParametersSet();
    }



    int GetValue(int product)
    {
        try
        {
           return Values[product];
        }
        catch { }

        return 0;
    }

    private async void Create()
    {

        // Preparar la vista.
        section = 3;
        StateHasChanged();

        // Modelo.
        OutflowsTypes type = (OutflowsTypes)Category;

        // No tiene tipo.
        if (type == OutflowsTypes.None)
        {
            section = 0;
            StateHasChanged();
            return;
        }


        // Variables
        List<OutflowDetailsDataModel> details = new();
        OutflowDataModel entry;


        // Rellena los detalles
        foreach (var control in Selected ?? [])
        {

            Values.TryGetValue(control.Id, out int quantity);
            OutflowDetailsDataModel model = new()
            {
              Cantidad = quantity,
              ProductDetail = new()
              {
                  Id = control.DetailModel.Id
              },
              ProductDetailId = control.DetailModel.Id
            };
            details.Add(model);
        }


        // Si no hay detalles
        if (details.Count <= 0)
        {

            return;
        }


        // Model de salida
        entry = new()
        {
            Details = details,
            Date = new DateTime(date.Year, date.Month, date.Day),
            Type = type,
            Inventory = new()
            {
                ID = Contexto.Inventory.ID
            },
            InventoryId = Contexto.Inventory.ID,
            ProfileID = Session.Instance.Informacion.ID
        };


        // Envía al servidor
        var response = await Access.Inventory.Controllers.Outflows.CreateAsync(entry, LIN.Access.Inventory.Session.Instance.Token);


        // Si hubo un error
        if (response.Response != Responses.Success)
        {
            section = 2;
            StateHasChanged();
            await Task.Delay(2000);
            section = 0;
            return;
        }


        section = 1;
        StateHasChanged();

        _ = Services.Realtime.InventoryAccess.SendCommand(new()
        {
            Command = $"addOutflow({response.LastID}, true)"
        });

        await Task.Delay(2000);
        section = 0;
        StateHasChanged();

    }


}