
namespace LIN.UI.Controls;

public partial class ProductForPick : Grid
{


    //========= Eventos =========//

    /// <summary>
    /// Evento Click sonbre el control
    /// </summary>
    public event EventHandler<EventArgs>? Clicked;


    public bool CounterVisible
    {
        get => (bool)GetValue(CounterVisibleProperty);
        set
        {
            SetValue(CounterVisibleProperty, value);
        }
    }



    //========= Propiedades =========//

    /// <summary>
    /// Model del contacto
    /// </summary>
    public ProductModel Modelo { get; set; }



    /// <summary>
    /// Lista de colores para mostrar 
    /// </summary>
    private readonly static Color[] Colors = {
        Color.FromRgb(233, 69, 59),
        Color.FromRgb(168, 119, 89),
        Color.FromRgb(240, 126, 29),
        Color.FromRgb(156, 119, 214),
        Color.FromRgb(77, 188, 195),
        Color.FromRgb(48, 196, 110),
        Color.FromRgb(242, 107, 176),
        Color.FromRgb(50, 151, 219),
        Color.FromRgb(50, 151, 219)
};




    /// <summary>
    /// Constructor
    /// </summary>
    public ProductForPick(ProductModel modelo)
    {
        InitializeComponent();
        this.Modelo = modelo;
        //   ContactSubscriber.Suscribe(this);
        LoadModelVisible();
    }



    /// <summary>
    /// Muestra los datos del contacto en el display
    /// </summary>
    public void LoadModelVisible()
    {

        // Datos
        lbName.Text = Modelo.Name;

        if (string.IsNullOrEmpty(Modelo.Code.Trim()))
        {
            lbCode.Text = string.Empty;
            separator.Text = string.Empty;
        }
        else
        {
            lbCode.Text = Modelo.Code;
        }

        lbQu.Text = $"{Modelo.DetailModel.Quantity}u";

        // Si no hay imagen que mostar
        if (Modelo.Image.Length != 0)
        {
            img.Show();
            img.Source = ImageEncoder.Decode(Modelo.Image);
        }

    }



    public int GetCounterValue()
    {
        return counter.Value;
    }


    /// <summary>
    /// Obtiene un color random
    /// </summary>
    /// <returns></returns>
    private static Color RandonColor()
    {
        var rd = new Random();
        var value = rd.Next(0, Colors.Length - 1);
        return Colors[value];
    }


    public void Select()
    {
        bg.Stroke = Microsoft.Maui.Graphics.Colors.RoyalBlue;
        bg.StrokeThickness = 1;
    }

    public void UnSelect()
    {
        bg.Stroke = Microsoft.Maui.Graphics.Colors.LightGray;
        bg.StrokeThickness = 0.5;
    }


    /// <summary>
    /// Submit del evento click
    /// </summary>
    private void EventoClick(object sender, EventArgs e)
    {
        Clicked?.Invoke(this, new());
    }



    /// <summary>
    /// Propiedad Color de fondo
    /// </summary>
    public static readonly BindableProperty CounterVisibleProperty = BindableProperty.Create(
        propertyName: nameof(CounterVisible),
        returnType: typeof(bool),
        declaringType: typeof(ProductForPick),
        defaultValue: false,
        defaultBindingMode: BindingMode.TwoWay);


}