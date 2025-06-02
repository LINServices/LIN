namespace LIN.Components.Layout;

public partial class MainLayout
{

    /// <summary>
    /// Settings.
    /// </summary>
    public static DockSettings Settings = new();


    /// <summary>
    /// Popup de nuevo contacto.
    /// </summary>
    public static bool ShowNavigation { get; set; } = false;


    /// <summary>
    /// Popup de nuevo contacto.
    /// </summary>
    public static BottomNavigation Navigation { get; set; } = null!;


    /// <summary>
    /// Popup de nuevo contacto.
    /// </summary>
    public static NewContactPopup NewContactPopup { get; set; } = null!;


    /// <summary>
    /// Selector de dispositivos.
    /// </summary>
    public static DevicesDrawer DevicesSelector { get; set; } = null!;


    /// <summary>
    /// Popup de contacto.
    /// </summary>
    public static ContactPopup ContactPop { get; set; } = null!;


    /// <summary>
    /// Instance.
    /// </summary>
    public static MainLayout Instance = null!;


    /// <summary>
    /// Errores.
    /// </summary>
    public ErrorToast ErrorManager { get; set; } = null!;


    /// <summary>
    /// Nuevo.
    /// </summary>
    public MainLayout()
    {
        Instance = this;
    }


    /// <summary>
    /// Actualizar estado
    /// </summary>
    public static void Update()
    {
        Instance.State();
    }


    /// <summary>
    /// Navegar a una url.
    /// </summary>
    /// <param name="url">Url.</param>
    public static void Navigate(string url)
    {
        Instance.Nav.NavigateTo(url);
    }


    /// <summary>
    /// Actualizar el estado.
    /// </summary>
    private void State()
    {
        this.InvokeAsync(StateHasChanged);
    }


    /// <summary>
    /// Acción al presionar el botón del centro.
    /// </summary>
    public static void OnCenter(Action action)
    {
        Settings.OnCenterClick = action;
    }


    /// <summary>
    /// Configurar.
    /// </summary>
    /// <param name="settings"></param>
    public static void Configure(DockSettings settings)
    {
        Settings = settings;
        MainLayout.Update();
    }

    public static void ShowError(string message)
    {
        Instance.ErrorManager.ShowErrorToast(message);
    }

}