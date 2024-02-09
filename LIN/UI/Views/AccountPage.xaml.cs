using LIN.Types.Cloud.Identity.Enumerations;

namespace LIN.UI.Views;


public partial class AccountPage : ContentPage
{


    ///// <summary>
    ///// Lista de dispositivos
    ///// </summary>
    //private static List<DeviceModel> Devices { get; set; } = new();



    /// <summary>
    /// Lista de controles de dispositivos
    /// </summary>
    public static List<DeviceControl> DevicesControls { get; set; } = new();





    /// <summary>
    /// Constructor
    /// </summary>
    public AccountPage()
    {

        InitializeComponent();

        // Pagina actual
        AppShell.ActualPage = this;

        // Evento (Aparece esta pagina)
        Appearing += AccountPage_Appearing;

        // Eventos para el HUB
        //AppShell.Hub.OnReceiveDevicesList += OnRecieveAll;
        //AppShell.Hub.OnDeviceJoins += OnReceiveDevice;
        //AppShell.Hub.OnDeviceLeaves += OnSomeoneLeave;

        // Muestra la información
        perfil.Source = ImageEncoder.Decode(Session.Instance.Account.Profile);
        lbName.Text = Session.Instance. Account.Name;
        displayUser.Text = "@" + Session.Instance.Account.Identity.Unique;

    

    }



   





















    #region Eventos



    /// <summary>
    /// Evento: Un dispositivo se deconecta
    /// </summary>
    private void OnSomeoneLeave(object? sender, string e)
    {
        Dispatcher.DispatchAsync(new Action(() =>
        {

            //// Obtiene los controles
            //var controls = DevicesControls.Where(D => D.Modelo.ID == e);

            //// Oculta los controles visibles
            //foreach (var control in controls)
            //    control.Hide();

            //// Elimina los controles del cache
            //DevicesControls.RemoveAll(D => D.Modelo.ID == e);

            //// Elimina los modelos
            //Devices.RemoveAll(T => T.ID == e);

        }));

    }



    /// <summary>
    /// Evento: Recibir un dispositivo
    /// </summary>
    //private void OnReceiveDevice(object? sender, DeviceModel e)
    //{
    //    Dispatcher.DispatchAsync(() =>
    //    {

    //        // Contador
    //        var models = Devices.Where(T => T.DeviceKey == e.DeviceKey).ToList();

    //        // Remplaza la informacion
    //        if (models.Count > 0)
    //        {
    //            models[0].BateryLevel = e.BateryLevel;
    //            models[0].BateryConected = e.BateryConected;
    //            models[0].Estado = e.Estado;
    //            models[0].ID = e.ID;
    //            models[0].Logitud = e.Logitud;
    //            models[0].Latitud = e.Latitud;
    //            return;
    //        }

    //        // Agrega el nuevo dispositivo a la lista
    //        Devices.Add(e);

    //        // Renderiza un modelo
    //        RenderDevice(e);

    //    });
    //}



    ///// <summary>
    ///// Evento: Reciben todos los dispositivos
    ///// </summary>
    //private void OnRecieveAll(object? sender, List<DeviceModel> e)
    //{
    //    Dispatcher.DispatchAsync(() =>
    //    {

    //        foreach (var deC in DevicesControls)
    //        {
    //            deC.MarkedToHide = true;
    //        }

    //        // Recorre los nuevos modelos
    //        foreach (var @new in e)
    //        {

    //            // Comprueba si ya existen
    //            var have = DevicesControls.Where(T => T.Modelo.DeviceKey == @new.DeviceKey);

    //            // Si ya existe
    //            if (have.Any())
    //            {
    //                foreach (var i in have)
    //                    i.MarkedToHide = false;

    //                continue;
    //            }



    //            // Renderiza el nuevo modelo
    //            Devices.Add(@new);

    //            RenderDevice(@new);

    //        }


    //        var items = DevicesControls.FindAll(T => T.MarkedToHide);

    //        foreach (var i in items)
    //        {
    //            i.Hide();
    //            DevicesControls.Remove(i);
    //        }

    //    });
    //}



    #endregion














    /// <summary>
    /// Evento: Alguien sale
    /// </summary>
    [Obsolete("Metodo obsoleto, Eliminar")]
    private void Hub_OnSomeoneLeave(object? sender, string e)
    {

        Dispatcher.DispatchAsync(new Action(() =>
        {
            //var some = DevicesControls.Where(C => C.Modelo.ID == e).FirstOrDefault();
            //if (some != null)
            //{
            //    some.Hide();
            //    DevicesControls.Remove(some);
            //    Devices.RemoveAll(T => T.ID == e);
            //}
        }));

    }



  


  

    /// <summary>
    /// Pagina apareciendo
    /// </summary>
    bool rendering = false;
    private async void AccountPage_Appearing(object? sender, EventArgs e)
    {

        //if (AppShell.Hub == null)
        //    return;

        //AppShell.Hub.SendTest();
        //AppShell.ActualPage = this;

        if (rendering)
            return;

        rendering = true;

      // var devices = await LIN.Access.Controllers.Devices.ReadAll(AppShell.Hub.Id, Session.Instance.Informacion.Id);


     //OnRecieveAll(this, devices.Models);

        rendering = false;
//
     //   AppShell.Hub.TestConnection();

    }





    //private void RenderDevices(List<DeviceModel> modelos)
    //{
    //    foreach (var model in modelos)
    //        RenderDevice(model);
    //}


    //private void RenderDevice(DeviceModel modelo)
    //{
    //    // Control
    //    var control = new DeviceControl(modelo, true);

    //    // Evento
    //    control.Clicked += (sender, e) =>
    //     {
    //         new Views.Devices.Index(modelo).Show();
    //     };

    //    DevicesControls.Add(control);
    //    contenido.Add(control);
    //}

    private async void Button_Clicked(object sender, EventArgs e)
    {

        var pop = new Popups.UserPassEdit();
        await pop.Show();

    }

    private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
    {



    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {




    }

    private void Label_Clicked(object sender, EventArgs e)
    {

    }

    private void lbName_Clicked(object sender, EventArgs e)
    {

    }

    private async void LogOutEvent(object sender, EventArgs e)
    {
        Services.Login.Logout.Start();
    }

    private void Control_Clicked(object? sender, EventArgs e)
    {
        var x = (ProductTemplate)sender;
        x.Select();
    }
}