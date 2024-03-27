namespace LIN.Pages;


public partial class Home : IDisposable, INotificationObserver
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


}