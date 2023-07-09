﻿using static LIN.Controls.Builder;

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
    public static void ShowOnTop(this ContentPage newPage)
    {
        var npage = new NavigationPage(newPage);
        NavigationPage.SetHasNavigationBar(newPage, false);
        App.Current!.MainPage = npage;
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

        // Configuracion
        builder
            .UseMauiApp<App>()
            .UseCustomControls()
            .ConfigureFonts(SetFonts)
            .UseMauiCommunityToolkit()
            .ConfigureEssentials(essentials =>
            {
                essentials.UseMapServiceToken("gCUbfMPXmCnDH2WR6uPk~JduHoZNxfxpNPxPihSH2aw~AoCRe2_PQIXYtX5u3x9BV03jFM3RE0zir7_M0c6laIIfdlNdgYeFhmohu_6bIQIp");
            })
            .ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android => android
                    .OnActivityResult((activity, requestCode, resultCode, data) =>
                    {
                    })

                    .OnStart((activity) =>
                    {

                        // Battery.Default.BatteryInfoChanged += Default_BatteryInfoChanged;

                        if (Sesion.IsOpen)
                        {
                            AppShell.Hub.ReconnectAndUpdate();
                        }


                    })

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


                       .OnStop((activity) =>
                       {
                           try
                           {
                               AppShell.Hub.CloseSesion();
                           }
                           catch
                           {

                           }

                       }
                       ));
#endif



#if WINDOWS
                events.AddWindows(windows => windows
                       .OnActivated((window, args) => LogEvent(nameof(WindowsLifecycle.OnActivated)))
                       .OnClosed((window, args) =>
                       {
                           try
                           {
                               AppShell.Hub.CloseSesion();
                           }
                           catch
                           {

                           }

                       })
                           .OnLaunched((window, args) =>
                           {
                               if (Sesion.IsOpen)
                                   AppShell.Hub.Reconnect();
                           })

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



        DeviceSesionKey = Shared.Tools.KeyGen.Generate(20, "dv.");

        ScriptRuntime.Scripts.Build();


        // Servicios de LIN
        Services.BatteryService.Initialize();

        return builder.Build();
    }





    /// <summary>
    /// Configuracion de las fuentes
    /// </summary>
    private static void SetFonts(IFontCollection fonts)
    {
        SetFontsAndroid(fonts);
    }


    /// <summary>
    /// Configuracion de las fuentes
    /// </summary>
    private static void SetFontsWindows(IFontCollection fonts)
    {
        // Fuentes de Windows
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

        // Fuentes de la aplicacion LIN
        fonts.AddFont("Quicksand-Bold.ttf", "QSB");
        fonts.AddFont("Quicksand-Light.ttf", "QSL");
        fonts.AddFont("Quicksand-Medium.ttf", "QSM");
        fonts.AddFont("Quicksand-Regular.ttf", "QSR");
        fonts.AddFont("Quicksand-SemiBold.ttf", "QSSB");

        // Fuentes de utilidades
        fonts.AddFont("BarcodeFont.ttf", "Barcode");
    }

    /// <summary>
    /// Configuracion de las fuentes
    /// </summary>
    private static void SetFontsAndroid(IFontCollection fonts)
    {
        // Fuentes de Windows
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

        // Fuentes de la aplicacion LIN
        fonts.AddFont("Gilroy-Bold.ttf", "QSB");
        fonts.AddFont("Gilroy-Light.ttf", "QSL");
        fonts.AddFont("Gilroy-Medium.ttf", "QSM");
        fonts.AddFont("Gilroy-Regular.ttf", "QSR");
        fonts.AddFont("Gilroy-SemiBold.ttf", "QSSB");

        // Fuentes de utilidades
        fonts.AddFont("BarcodeFont.ttf", "Barcode");
    }





}
