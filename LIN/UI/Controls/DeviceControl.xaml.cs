using LIN.UI.Views;
using Microsoft.Maui.Controls;
using System.Reflection;

namespace LIN.UI.Controls;


public partial class DeviceControl : Grid
{

    //****** Eventos ******//


    /// <summary>
    /// Evento click sobre el control
    /// </summary>
    public event EventHandler<EventArgs>? Clicked;



    //****** Propiedades ******//

    /// <summary>
    /// Marcado a ocultar
    /// </summary>
    public bool MarkedToHide { get; set; }


    /// <summary>
    /// Modelo
    /// </summary>
    //public DeviceModel Modelo { get; set; }



    /// <summary>
    /// Constructor
    /// </summary>
    //public DeviceControl(DeviceModel modelo, bool desvincular)
    //{
    //    InitializeComponent();
    //    this.Modelo = modelo;
    //    LoadModelVisible();

    //    if (desvincular)
    //        btnDesvincular.Show();
        
    //}



    /// <summary>
    /// Renderiza la informacion
    /// </summary>
    public void LoadModelVisible()
    {

        //displayName.Text = Modelo.Name;
        //ToolTipProperties.SetText(displayName, Modelo.DeviceKey);

        //displayApp.Text = "Aplicación";


        //if (Modelo.Platform == Platforms.Windows)
        //    img.Source = ImageSource.FromFile("ordenador.png");
        //else if (Modelo.Platform == Platforms.Android)
        //    img.Source = ImageSource.FromFile("telefono.png");
        //else if (Modelo.Platform == Platforms.Web)
        //    img.Source = ImageSource.FromFile("web.png");


    }



    /// <summary>
    /// Evento submit: Clicked
    /// </summary>
    private void ClickedSender(object sender, EventArgs e) => Clicked?.Invoke(this, new());


    private void Desvincular(object sender, EventArgs e)
    {
      //  AppShell.Hub.SendCommand(Modelo.ID, "disconnect()");
    }



}