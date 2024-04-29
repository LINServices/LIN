using LIN.Inventory.Shared.Services.Models;
using static System.Collections.Specialized.BitVector32;

namespace LIN.Components.Pages.Sections.New;


public partial class NewInflow
{


    /// <summary>
    /// Id.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;



    /// <summary>
    /// Categoría.
    /// </summary>
    private int Category { get; set; }



    /// <summary>
    /// Sección.
    /// </summary>
    private int section = 0;



    /// <summary>
    /// Fecha.
    /// </summary>
    private DateTime Date { get; set; } = DateTime.Now;



    /// <summary>
    /// Productos seleccionados.
    /// </summary>
    private List<ProductModel> Selected { get; set; } = [];



    /// <summary>
    /// Valores.
    /// </summary>
    private Dictionary<int, int> Values { get; set; } = [];



    /// <summary>
    /// Drawer de productos.
    /// </summary>
    DrawerProducts DrawerProducts = null!;



    /// <summary>
    /// Contexto del inventario.
    /// </summary>
    private InventoryContextModel? Contexto { get; set; }



    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {

        // Obtener el contexto.
        Contexto = LIN.Inventory.Shared. Services.InventoryContext.Get(int.Parse(Id));

        // Base.
        base.OnParametersSet();
    }




    /// <summary>
    /// Obtener el valor.
    /// </summary>
    private int GetValue(int product)
    {
        Values.TryGetValue(product, out var value);
        return value;
    }


    string ErrorMessage="";

    /// <summary>
    /// Crear.
    /// </summary>
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
                    Id = control?.DetailModel?.Id ?? 0
                },
                ProductDetailId = control?.DetailModel?.Id ?? 0
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
            Date = Date,
            Type = type,
            Inventory = new()
            {
                ID = Contexto?.Inventory.ID ?? 0
            },
            InventoryId = Contexto?.Inventory.ID ?? 0,
            ProfileID = Session.Instance.Informacion.ID
        };


        // Envía al servidor
        var response = await Access.Inventory.Controllers.Inflows.Create(entry, LIN.Access.Inventory.Session.Instance.Token);



        switch (response.Response)
        {

            case Responses.Success:
                break;

            case Responses.Unauthorized:
                section = 2;
                ErrorMessage = "No tienes autorización para crear movimientos en este inventario.";
                StateHasChanged();
                return;

            default:
                section = 2;
                ErrorMessage = "Hubo un error al crear este movimiento.";
                StateHasChanged();
                return;
        }




        section = 1;
        StateHasChanged();

        _ = Services.Realtime.InventoryAccessHub.SendCommand(new()
        {
            Command = $"addInflow({response.LastID}, true)",
            Inventory = Contexto?.Inventory.ID ?? 0
        });

        await Task.Delay(2000);
        section = 0;
        StateHasChanged();

    }



    /// <summary>
    /// Obtener el valor.
    /// </summary>
    void ValueChange(int product, int q)
    {
        try
        {
            Values[product] = q;
        }
        catch (Exception)
        {
            Values.Add(product, q);
        }

    }


    void GoNormal()
    {
        section = 0;
        StateHasChanged();
    }

}