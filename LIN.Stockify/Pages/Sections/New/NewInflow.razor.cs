using LIN.Types.Inventory.Enumerations;
using LIN.Types.Inventory.Models;

namespace LIN.Pages.Sections.New;

public partial class NewInflow
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
        InflowsTypes type = (InflowsTypes)Category;

        // No tiene tipo.
        if (type == InflowsTypes.Undefined)
        {
            section = 0;
            StateHasChanged();
            return;
        }


        // Variables
        List<InflowDetailsDataModel> details = new();
        InflowDataModel entry;


        // Rellena los detalles
        foreach (var control in Selected ?? [])
        {
            Values.TryGetValue(control.Id, out int quantity);
            InflowDetailsDataModel model = new()
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


        // Model de entrada
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
        var response = await Access.Inventory.Controllers.Inflows.Create(entry, LIN.Access.Inventory.Session.Instance.Token);


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
            Command = $"addInflow({response.LastID}, true)",
            Inventory = Contexto.Inventory.ID
        });

        await Task.Delay(2000);
        section = 0;
        StateHasChanged();

    }




    [Parameter]
    public string Id { get; set; } = string.Empty;



    string Name { get; set; } = string.Empty;

    string Direction { get; set; } = string.Empty;

    int Category { get; set; }


    int section = 0;

    DateOnly date = DateOnly.FromDateTime(DateTime.Now);


    List<ProductModel> Selected = [];

    Dictionary<int, int> Values = [];


    DrawerProducts DrawerProducts = null!;



    void ValueChange(int product, int q)
    {
        try
        {
            Values[product] = q;

        }
        catch
        {
            Values.Add(product, q);
        }

    }


}