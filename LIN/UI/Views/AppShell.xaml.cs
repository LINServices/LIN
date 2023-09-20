using SILF.Script.Enums;
using SILF.Script.Interfaces;

namespace LIN.UI.Views;


public partial class AppShell : Shell
{

    /// <summary>
    /// Instancia de AppShell
    /// </summary>
    public static AppShell? Instance { get; set; }


    public new IElementHandler? Handler => base.Handler;


    public static  IElementHandler? ElementHandler => Instance?.Handler;


    /// <summary>
    /// Hub de ViewON
    /// </summary>
    public static LIN.Access.Auth.Hubs.AccountHub Hub = new(BuildHub());



    /// <summary>
    /// Obtiene o establece la pagina actual de contenido
    /// </summary>
    public static ContentPage? ActualPage { get; set; }



    /// <summary>
    /// Constructor
    /// </summary>
    public AppShell()
    {
        InitializeComponent();
        Instance = this;

        Hub = new(BuildHub());

        // Eventos del HUB
        Hub.OnReceivingCommand += Hub_OnRecieve;
        Hub.OnDeviceChange += Hub_OnChange;

        BatteryService.StatusChange += BatteryService_StatusChange;

    }



    /// <summary>
    /// EVENTO: Cambia el estado
    /// </summary>
    private void BatteryService_StatusChange(object? sender, BatteryStatus e)
    {
        
        if (Hub?.DeviceModel == null)
            return;

        if (Hub.DeviceModel.BateryLevel != e.Percent || Hub.DeviceModel.BateryConected != e.IsChargin)
        {
            Hub.DeviceModel.BateryLevel = e.Percent;
            Hub.DeviceModel.BateryConected = e.IsChargin;
        }
        Hub.SendBattery();
    }



    /// <summary>
    /// Abre nueva ventana de ViewON
    /// </summary>
    /// <param name="command">Comando SILF</param>
    public static async void OnViewON(string command, Applications app = Applications.Inventory)
    {

        // Filtro de ViewON
        var filtro = new DeviceFilterModel()
        {
            App = new[] { Applications.Inventory, app },
            AutoSelect = false,
            HasMe = false
        };

        // Abre el Popup
        var popup = new Popups.DeviceSelector(command, filtro);
        await popup.Show();

    }
















    public static void SetTitle(string user)
    {
        if (Instance == null)
            return;

        Instance.lbUser.Text = user;
    }

    public static void SetImage(ImageSource source)
    {
        if (Instance == null)
            return;

        Instance.perfil.Source = source;
    }











    /// <summary>
    /// Construlle el HUB
    /// </summary>
    private static async Task<DeviceModel> BuildHub()
    {

        // Arma el modelo
        var model = new DeviceModel()
        {
            Name = MauiProgram.GetDeviceName(),
            Cuenta = Session.Instance.Account.ID,
            Modelo = DeviceInfo.Current.Model,
            BateryConected = BatteryService.IsChargin,
            BateryLevel = BatteryService.Percent,
            Manufacter = DeviceInfo.Current.Manufacturer,
            OsVersion = DeviceInfo.Current.VersionString,
            Platform = MauiProgram.GetPlatform(),
            App = Applications.Inventory,
            DeviceKey = MauiProgram.DeviceSesionKey,
            Token = Session.Instance.AccountToken,
        };

        // Locacion
        var location = await LocationService.GetLocation();

        double Logitud = location.Longitude;
        double Latitud = location.Latitude;

        model.Logitud = Logitud;
        model.Latitud = Latitud;

        return model;

    }



    /// <summary>
    /// Cuando algo cambia
    /// </summary>
    private void Hub_OnChange(object? sender, string e)
    {
        Dispatcher.DispatchAsync(new Action(() =>
        {
            Hub.GetDevicesList(Session.Instance.Account.ID);
        }));
    }



    /// <summary>
    /// Recibe un Script SILF
    /// </summary>
    /// <param name="e">Script</param>
    private void Hub_OnRecieve(object? sender, string e)
    {
        Dispatcher.DispatchAsync(new Action(() =>
        {

            

            var app = new SILF.Script.App(e, new a());

            app.AddDefaultFunctions(ScriptRuntime.Scripts.Actions);

            app.Run();

        }));
    }



}

class a : IConsole
{
    public void InsertLine(string result, LogLevel logLevel)
    {
       
    }
}