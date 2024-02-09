namespace LIN.UI.Controls;


public partial class Product : Grid, IProductViewer
{


    /// <summary>
    /// Evento click del control
    /// </summary>
    public event EventHandler<EventArgs>? Clicked;



    /// <summary>
    /// Modelo del producto
    /// </summary>
    public ProductModel Modelo { get; set; }


    public string? ContextKey { get; init; }




    /// <summary>
    /// Constructor
    /// </summary>
    public Product(ProductModel modelo)
    {
        InitializeComponent();
        this.Modelo = modelo;
        LoadModelVisible();

        ContextKey = $"ipv.{modelo.Id}";

        ProductObserver.Add(this);

    }

    ~Product()
    {
        System.Diagnostics.Debug.WriteLine("#SEARH-ME#");
        ProductObserver.Remove(this);
    }


    protected override void ChangeVisualState()
    {
        System.Diagnostics.Debug.WriteLine("#SEARH-ME#-VisualState");
        base.ChangeVisualState();
    }



    /// <summary>
    /// Hace el modelo visible a la UI
    /// </summary>
    public async void LoadModelVisible()
    {

        if (Modelo.Statement != ProductBaseStatements.Normal)
        {
            this.Hide();
            return;
        }

        // Metadatos 
        displayName.Text = Modelo.Name;
        displayPrice.Text = Modelo.DetailModel.PrecioCompra.ToString("0.##");


        // Imagen
        if (Modelo.Image.Length == 0)
            displayImagen.Source = ImageSource.FromFile("caja.png");
        else
            displayImagen.Source = ImageEncoder.Decode(Modelo.Image);

        // Cantidad
        switch (Modelo.DetailModel.Quantity)
        { 
            case <= 0:
                {
                    displayCantidad.Text = "Agotado";
                    displayCantidad.TextColor = Color.Parse("#FF4545");
                    bgCantidad.BackgroundColor = Color.Parse("#F5CDCD");
                    break;
                }
            case < 10:
                {
                    displayCantidad.Text = "Limitado";
                    displayCantidad.TextColor = Color.Parse("#FF8033");
                    bgCantidad.BackgroundColor = Color.Parse("#F5E1CD");
                    break;
                }
            default:
                {
                    displayCantidad.Text = "Activo";
                    displayCantidad.TextColor = Color.Parse("#41A553");
                    bgCantidad.BackgroundColor = Color.Parse("#CDF5D1");
                    break;
                }
        }

        // Categoria
        displayCategory.Text = Modelo.Category.Humanize();

        await Task.Delay(1);
    }



    /// <summary>
    /// Envua el evento click
    /// </summary>
    private void SendEventClick(object sender, EventArgs e)
    {
        Clicked?.Invoke(this, new());
    }

    private void displayImagen_Clicked(object sender, EventArgs e)
    {

    }

    public void RenderNewData(From from)
    {
        LoadModelVisible();
    }

    public void ModelHasChange()
    {
        ProductObserver.Update(this, From.SameDevice);
    }
}