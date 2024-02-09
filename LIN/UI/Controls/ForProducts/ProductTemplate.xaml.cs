using Microsoft.Maui.Controls.Platform;

namespace LIN.UI.Controls;


public partial class ProductTemplate : Grid
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

    public bool IsSelected { get; private set; } = false;

    /// <summary>
    /// Constructor
    /// </summary>
    public ProductTemplate(ProductModel modelo)
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
        lbDescripcion.Text = Modelo.Description;


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


  

    public void Select()
    {
        bg.Stroke = Colors.RoyalBlue;
        IsSelected = true;
    }


    public void UnSelect()
    {
        bg.Stroke = Colors.LightGray;
        IsSelected = false;
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