using LIN.UI.Popups;

namespace LIN.UI.Views.Inflows;

public partial class AddItem : ContentPage
{

    //====== Propiedades ======//

    /// <summary>
    /// Lista de controles
    /// </summary>
    private readonly List<Controls.ProductForPick> Models = new();


    /// <summary>
    /// Tipo de la entrada
    /// </summary>
    private InflowsTypes Tipo = InflowsTypes.Undefined;



    readonly InventoryAccessHub? Hub = null;

    public int Inventario { get; set; }


    /// <summary>
    /// Constructor
    /// </summary>
    public AddItem(int inventario, InventoryAccessHub? hub)
    {
        InitializeComponent();
        this.Inventario = inventario;
        this.Hub = hub;
    }





    /// <summary>
    /// Click sobre el boton de agregar producto
    /// </summary>
    private async void EventAdd(object sender, EventArgs e)
    {

        // Nuevo popup
        var pop = new ProductSelector(Inventario, true);
        var models = (List<ProductModel>?)await this.ShowPopupAsync(pop);

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
        List<InflowDetailsDataModel> details = new();
        InflowDataModel entry;


        // Prepara la vista
        indicador.Show();
        lbInfo.Hide();
        btn.Hide();
        await Task.Delay(1);


        // Si no hay ningun tipo
        if (Tipo == InflowsTypes.Undefined)
        {
            // Muestra el mensaje de error
            DisplayMessage("Debes selecionar un tipo de entrada");
            return;
        }


        // Analisis
        var metrics = Analysis();
        if (!metrics)
        {
            DisplayMessage(null);
            return;
        }

        // Rellena los detalles
        foreach (var control in Models)
        {
           
            // Model
            InflowDetailsDataModel model = new()
            {
                ProductoDetail = control.Modelo.IDDetail,
                Cantidad = control.GetCounterValue()
            };
            details.Add(model);
        }


        // Si no hay detalles
        if (details.Count <= 0)
        {
            DisplayMessage("Para agregar una entrada deben haber minimo 1 producto");
            return;
        }


        // Model de entrada
        entry = new()
        {
            Details = details,
            Date = DateTime.Now,
            Type = Tipo,
            Inventario = Inventario,
            ProfileID = Session.Instance.Informacion.ID
        };


        // Envía al servidor
        var response = await Access.Inventory.Controllers.Inflows.Create(entry);


        // Si hubo un error
        if (response.Response != Responses.Success)
        {
            DisplayMessage("Hubo un error al agregar la entrada");
            return;
        }



        // Rellena los detalles
        foreach (var control in Models)
        {
            int newQ = control.Modelo.Quantity + control.GetCounterValue();
            ProductObserver.UpdateQuantity(control.Modelo.ProductID, newQ, From.SameDevice);
        }



        // Actualización en tiempo real
       // Hub?.SendAddModelInflow(Inventario, response.LastID);

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
    /// Comprueba si hay errores en las cantidades o otros datos
    /// </summary>
    private bool Analysis()
    {

        // Si el tipo es por ajuste
        if (Tipo == InflowsTypes.Ajuste)
            return true;

        // Rellena los detalles
        foreach (var control in Models)
        {
            int cantidad = control.GetCounterValue();
            if (cantidad <= 0)
            {
                DisplayAlert("Error", $"La cantidad de el producto '{control.Modelo.Name}' no puede ser igual a 0.", "Aceptar");
                return false;
            }
        }

        return true;
    }



    /// <summary>
    /// EVENTO: Cuando cambia el item seleccionado de categoria
    /// </summary>
    private void inpCategoria_SelectedIndexChanged(object sender, EventArgs e)
    {


        Tipo = (string)inpCategoria.SelectedItem switch
        {
            // Compra
            "Compra" => InflowsTypes.Compra,
            // Devolucion
            "Devolucion" => InflowsTypes.Devolucion,
            // Regalo
            "Regalo" => InflowsTypes.Regalo,
            // Ajuste
            "Ajuste" => InflowsTypes.Ajuste,
            // Default
            _ => InflowsTypes.Undefined,
        };


        if (Tipo == InflowsTypes.Ajuste)
        {
            DisplayAlert("Advertencia", "La función de \"entrada por ajuste\" remplaza el valor actual de las existencias con la cantidad ingresada en lugar de sumarla al valor actual.", "Aceptar");
        }



    }



}