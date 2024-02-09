using LIN.Services;
namespace LIN.UI.Controls;

public partial class Inventory : Grid
{


    /// <summary>
    /// Evento click del control
    /// </summary>
    public event EventHandler<EventArgs>? Clicked;



    /// <summary>
    /// Modelo del producto
    /// </summary>
    public InventoryDataModel Modelo { get; set; }




    /// <summary>
    /// Constructor
    /// </summary>
    public Inventory(InventoryDataModel modelo)
    {
        InitializeComponent();
        this.Modelo = modelo;
        // ProductSubscriber.Suscribe(this);
        LoadModelVisible();
    }




    /// <summary>
    /// Hace el modelo visible a la UI
    /// </summary>
    public void LoadModelVisible()
    {


        displayName.Text = Modelo.Nombre;
        displayDireccion.Text = Modelo.Direction;
        displayRol.Text = Modelo.MyRol.Humanize();

    }



    /// <summary>
    /// Envua el evento click
    /// </summary>
    private void SendEventClick(object sender, EventArgs e)
    {
        Clicked?.Invoke(this, new());
    }


}