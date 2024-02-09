using static LIN.Controls.Builder;

namespace LIN;


public static class MauiProgram
{


    /// <summary>
    /// Key del dispositivo
    /// </summary>
    public static string DeviceSesionKey { get; set; } = "";






    /// <summary>
    /// Abre una nueva pagina
    /// </summary>
    public static void ShowOnTop(this Page newPage)
    {
        try
        {
            var npage = new NavigationPage(newPage);
            NavigationPage.SetHasNavigationBar(newPage, false);
            App.Current!.MainPage = npage;
        }
        catch (Exception ex)
        {
            App.Current!.MainPage.DisplayAlert("Error", ex.Message, "Ok");
        }
    }









    public static string GetDeviceName()
    {
        return DeviceInfo.Current.Name;
    }


    public static Platforms GetPlatform()
    {
#if ANDROID
        return Platforms.Android;
#elif WINDOWS
        return Platforms.Windows;
#endif
    }


    public static MauiApp CreateMauiApp()
    {

        // Builder
        var builder = MauiApp.CreateBuilder();


        // Configuración
        builder.UseMauiApp<App>();
#if ANDROID
        builder.Services.AddTransient<IBackgroundService, LIN.DemoBackground>();
#endif
      
        builder.ConfigureFonts(SetFonts);
        builder.UseMauiCommunityToolkit();
        builder.ConfigureEssentials(essentials =>
            {
                essentials.UseMapServiceToken("gCUbfMPXmCnDH2WR6uPk~JduHoZNxfxpNPxPihSH2aw~AoCRe2_PQIXYtX5u3x9BV03jFM3RE0zir7_M0c6laIIfdlNdgYeFhmohu_6bIQIp");
            });


        builder.ConfigureLifecycleEvents(events =>
         {
#if ANDROID
                events.AddAndroid(android => android


                    .OnActivityResult((activity, requestCode, resultCode, data) =>
                    {
                    })

                    // Evento (Al iniciar)
                    .OnStart(LifecycleEvent.Events.OnStart)

                    // Evento (Al finalizar)
                    .OnStop(LifecycleEvent.Events.OnStop)





                    .OnCreate((activity, bundle) =>
                    {
                    })

                    .OnBackPressed((activity) =>
                    {
                        return false;
                    })

                    .OnRestart((act) =>
                    {

                    })





                       );
#endif



#if WINDOWS
             events.AddWindows(windows => windows
                    .OnActivated((window, args) => LogEvent(nameof(WindowsLifecycle.OnActivated)))
                    .OnClosed((window, args) =>
                    {
                        try
                        {
                        //    if (AppShell.Hub != null)
                        //        _ = AppShell.Hub.CloseSesion();
                        }
                        catch
                        {

                        }

                    })
                        //                          .OnLaunched((window, args) =>
                        //                          {
                        //                              try
                        //                              {
                        //if (Session.IsLocalOpen)
                        //                                  AppShell.Hub.Reconnect();
                        //                              }
                        //                              catch
                        //                              {

                        //                              }

                        //                          })

                        .OnLaunching((window, args) => LogEvent(nameof(WindowsLifecycle.OnLaunching)))
                        .OnVisibilityChanged((window, args) => LogEvent(nameof(WindowsLifecycle.OnVisibilityChanged)))
                        .OnPlatformMessage((window, args) =>
                        {
                            if (args.MessageId == Convert.ToUInt32("031A", 16))
                            {
                                // System theme has changed
                            }
                        }));

             static bool LogEvent(string eventName, string type = null)
             {
                 System.Diagnostics.Debug.WriteLine($"Lifecycle event: {eventName}{(type == null ? string.Empty : $" ({type})")}");
                 return true;
             }


#endif




         });

  builder.UseCustomControls();


        DeviceSesionKey = KeyGen.Generate(20, "dv.");

        ScriptRuntime.Scripts.Build();


        // Servicios de LIN
        BatteryService.Initialize();

        return builder.Build();
    }



    /// <summary>
    /// Configuración de las fuentes
    /// </summary>
    private static void SetFonts(IFontCollection fonts)
    {
        // Fuentes de Windows
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

        // Fuentes de la aplicación LIN
        fonts.AddFont("Gilroy-Bold.ttf", "QSB");
        fonts.AddFont("Gilroy-Light.ttf", "QSL");
        fonts.AddFont("Gilroy-Medium.ttf", "QSM");
        fonts.AddFont("Gilroy-Regular.ttf", "QSR");
        fonts.AddFont("Gilroy-SemiBold.ttf", "QSSB");

        // Fuentes de utilidades
        fonts.AddFont("BarcodeFont.ttf", "Barcode");
    }





}
