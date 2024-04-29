using LIN.Inventory.Shared.Drawers;
using System.Runtime.CompilerServices;

namespace LIN.Components.Layout;


public partial class MainLayout
{

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


    public static MainLayout e = null!;


    public MainLayout()
    {
        e = this;
    }


    public static void Update()
    {
        e.State();
    }


    public static void Navigate(string url)
    {
        e.Nav.NavigateTo(url);
    }

    void State()
    {
        this.InvokeAsync(StateHasChanged);
    }

    public static void OnCenter(Action action)
    {
        Settings.OnCenterClick = action;


    }


    public static void Configure(DockSettings settings)
    {
        
        Settings = settings;
        MainLayout.Update();
    }


}