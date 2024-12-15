#if ANDROID
using Android.Views;
#endif
using LIN.Access.Auth;
using LIN.Inventory.Realtime.Extensions;
using LIN.Inventory.Shared.Interfaces;
using LIN.Services;
using Microsoft.Extensions.Logging;
namespace LIN;

public static class MauiProgram
{

    /// <summary>
    /// Nueva estancia de la aplicación.
    /// </summary>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        // WebView.
        builder.Services.AddMauiBlazorWebView();

        // Local services.
        builder.Services.AddTransient<IDeviceSelector, DeviceSelector>();
        builder.Services.AddAuthenticationService();
        builder.Services.AddInventoryService();
        builder.Services.AddRealTime();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Acceso a datos.
        LIN.Access.Search.Build.Init();

        // Archivos.
        builder.Services.AddSingleton<IOpenFiles, Services.File>();

        var app = builder.Build();

        // Usar servicios de tiempo real.
        app.Services.UseRealTime(DeviceInfo.Current.Name, GetPlatform(), [], Scripts.Get(app.Services));

        return app;
    }


    /// <summary>
    /// En Android, establecer el color de la barra ne navegación al color de la app.
    /// </summary>
    public static void SetUIColors()
    {
#if ANDROID
        var currentActivity = Platform.CurrentActivity;

        if (currentActivity == null || currentActivity.Window == null)
            return;

        var currentTheme = AppInfo.RequestedTheme;

        if (currentTheme == AppTheme.Light)
        {
            currentActivity.Window.SetStatusBarColor(new(247, 248, 253));
            currentActivity.Window.SetNavigationBarColor(new(247, 248, 253));
            currentActivity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
        }
        else
        {
            currentActivity.Window.SetStatusBarColor(new(0, 0, 0));
            currentActivity.Window.SetNavigationBarColor(new(0, 0, 0));
            currentActivity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.Visible;
        }
#endif
    }


    public static string GetPlatform()
    {
#if WINDOWS
        return "windows";
#elif ANDROID
return "android";
#endif
    }

}