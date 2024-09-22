namespace LIN.Components.Pages;

public partial class Home : IDisposable, INotificationModelObserver
{

    /// <summary>
    /// Emma IA.
    /// </summary>
    private EmmaDrawer EmmaIA { get; set; } = null!;


    /// <summary>
    /// Chart.
    /// </summary>
    private Chart Chart { get; set; } = null!;


    /// <summary>
    /// Evento al inicializar.
    /// </summary>
    protected override async void OnInitialized()
    {

        MainLayout.Configure(new()
        {
            OnCenterClick = () => { EmmaIA.Show(); },
            Section = 0,
            DockIcon = 1
        });
        MainLayout.ShowNavigation = true;

        await Home.RefreshData();

        StateHasChanged();
        base.OnInitialized();
    }



    /// <summary>
    /// Evento al remover una notificación.
    /// </summary>
    private void OnRemove()
    {
        StateHasChanged();
    }



    /// <summary>
    /// Notificaciones.
    /// </summary>
    private static ReadAllResponse<Notificacion> Notifications = new()
    {
        Response = Responses.IsLoading
    };


    /// <summary>
    /// Notificaciones.
    /// </summary>
    private static ReadOneResponse<HomeDto>? HomeDTO = null;




    /// <summary>
    /// Refrescar los datos de notificaciones.
    /// </summary>
    private static async Task<bool> RefreshData()
    {

        if (Notifications.Response == Responses.Success)
            return true;

        // Items
        var items = await LIN.Access.Inventory.Controllers.InventoryAccess.ReadNotifications(LIN.Access.Inventory.Session.Instance.Token);

        // Rellena los items
        Notifications = items;
        return true;

    }




    /// <summary>
    /// Refrescar los datos de notificaciones.
    /// </summary>
    private async Task<bool> RefreshDataHome(bool force = false)
    {

        try
        {
            if ((HomeDTO != null && !force) && HomeDTO.Response == Responses.Success)
                return true;

            // Items
            var items = await LIN.Access.Inventory.Controllers.Statistics.HomeStatistics(LIN.Access.Inventory.Session.Instance.Token);

            // Rellena los items
            HomeDTO = items;
            Chart?.Set(HomeDTO.Model);

            StateHasChanged();
            return true;

        }
        catch (Exception) { }

        return false;

    }


    /// <summary>
    /// Renderizar.
    /// </summary>
    public void Render()
    {
        this.InvokeAsync(StateHasChanged);
    }



    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {
        NotificationObserver.Add(this);
        base.OnParametersSet();
    }


    /// <summary>
    /// Agregar modelo de notificación.
    /// </summary>
    /// <param name="modelo">Modelo.</param>
    public void Add(Notificacion modelo)
    {
        InvokeAsync(() =>
        {
            var exist = Notifications.Models.Any(t => t.ID == modelo.ID);
            if (!exist)
                Notifications.Models.Add(modelo);

            StateHasChanged();
        });
    }



    /// <summary>
    /// Eliminar un modelo de notificación.
    /// </summary>
    /// <param name="id">Id de la notificación.</param>
    public void Remove(int id)
    {
        Notifications.Models.RemoveAll(t => t.ID == id);
        InvokeAsync(StateHasChanged);
    }



    /// <summary>
    /// Evento dispose.
    /// </summary>
    public void Dispose()
    {
        NotificationObserver.Remove(this);
    }



    /// <summary>
    /// Limpiar.
    /// </summary>
    public static void Clean()
    {
        Notifications = new()
        {
            Response = Responses.IsLoading
        };
    }


    static string FormatearNumero(decimal numero)
    {
        // Definir los límites para las representaciones abreviadas
        const int mill = 1000000;
        const int kilo = 1000;

        // Verificar si el número es mayor a un millón
        if (numero >= mill)
        {
            // Representación abreviada en millones
            return $"{numero / (decimal)mill:F1}M";
        }
        // Verificar si el número es mayor a mil
        else if (numero >= kilo)
        {
            // Representación abreviada en miles
            return $"{numero / (decimal)kilo:F1}K";
        }
        // Si el número es menor a mil, no se realiza ninguna abreviatura
        else
        {
            return numero.ToString();
        }
    }


    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
            _ = RefreshDataHome();

        base.OnAfterRender(firstRender);
    }





    public string Calcular()
    {
        try
        {

            if (HomeDTO == null)
                return "0";

            var result = ((HomeDTO.Model.TodaySalesTotal - HomeDTO.Model.YesterdaySalesTotal) / HomeDTO.Model.YesterdaySalesTotal * 100);
            return result.ToString("0.#");
        }
        catch (Exception)
        {
        }
        return "0";
    }

}