
using LIN.Access.Inventory.Controllers;
using LIN.Types.Contacts.Models;
using LIN.UI.Popups;

namespace LIN.UI.Views.Products;

public partial class Add : ContentPage
{

    /// <summary>
    /// HUB de conexion
    /// </summary>
    InventoryAccessHub? HubConnection { get; set; }


    bool WithPlantilla { get; set; } = false;


    ProductModel Plantilla { get; set; }


    /// <summary>
    /// Id del inventario asociado
    /// </summary>
    public int InventoryID { get; set; }




    /// <summary>
    /// Constructor
    /// </summary>
    public Add(int idInventory, InventoryAccessHub? hubonnection)
    {
        InitializeComponent();
        this.HubConnection = hubonnection;
        this.InventoryID = idInventory;
    }




    /// <summary>
    /// Comprueba si los datos estan completos
    /// </summary>
    private bool IsDataComplete()
    {

        // Name
        if (string.IsNullOrWhiteSpace(txtName.Text))
        {

            ShowInfo("Por favor, completa el nombre.");
            btn.Show();
            return false;
        }

        // Categoria
        if (inpCategoria.SelectedItem == null)
        {
            ShowInfo("Por favor, selecciona una categoría.");
            btn.Show();
            return false;
        }

        // Proveedor
        if (contacto.Modelo.Id <= 0)
        {

            ShowInfo("Por favor, selecciona un proveedor.");
            btn.Show();
            return false;
        }

        // Precios
        if (string.IsNullOrWhiteSpace(txtPrecioCompra.Text) || string.IsNullOrWhiteSpace(txtPrecioVenta.Text))
        {
            ShowInfo("Por favor, completa los precios.");
            btn.Show();
            return false;
        }

        // Descripcion
        if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
        {
            ShowInfo("Por favor, agrega una descripción.");
            btn.Show();
            return false;
        }


        return true;

    }



    /// <summary>
    /// Evento Click sobre el selector de contactos
    /// </summary>
    private async void ContactMini_Clicked(object sender, EventArgs e)
    {
        // Nuevo popup
        var pop = new ContactSelector(false);
        var model = (List<ContactModel>?)await this.ShowPopupAsync(pop);

        if (model == null || model.Count <= 0)
            return;

        contacto.Modelo = model[0];
        contacto.LoadModelVisible();
    }



    /// <summary>
    /// Muestra un mensaje
    /// </summary>
    private void ShowInfo(string message)
    {
        lbInfo.Show();
        lbInfo.Text = message ?? "";
        indicador.Hide();
    }



    /// <summary>
    /// Evento click sobre el boton de crear
    /// </summary>
    private async void Button_Clicked(object sender, EventArgs e)
    {

        // Organiza la vista
        lbInfo.Hide();
        btn.Hide();
        indicador.Show();


        // Si los datos estan incompletos
        var isComplete = IsDataComplete();

        // Retorna
        if (!isComplete)
        {
            return;
        }


        // Valida si los precios son validos
        if ((!decimal.TryParse(txtPrecioVenta.Text, out decimal precioVenta)) | (!decimal.TryParse(txtPrecioCompra.Text, out decimal precioCompra)))
        {
            ShowInfo("Los precios no son validos");
            btn.Show();
            return;
        }


        // Obtiene la imagen
        var imagen = Task.Run(Array.Empty<byte>);

        if (inputImage.IsChanged)
            imagen = inputImage.GetBytes();


        // Model
        ProductModel modelo;
        if (WithPlantilla)
        {
            modelo = new()
            {
                Plantilla = Plantilla.Plantilla,
                Inventory = InventoryID,
                Provider = contacto.Modelo.Id,
                Estado = ProductBaseStatements.Normal,
                PrecioCompra = precioCompra,
                PrecioVenta = precioVenta,
                Quantity = cantidad.Value
            };
        }
        else
        {
            modelo = new()
            {
                Name = txtName.Text,
                Category = TextShortener.ToProductCategory((string)inpCategoria.SelectedItem ?? ""),
                Image = await imagen,
                Inventory = InventoryID,
                Code = txtCode.Text,
                Description = txtDescripcion.Text,
                Provider = contacto.Modelo.Id,
                Estado = ProductBaseStatements.Normal,
                PrecioCompra = precioCompra,
                PrecioVenta = precioVenta,
                Quantity = cantidad.Value
            };
        }






        // Respuesta del controlador
        var response = await Access.Inventory.Controllers.Product.Create(modelo, Session.Instance.Token);

        // Organización de la interfaz
        indicador.Hide();
        btn.Show();


        switch (response.Response)
        {
            case Responses.Success:
                break;
            case Responses.Unauthorized:
                ShowInfo("No tienes permisos para modificar este inventario.");
                return;
            default:
                ShowInfo("Hubo un error al agregar el producto.");
                return;

        }


        // Actualizacion en tiempo real
        //HubConnection?.SendAddModel(InventoryID, response.LastID);

        // Muestra el popup de agregado
        await this.ShowPopupAsync(new Popups.DefaultPopup());

    }


    private async void inputImage_ImageChanged(object sender, LIN.Controls.Events.ImageChanged e)
    {
        var bits = await inputImage.GetFileBytes();
        var ss = await IA.IAVision(bits);

        if (ss.Response != Responses.Success)
        {
            inpCategoria.SelectedIndex = 0;
            return;
        }

        var re = ss.Model;

        SelectCategorie(re);
    }


    void SelectCategorie(ProductCategories value)
    {
        switch (value)
        {

            case ProductCategories.Agricultura:
                inpCategoria.SelectedIndex = 10;
                break;
            case ProductCategories.Alimentos:
                inpCategoria.SelectedIndex = 5;
                break;
            case ProductCategories.Arte:
                inpCategoria.SelectedIndex = 12;
                break;
            case ProductCategories.Frutas:
                inpCategoria.SelectedIndex = 13;
                break;
            case ProductCategories.Animales:
                inpCategoria.SelectedIndex = 14;
                break;
            case ProductCategories.Automóviles:
                inpCategoria.SelectedIndex = 11;
                break;
            case ProductCategories.Deporte:
                inpCategoria.SelectedIndex = 2;
                break;
            case ProductCategories.Hogar:
                inpCategoria.SelectedIndex = 8;
                break;
            case ProductCategories.Juguetes:
                inpCategoria.SelectedIndex = 9;
                break;
            case ProductCategories.Limpieza:
                inpCategoria.SelectedIndex = 6;
                break;
            case ProductCategories.Moda:
                inpCategoria.SelectedIndex = 3;
                break;
            case ProductCategories.Salud:
                inpCategoria.SelectedIndex = 4;
                break;
            case ProductCategories.Servicios:
                inpCategoria.SelectedIndex = 7;
                break;
            case ProductCategories.Tecnología:
                inpCategoria.SelectedIndex = 1;
                break;
            default:
                inpCategoria.SelectedIndex = 0;
                break;

        }
    }


    private async void SelectPlantilla(object sender, EventArgs e)
    {

        var pop = new Popups.ProductTemplateSelector();
        var result = await pop.Show();

        if (result is not null && result is ProductModel)
        {
            txtName.Text = ((ProductModel)result).Name;
            txtDescripcion.Text = ((ProductModel)result).Description;
            SelectCategorie(((ProductModel)result).Category);

            PrepareUnableState();
            WithPlantilla = true;
            displayPlantilla.Show();
            Plantilla = (ProductModel)result;
        }

    }



    private void PrepareUnableState()
    {
        txtName.IsEnabled = false;
        txtName.ReadOnly = true;
        txtDescripcion.IsEnabled = false;
        inpCategoria.IsEnabled = false;

    }


    private void PrepareEnableState()
    {
        txtName.IsEnabled = true;
        txtName.ReadOnly = false;
        txtDescripcion.IsEnabled = true;
        inpCategoria.IsEnabled = true;
    }

    private void Label_Clicked(object sender, EventArgs e)
    {
        PrepareEnableState();
        WithPlantilla = true;
        displayPlantilla.Hide();
    }
}