using LIN.Access.Hubs;
using LIN.Shared.Responses;
using LIN.UI.Popups;

namespace LIN.UI.Views.Outflows;

public partial class AddItem : ContentPage
{

    ProductAccessHub? Hub;


    //====== Propiedades ======//

    /// <summary>
    /// Lista de controles
    /// </summary>
    private readonly List<Controls.ProductForPick> Models = new();


    /// <summary>
    /// Tipo de la entrada
    /// </summary>
    private LIN.Shared.Enumerations.OutflowsTypes Tipo = LIN.Shared.Enumerations.OutflowsTypes.None;



    public int Inventario { get; set; }



    /// <summary>
    /// Constructor
    /// </summary>
    public AddItem(int inventario, ProductAccessHub? hub)
    {
        InitializeComponent();
        Inventario = inventario;
        Hub = hub;
    }





    /// <summary>
    /// Click sobre el boton de agregar producto
    /// </summary>
    private async void EventAdd(object sender, EventArgs e)
    {

        // Nuevo popup
        var pop = new ProductSelector(Inventario, true);
        var models = (List<ProductDataTransfer>?)await this.ShowPopupAsync(pop);

        if (models == null || models.Count <= 0)
            return;



        foreach (var model in models)
        {
            // Si ya existia en la interfaz
            var contains = Models.Where(T => T.Modelo.ProductID == model.ProductID).Count();
            if (contains > 0)
            {
                await DisplayAlert("Advertencia", $"Ya agregaste '{model.Name}' a las lista.", "Continuar");
                return;
            }


            // Crea el nuevo control
            var control = new Controls.ProductForPick(model)
            {
                CounterVisible = true
            };

            // Muetra el control
            Models.Add(control);
            content.Add(control);
        }


    }



    /// <summary>
    /// Click sobre el boton de eliminar todo
    /// </summary>
    private void EventDelete(object sender, EventArgs e)
    {
        content.Clear();
        Models.Clear();
    }



    /// <summary>
    /// Envia la nueva entrada
    /// </summary>
    private async void EventSubmit(object sender, EventArgs e)
    {

        // Variables
        List<LIN.Shared.Models.OutflowDetailsDataModel> details = new();
        LIN.Shared.Models.OutflowDataModel outflow;


        // Prepara la vista
        indicador.Show();
        lbInfo.Hide();
        btn.Hide();
        await Task.Delay(1);


        // Si no hay ningun tipo
        if (Tipo == LIN.Shared.Enumerations.OutflowsTypes.None)
        {
            // Muestra el mensaje de error
            DisplayMessage("Debes selecionar un tipo de salida");
            return;
        }


        // Rellena los detalles
        foreach (var control in Models)
        {
            // Model
            OutflowDetailsDataModel model = new()
            {
                ProductoDetail = control.Modelo.IDDetail,
                Cantidad = control.GetCounterValue()
            };
            details.Add(model);
        }


        // Si no hay detalles
        if (details.Count <= 0)
        {
            DisplayMessage("Para agregar una salida deben haber minimo 1 producto");
            return;
        }


        // Model de entrada
        outflow = new()
        {
            Details = details,
            Date = DateTime.Now,
            Type = Tipo,
            Inventario = Inventario,
            Usuario = Sesion.Instance.Informacion.ID
        };


        // Envia al servidor
        var response = await Access.Inventory.Controllers.Outflows.CreateAsync(outflow);


        // Si hubo un error
        if (response.Response != Responses.Success)
        {
            DisplayMessage("Hubo un error al agregar la salida");
            return;
        }


        // Actualizacion en tiempo real
        Hub?.SendAddModelOutflow(Inventario, response.LastID);


        // Muestra el popup
        await this.ShowPopupAsync(new DefaultPopup());

        // Prepara la vista
        indicador.Hide();
        lbInfo.Hide();
        btn.Show();

    }



    /// <summary>
    /// Muestra un mensaje de error
    /// </summary>
    private void DisplayMessage(string? message)
    {
        indicador.Hide();

        if (message != null)
        {
            lbInfo.Show();
            lbInfo.Text = message ?? "";
        }
        else
        {
            lbInfo.Hide();
        }
        btn.Show();
    }





    /// <summary>
    /// EVENTO: Cuando cambia el item seleccionado de categoria
    /// </summary>
    private void inpCategoria_SelectedIndexChanged(object sender, EventArgs e)
    {


        Tipo = (string)inpCategoria.SelectedItem switch
        {
            // Compra
            "Consumo interno" => LIN.Shared.Enumerations.OutflowsTypes.Consumo,
            // Devolucion
            "Caducidad" => LIN.Shared.Enumerations.OutflowsTypes.Caducidad,
            // Regalo
            "Venta" => LIN.Shared.Enumerations.OutflowsTypes.Venta,
            // Ajuste
            "Fraude" => LIN.Shared.Enumerations.OutflowsTypes.Fraude,
            "Donacion" => LIN.Shared.Enumerations.OutflowsTypes.Donacion,
            "Perdida" => LIN.Shared.Enumerations.OutflowsTypes.Perdida,
            // Default
            _ => LIN.Shared.Enumerations.OutflowsTypes.None,
        };


    }


}