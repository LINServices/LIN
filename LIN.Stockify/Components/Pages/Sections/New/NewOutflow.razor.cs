using LIN.Inventory.Realtime.Manager.Models;
using LIN.Types.Inventory.Enumerations;

namespace LIN.Components.Pages.Sections.New;

public partial class NewOutflow
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
    /// Sección actual.
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
    /// Valores
    /// </summary>
    private Dictionary<int, int> Values = [];


    /// <summary>
    /// Drawer de productos.
    /// </summary>
    private DrawerProducts DrawerProducts { get; set; } = null!;


    /// <summary>
    /// Contexto del inventario.
    /// </summary>
    private InventoryContext? Contexto { get; set; }


    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {

        // Obtener el contexto.
        Contexto = InventoryManager.Get(int.Parse(Id));

        // Base.
        base.OnParametersSet();
    }


    /// <summary>
    /// Obtener valor.
    /// </summary>
    private int GetValue(int product)
    {
        Values.TryGetValue(product, out var value);
        return value;
    }

    private string ErrorMessage = "";


    /// <summary>
    /// Crear.
    /// </summary>
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
                    Id = control.DetailModel?.Id ?? 0
                },
                ProductDetailId = control.DetailModel?.Id ?? 0
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
            Date = Date,
            Type = type,
            Inventory = new()
            {
                ID = Contexto?.Inventory?.ID ?? 0
            },
            InventoryId = Contexto?.Inventory?.ID ?? 0,
            ProfileID = Session.Instance.Information.Id
        };


        // Envía al servidor
        var response = await Access.Inventory.Controllers.Outflows.Create(entry, LIN.Access.Inventory.Session.Instance.Token);


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

        await Task.Delay(2000);
        section = 0;
        StateHasChanged();

    }



    /// <summary>
    /// Cambio del valor.
    /// </summary>
    private void ValueChange(int product, int q)
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

    private void GoNormal()
    {
        section = 0;
        StateHasChanged();
    }




}