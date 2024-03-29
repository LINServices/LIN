﻿#if ANDROID
using Android.Views;
using LIN.Pages.Sections;

#endif

using LIN.Services;
using Microsoft.Extensions.Logging;

namespace LIN
{
    public static class MauiProgram
    {


        public static string GetPlatform()
        {
#if ANDROID
        return "Android";
#elif WINDOWS
            return "Windows";
#endif
        }


        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            LIN.Access.Auth.Build.Init();
            Build.Init();
            LIN.Access.Search.Build.Init();



            Realtime.DeviceName = DeviceInfo.Current.Name;

            Realtime.Build();
            return builder.Build();
        }





        public static void Aa()
        {
#if ANDROID
            var currentActivity = Platform.CurrentActivity;

            if (currentActivity == null || currentActivity.Window == null)
                return;

            var currentTheme = AppInfo.RequestedTheme;

            if (currentTheme == AppTheme.Light)
            {
                currentActivity.Window.SetStatusBarColor(new(247, 248, 253));
                currentActivity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
            }
            else
            {
                currentActivity.Window.SetStatusBarColor(new(0, 0, 0));
                currentActivity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
            }


            //currentActivity.Window.SetTitleColor(new(0, 0, 0));
#endif
        }


    }



}
