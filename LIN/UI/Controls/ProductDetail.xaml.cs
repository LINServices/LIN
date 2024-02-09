namespace LIN.UI.Controls;

public partial class ProductDetail : Grid
{


    /// <summary>
    /// Evento click del control
    /// </summary>
    public event EventHandler<EventArgs>? Clicked;



    /// <summary>
    /// Modelo del producto
    /// </summary>
    public InflowDetailsDataModel Modelo { get; set; }


    public ProductModel Producto { get; set; }


    List<ProductModel> Tranfers;

    Action? OnFinish;

    /// <summary>
    /// Constructor
    /// </summary>
    public ProductDetail(InflowDetailsDataModel modelo, List<ProductModel> tranfers, Action? OnFinish = null)
    {
        InitializeComponent();
        this.Modelo = modelo;
        Tranfers = tranfers;
        this.OnFinish = OnFinish;
        LoadModelVisible();
    }




    /// <summary>
    /// Hace el modelo visible a la UI
    /// </summary>
    public async void LoadModelVisible()
    {

        // Obtiene el producto
        Producto = (await Access.Inventory.Controllers.Product.ReadOneByDetail(Modelo.ProductDetailId, Session.Instance.Token)).Model;

        Tranfers.Add(Producto);
        OnFinish?.Invoke();
        // Metadatos 
        displayName.Text = Producto.Name;

        // Imagen
        if (Producto.Image.Length == 0)
            displayImagen.Source = ImageSource.FromFile("caja.png");
        else
            displayImagen.Source = ImageEncoder.Decode(Producto.Image);


        if (Modelo is InflowDetailsDataModel)
            displayCant.Text = $"+{Modelo.Cantidad}";
        else
        {
            displayCant.Text = $"-{Modelo.Cantidad}";
        }



        var price = Producto.DetailModel.PrecioVenta - Producto.DetailModel.PrecioCompra;

        displayPrice.Text = $"{price * Modelo.Cantidad}";


        // Categoria
        await Task.Delay(1);
    }



    /// <summary>
    /// Envua el evento click
    /// </summary>
    private void SendEventClick(object sender, EventArgs e)
    {
        Clicked?.Invoke(this, new());
    }


}