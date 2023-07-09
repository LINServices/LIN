using LIN.Access.Hubs;
using LIN.Shared.Responses;
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
    private LIN.Shared.Enumerations.InflowsTypes Tipo = LIN.Shared.Enumerations.InflowsTypes.Undefined;



    readonly ProductAccessHub? Hub = null;

    public int Inventario { get; set; }


    /// <summary>
    /// Constructor
    /// </summary>
    public AddItem(int inventario, ProductAccessHub? hub)
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
        List<LIN.Shared.Models.InflowDetailsDataModel> details = new();
        LIN.Shared.Models.InflowDataModel entry;


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
            LIN.Shared.Models.InflowDetailsDataModel model = new()
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
            Usuario = Sesion.Instance.Informacion.ID
        };


        // Envia al servidor
        var response = await LIN.Access.Controllers.Inflows.Create(entry);


        // Si hubo un error
        if (response.Response != Responses.Success)
        {
            DisplayMessage("Hubo un error al agregar la entrada");
            return;
        }


        // Actualizacion en tiempo real
        Hub?.SendAddModelInflow(Inventario, response.LastID);

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
        if (Tipo == LIN.Shared.Enumerations.InflowsTypes.Ajuste)
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
            "Compra" => LIN.Shared.Enumerations.InflowsTypes.Compra,
            // Devolucion
            "Devolucion" => LIN.Shared.Enumerations.InflowsTypes.Devolucion,
            // Regalo
            "Regalo" => LIN.Shared.Enumerations.InflowsTypes.Regalo,
            // Ajuste
            "Ajuste" => LIN.Shared.Enumerations.InflowsTypes.Ajuste,
            // Default
            _ => LIN.Shared.Enumerations.InflowsTypes.Undefined,
        };


        if (Tipo == LIN.Shared.Enumerations.InflowsTypes.Ajuste)
        {
            DisplayAlert("Advertencia", "La funci�n de \"entrada por ajuste\" remplaza el valor actual de las existencias con la cantidad ingresada en lugar de sumarla al valor actual.", "Aceptar");
        }



    }



}