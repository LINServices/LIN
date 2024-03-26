namespace LIN.Pages;


public partial class Home
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
    static ReadAllResponse<Notificacion> Notifications = new()
    {
        Response = Responses.IsLoading
    };



    /// <summary>
    /// Refrescar los datos de notificaciones.
    /// </summary>
    private static async Task<bool> RefreshData()
    {

        if (Notifications.Response == Responses.Success)
            return true;

        // Items
        var items = await LIN.Access.Inventory.Controllers.Inventories.ReadNotifications(LIN.Access.Inventory.Session.Instance.Token);

        // Rellena los items
        Notifications = items;
        return true;

    }


}