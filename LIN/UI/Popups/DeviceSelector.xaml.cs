namespace LIN.UI.Popups;


public partial class DeviceSelector : Popup
{

    //======= Propiedades =======//


    /// <summary>
    /// Lista de modelos
    /// </summary>
    private protected List<DeviceModel> Modelos { get; set; } = new();



    /// <summary>
    /// Lista de controles
    /// </summary>
    private protected List<DeviceControl> Controles { get; set; } = new();



    /// <summary>
    /// Comando que se va a ejecutar
    /// </summary>
    private protected string Comando { get; set; } = "";



    /// <summary>
    /// Filtro que se va a aplicar sobre los dispositivos
    /// </summary>
    private protected DeviceFilterModel Filtro { get; set; } = new();





    /// <summary>
    /// Constructor
    /// </summary>
    public DeviceSelector(string command, DeviceFilterModel filter)
    {
        // Inicializador
        InitializeComponent();
        this.CanBeDismissedByTappingOutsideOfPopup = false;

        // Propiedades
        this.Comando = command;
        this.Filtro = filter;
        Load();
    }




    /// <summary>
    /// Carga los datos y metadatos
    /// </summary>
    private async void Load()
    {
        Modelos.Clear();
        Controles.Clear();
        content.Clear();
        displayInfo.Hide();
        indicador.Show();
        await Task.Delay(1);

        // Carga los datos
        bool data = await RetrieveData();
        if (!data)
        {
            ShowInfo("Hubo un error al obtener los dispositivos");
            return;
        }


        // No hay dispositivos
        if (Modelos.Count <= 0)
        {
            ShowInfo("No hay dispositivos conectados");
            return;
        }

        else if (Modelos.Count == 1 && Filtro.AutoSelect)
        {
            try
            {
                AppShell.Hub.SendCommand(Modelos[0].ID, Comando);
                this.Close();
                return;
            }
            catch
            {
            }
        }


        // Construlle los controles
        BuildControls(Modelos);

        // Renderiza los controles
        RenderControls(Controles);

        ShowInfo($"{Modelos.Count} Dispositivos conectados");

    }



    /// <summary>
    /// Obtiene la informacion desde el servidor
    /// </summary>
    private async Task<bool> RetrieveData()
    {

        //Solicitud
        var devices = await Access.Auth.Controllers.Devices.ReadAll(Session.Instance.AccountToken);

        // Respuesta incorrecta
        if (devices.Response != Responses.Success)
            return false;

        // Modelos
        List<DeviceModel> filters = new();


        if (!Filtro.HasMe)
            filters = devices.Models.Where(T => T.ID != AppShell.Hub.ID).ToList();

        Modelos = ApplyFilters(filters, Filtro);

        return true;
    }




















    /// <summary>
    /// Muestra un mensaje
    /// </summary>
    private void ShowInfo(string message)
    {
        indicador.Hide();
        displayInfo.Show();
        displayInfo.Text = message ?? "";
    }



    /// <summary>
    /// Construlle una lista de controles
    /// </summary>
    private void BuildControls(List<DeviceModel> models)
    {
        foreach (var model in models)
        {
            var control = BuildOneControl(model);
            Controles.Add(control);
        }

    }



    /// <summary>
    /// Construlle un control
    /// </summary>
    private DeviceControl BuildOneControl(DeviceModel model)
    {
        var control = new DeviceControl(model, false);
        control.Clicked += (sender, e) =>
        {
            AppShell.Hub.SendCommand(model.ID, Comando);
            this.Close();
        };
        control.Margin = new(0, 3, 0, 0);
        return control;
    }



    /// <summary>
    /// Renderiza una lista de controles
    /// </summary>
    private void RenderControls(List<DeviceControl> controls)
    {
        content.Clear();
        foreach (var control in controls)
            RenderOneControl(control);
    }



    /// <summary>
    /// Renderiza un control
    /// </summary>
    private void RenderOneControl(DeviceControl control)
    {
        content.Add(control);
    }




    private void BtnCancelClick(object sender, EventArgs e)
    {
        this.Close();
    }


    private void BtnUpdateClick(object sender, EventArgs e)
    {
        Load();
    }







    /// <summary>
    /// Aplica todos los filtros
    /// </summary>
    private static List<DeviceModel> ApplyFilters(List<DeviceModel> lista, DeviceFilterModel filtro)
    {
        // Lista de elementos filtrados
        List<DeviceModel> filtrado = new();

        var filterApp = ApplyAppFilters(lista, filtro);
        var filterPlatform = ApplyPlatformsFilters(filterApp, filtro);

        return filterPlatform;

    }



    /// <summary>
    /// Aplica el filtro de plataforma
    /// </summary>
    private static List<DeviceModel> ApplyPlatformsFilters(List<DeviceModel> lista, DeviceFilterModel filtro)
    {
        // Lista de elementos filtrados
        List<DeviceModel> filtrado = new();
        if (filtro.Plataformas.Contains(Platforms.Undefined))
        {
            filtrado.AddRange(lista);
            return filtrado;
        }

        // Filtrado por plataforma
        foreach (var model in lista)
        {
            if (filtro.Plataformas.Contains(model.Platform))
                filtrado.Add(model);
        }

        return filtrado;

    }



    /// <summary>
    /// Aplica el filtro de aplicacion
    /// </summary>
    private static List<DeviceModel> ApplyAppFilters(List<DeviceModel> lista, DeviceFilterModel filtro)
    {
        // Lista de elementos filtrados
        List<DeviceModel> filtrado = new();
        if (filtro.App.Contains(Applications.Undefined))
        {
            filtrado.AddRange(lista);
            return filtrado;
        }

        // Filtrado por plataforma
        foreach (var model in lista)
        {
            filtrado.Add(model);
        }

        return filtrado;

    }



}