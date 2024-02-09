using LIN.Access.Inventory.Controllers;

namespace LIN.UI.Views;

public partial class Home : ContentPage
{

    /// <summary>
    /// Lista de modelos de productos
    /// </summary>
    private List<Types.Inventory.Transient.Notificacion> Notificacions = new();


    /// <summary>
    /// Lista de los prodructos
    /// </summary>
    private List<Controls.Notificacion> Controles = new();





    /// <summary>
    /// Constructor
    /// </summary>
    public Home()
    {
        Appearing += AppearingEvent;
        AppShell.ActualPage = this;
       // AppShell.Hub.OnReceiveNotification += Hub_OnReceiveNotification1;
        InitializeComponent();
        LoadUserData();
        Load();
        Ventas();
        SuscribeToHub();
    }





    private void Hub_OnReceiveNotification1(object? sender, string e)
    {
        Dispatcher.DispatchAsync(Load);
    }

    private async void Ventas()
    {

        var id = LIN.Access.Inventory.Session.Instance.Informacion.ID;

        var home = await Access.Inventory.Controllers.Profile.TotalSales(id, 30);
        var valueTask = Access.Inventory.Controllers.Profile.ValueInventorys(id);

        ventas7.SubTitle = home.Model.Ventas7 + " ventas realizadas.";
        ventas30.SubTitle = home.Model.Ventas30 + " ventas realizadas.";

        compras30.SubTitle = home.Model.Compras30 + " compras realizadas.";
        compras7.SubTitle = home.Model.Compras7 + " compras realizadas.";
        ValueInventorys.Text = (await valueTask).Model.ToString("0.##");
    }



    private void SuscribeToHub()
    {
        //AppShell.Hub.OnReceiveNotification += Hub_OnReceiveNotification;
    }

    private void Hub_OnReceiveNotification(object? sender, string e)
    {
        this.Dispatcher.DispatchAsync(async () =>
        {
            await RefreshData();
        });
    }


    /// <summary>
    /// Pagina apareciendo
    /// </summary>
    private void AppearingEvent(object? sender, EventArgs e)
    {
        AppShell.ActualPage = this;
    }



    /// <summary>
    /// Carga los datos
    /// </summary>
    private async void Load()
    {
        // Info
        ClearInterface();

        // Rellena los datos
        var dataRes = await RefreshData();

        // Comprueba si se rellenaron los datos
        if (!dataRes)
        {
            indicador.Hide();
            return;
        }

        // Carga el cache
        Controles = LoadCache(Notificacions);

        // Carga los controles a la vista
        LoadControls(Controles);

        // Si no hay productos
        if (!Notificacions.Any())
            //  lbInfo.Text = "No hay nada que mostrar aqui";

            // Muestra el mensaje
            indicador.Hide();
        //  lbInfo.Show();

    }



    /// <summary>
    /// Rellena los datos desde la base de datos
    /// </summary>
    private async Task<bool> RefreshData()
    {

        // Items
        var items = await Inventories.ReadNotifications(Session.Instance.Token);

        // Analisis de respuesta
        if (items.Response != Responses.Success)
            return false;

        // Rellena los items
        Notificacions = items.Models.ToList();
        return true;

    }




    /// <summary>
    /// Muestra los controles a la vista
    /// </summary>
    public void AppendModel(Types.Inventory.Transient.Notificacion modelo)
    {
        Notificacions.Add(modelo);
        Controles = LoadCache(Notificacions);
        LoadControls(Controles);
    }



    /// <summary>
    /// Muestra los controles a la vista
    /// </summary>
    private void LoadControls(List<Controls.Notificacion> lista, string? message = null)
    {

        // Vacia los elementos
        content.Clear();

        // Mensaje
        //lbInfo.Text = message ?? $"Se encontraron {lista.Count} invitaciones.";

        // Agrega los controles
        foreach (var control in lista)
        {
            control.Show();
            control.Clicked += (sender, e) =>
            {
                //     new ViewItem(control.Modelo).Show();
            };
            content.Add(control);
        }

    }


  //  public View TL => AppShell.Instance.ju;


    /// <summary>
    /// Carga los modelos a los nuevos controles
    /// </summary>
    private static List<Controls.Notificacion> LoadCache(List<LIN.Types.Inventory.Transient.Notificacion> lista)
    {

        // Lista
        List<Controls.Notificacion> listaReturn = new();

        // Agrega los controles
        foreach (var model in lista)
        {
            var control = new Controls.Notificacion(model ?? new());
            listaReturn.Add(control);
        }

        return listaReturn;

    }



    /// <summary>
    /// Limpia la interfaz
    /// </summary>
    private void ClearInterface()
    {
        //    lbInfo.Hide();
        indicador.Hide();
        content.Clear();
    }


































    /// <summary>
    /// Carga la informacion de usuario a la vista
    /// </summary>
    private async void LoadUserData()
    {
        perfil.Source = ImageEncoder.Decode(Session.Instance.Account.Profile);


            lbBienvenido.Text = "Bienvenida, ";

        lbName.Text = Session.Instance.Account.Name.Trim().Split(" ")[0];


        lbUser.Text = Session.Instance.Account.Name;
        AppShell.SetTitle(Session.Instance.Account.Name);
        AppShell.SetImage(ImageEncoder.Decode(Session.Instance.Account.Profile));




        //    lbUsuario.Text = "@" + Session.Instance.Informacion.UsuarioU;
        //    imgPerfil.Source = ;
        await Task.Delay(1);
    }




    //======== Eventos =========//



    /// <summary>
    /// Click sobre la imagen de perfil
    /// </summary>
    private void ImgPerfil_Clicked(object sender, EventArgs e)
    {
    }


    /// <summary>
    /// Click sobre boton de inventarios
    /// </summary>
    private void BtnProductos_Clicked(object sender, EventArgs e)
    {
        new Inventorys.Index().Show();
    }



    /// <summary>
    /// Boton de contactos
    /// </summary>
    private void BtnContactos_Clicked(object sender, EventArgs e)
    {
        new Contacts.Index().Show();
    }

    private async void Border_Clicked(object sender, EventArgs e)
    {
        ventas7.SubTitle = "Cargando...";
        ventas30.SubTitle = "Cargando...";
        ValueInventorys.Text = "Cargando...";
        await Task.Delay(10);

        Ventas();
    }

    private void Label_Clicked(object sender, EventArgs e)
    {
        Load();
    }
}