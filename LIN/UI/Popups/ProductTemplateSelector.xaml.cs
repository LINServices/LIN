namespace LIN.UI.Popups;


/// <summary>
/// Selector de un usuario
/// </summary>
public partial class ProductTemplateSelector : Popup
{


    /// <summary>
    /// Elemento seleccionado
    /// </summary>
    private ProductTemplate? SelectedItem = null;



    /// <summary>
    /// Constructor
    /// </summary>
    public ProductTemplateSelector()
    {
        InitializeComponent();
        this.CanBeDismissedByTappingOutsideOfPopup = false;
    }




    /// <summary>
    /// Busca un usuario
    /// </summary>
    private async void Buscar(string pattern)
    {

        // Prepara la vista
        content.Clear();
        indicador.Show();
        displayInfo.Hide();

        if (pattern.Trim().Length <= 0)
        {
            indicador.Hide();
            displayInfo.Show();
            displayInfo.Text = $"Ingresa un parametro valido";
            return;
        }

        // Encuentra el usuario
        var user = await Access.Inventory.Controllers.Product.ReadTemplates(pattern);


        // Analisis de respuesta
        switch (user.Response)
        {
            case Responses.Success:
                break;

            case Responses.InvalidUser:
                indicador.Hide();
                displayInfo.Show();
                displayInfo.Text = $"Hubo un error con tu cuenta.";
                return;

            case Responses.InvalidParam:
                indicador.Hide();
                displayInfo.Show();
                displayInfo.Text = $"El texto es invalido";
                return;

            case Responses.NotRows:
                indicador.Hide();
                displayInfo.Show();
                displayInfo.Text = $"No se encontraron resultados para '{pattern}'";
                return;

            default:
                indicador.Hide();
                displayInfo.Show();
                displayInfo.Text = $"Hubo un error";
                return;

        }


        // Renderiza una lista de modelos
        RenderModels(user.Models);

        // Vista
        indicador.Hide();
        displayInfo.Show();
        displayInfo.Text = $"Resultados para '{pattern}'";

    }



    /// <summary>
    /// Renderiza una lista de modelos
    /// </summary>
    private void RenderModels(List<ProductModel> models)
    {
        // Recorre los modelos
        foreach (var item in models)
        {

            // Carga el modelo a la vista
            var control = new Controls.ProductTemplate(item)
            {
                Margin = new(0, 5, 0, 0)
            };

            // Evento click sobre el control de Pick
            control.Clicked += PickItemClick!;


            if (item.Plantilla == SelectedItem?.Modelo.Plantilla)
            {
                control.Select();
            }

            // Agrega el control a la vista
            content.Add(control);

        }
    }



    /// <summary>
    /// Evento click sobre Pick
    /// </summary>
    private void PickItemClick(object sender, EventArgs e)
    {

        // Control
        ProductTemplate control = (ProductTemplate)sender;


        if (control.Modelo.Plantilla == SelectedItem?.Modelo.Plantilla)
        {
            control.UnSelect();
            SelectedItem = null;
            return;
        }

        SelectedItem?.UnSelect();
        control.Select();
        SelectedItem = control;

    }



    /// <summary>
    /// Boton de cancelar
    /// </summary>
    private void BtnCancelClick(object sender, EventArgs e)
    {
       // this.Close(new UserDataModel());
    }



    /// <summary>
    /// Boton de aceptar
    /// </summary>
    private void BtnSelectClick(object sender, EventArgs e)
    {
        if (SelectedItem == null)
        {
            displayInfo.Text = "Debes seleccionar una plantilla";
            return;
        }

        this.Close(SelectedItem.Modelo);
    }



    /// <summary>
    /// Boton de buscar
    /// </summary>
    private void ButtonBuscarClick(object sender, EventArgs e)
    {
        Buscar(buscador.Text ?? "");
    }




}