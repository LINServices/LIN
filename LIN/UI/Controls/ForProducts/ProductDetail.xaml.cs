namespace LIN.UI.Controls.Others;

public partial class ProductDetail : Grid
{


    /// <summary>
    /// Model del detalle
    /// </summary>
    public ProductModel Detalle { get; set; } = new();



    /// <summary>
    /// Constructor
    /// </summary>
    public ProductDetail(ProductModel detalle)
    {
        InitializeComponent();
        this.Detalle = detalle;
        LoadModel();
    }




    /// <summary>
    /// Carga el modelo a la vista
    /// </summary>
    private async void LoadModel()
    {

        // Obtiene el contacto
        //var contact = Access.Inventory.Controllers.Contact.Read(Detalle.Provider);

        // Muestra el modelo
        displayCompra.Contenido = $"${Detalle.DetailModel.PrecioCompra}";
        displayVenta.Contenido = $"${Detalle.DetailModel.PrecioVenta}";
        displayCantidad.Text = Detalle.DetailModel.Quantity.ToString();


        // Muesta el contacto
       // displayContacto.Modelo = (await contact).Model;
        displayContacto.LoadModelVisible();

        // Pone los colores adecuados
        SetImageAndColors();

        // Muestra las estadisticas
        var (porcent, ganancia) = Calculate();
        lbPorcent.Text += $"{porcent}%";
        lbGanancias.Text = $"${ganancia}";

    }



    /// <summary>
    /// Calcula el porcentaje de ganacia / perdida
    /// </summary>
    private (decimal porcent, decimal ganancia) Calculate()
    {

        try
        {
            // Ganancia o perdida neta
            decimal neto = Detalle.DetailModel.PrecioVenta - Detalle.DetailModel.PrecioCompra;

            // Porcentaje
            decimal percent = Math.Round((neto / Detalle.DetailModel.PrecioCompra) * 100, 2);

            // Retorna
            return (percent, neto * Detalle.DetailModel.Quantity);
        }
        catch 
        {
            return (0m, 0m);
        }


    }



    /// <summary>
    /// Pone las imagenes y colores
    /// </summary>
    private void SetImageAndColors()
    {

        // Si el precio de venta supera al de compra
        if (Detalle.DetailModel.PrecioVenta > Detalle.DetailModel.PrecioCompra)
        {
            img.Source = ImageSource.FromFile("subir.png");
            lbPorcent.Text = "+ ";
            lbPorcent.TextColor = new(80, 175, 0);
        }

        // Si el precio de venta es igual al de compra
        else if (Detalle.DetailModel.PrecioVenta == Detalle.DetailModel.PrecioCompra)
        {

            img.Source = ImageSource.FromFile("menos.png");
            lbPorcent.Text = "";
            lbPorcent.TextColor = new(243, 156, 18);
        }

        // Si el precio de venta es menor al de compra
        else if (Detalle.DetailModel.PrecioVenta < Detalle.DetailModel.PrecioCompra)
        {
            img.Source = ImageSource.FromFile("bajar.png");
            lbPorcent.TextColor = new(234, 67, 53);
        }

    }




}