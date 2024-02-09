using LIN.Access.Inventory.Hubs;

namespace LIN.UI.Popups;


public partial class ProductEdit : Popup, IProductViewer
{

    /// <summary>
    /// Modelo del producto
    /// </summary>
    public ProductModel Modelo { get; set; }


    public string? ContextKey { get; init; } = "";

    InventoryAccessHub? Hub;


    /// <summary>
    /// Constructor
    /// </summary>
    public ProductEdit(ProductModel product, InventoryAccessHub? hub)
    {
        InitializeComponent();
        this.CanBeDismissedByTappingOutsideOfPopup = false;
        Modelo = product;
        Hub = hub;
        LoadData();
    }



    /// <summary>
    /// Carga los datos y los controles
    /// </summary>
    private void LoadData()
    {

        inputImage.SetImage(Modelo.Image, "caja.png");
        txtName.Text = Modelo.Name;
        txtPrecioCompra.Text = Modelo.DetailModel.PrecioCompra.ToString();
        txtPrecioVenta.Text = Modelo.DetailModel.PrecioVenta.ToString();
        txtDescripcion.Text = Modelo.Description;


        switch (Modelo.Category)
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
            case ProductCategories.Undefined:
                break;
            default:
                inpCategoria.SelectedIndex = 0;
                break;

        }


    }




    private void BtnCancelClick(object sender, EventArgs e)
    {
        this.Close(null);
    }

    private async void BtnSelectClick(object sender, EventArgs e)
    {

        // Obtiene la imagen
        var imagen = Task.Run(() => { return Array.Empty<byte>(); });
        if (inputImage.IsChanged)
            imagen = inputImage.GetBytes();


        bool detailChange = false;
        bool plantillaChange = false;



        // Plantilla cambio
        plantillaChange =
            !(Modelo.Category == TextShortener.ToProductCategory((string)inpCategoria.SelectedItem ?? "") &&
            Modelo.Name == txtName.Text &&
            Modelo.Code == "" &&
            Modelo.Description == txtDescripcion.Text &&
            !inputImage.IsChanged
            );


        // Valida si los precios son validos
        if ((!decimal.TryParse(txtPrecioVenta.Text, out decimal precioVenta)) | (!decimal.TryParse(txtPrecioCompra.Text, out decimal precioCompra)))
        {
            // MSG
            return;
        }

        // Detalles cambio
        detailChange = !(precioCompra == Modelo.DetailModel.PrecioCompra && precioVenta == Modelo.DetailModel.PrecioVenta);



        var newModel = new ProductModel()
        {
            Category = TextShortener.ToProductCategory((string)inpCategoria.SelectedItem ?? ""),
            Name = txtName.Text,
            Code = "",
            Description = txtDescripcion.Text,
            Statement = Modelo.Statement,
            ProductID = Modelo.ProductID,
            PrecioCompra = precioCompra,
            PrecioVenta = precioVenta,
            IDDetail = Modelo.IDDetail,
            Plantilla = Modelo.Plantilla,
            Quantity = Modelo.Quantity,
            Inventory = Modelo.Inventory
        };

        if (inputImage.IsChanged)
            newModel.Image = await imagen;
        else
            newModel.Image = Modelo.Image;


        scrollView.Hide();
        indi.Show();
        await Task.Delay(100);


        Task<ResponseBase>? task = null;

        if (detailChange & plantillaChange)
            task = Access.Inventory.Controllers.Product.Update(newModel);

        else if (plantillaChange)
            task = Access.Inventory.Controllers.Product.UpdateAsync(newModel, true);

        else if (detailChange)
            task = Access.Inventory.Controllers.Product.UpdateAsync(newModel, false);

        if (task != null)
        {
            var res = await task;

            if (res.Response == Responses.Success)
            {
                Modelo.FillWith(newModel);
                ModelHasChange();
               // Hub?.UpdateProduct(Modelo.Inventory, Modelo.ProductID);

            }


        }


        this.Close();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        this.Close();
    }

    public void RenderNewData(From from)
    {
        LoadData();
    }

    public void ModelHasChange()
    {
        ProductObserver.Update(Modelo, From.SameDevice);
    }
}