global using CommunityToolkit.Maui.Animations;
using LIN.Controls.Handlers;

#if ANDROID
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
#endif

using Microsoft.Maui.Platform;

namespace LIN.Controls;

public static class Builder
{


    public static MauiAppBuilder UseCustomControls(this MauiAppBuilder builder)
    {




        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(BorderlessEntry), (handler, view) =>
        {
            if (view is BorderlessEntry)
            {
#if __ANDROID__

                handler.PlatformView.Background = null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
#elif __IOS__
              handler  .PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif WINDOWS
                handler.PlatformView.FocusVisualPrimaryBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(100, 81, 43, 212));
                handler.PlatformView.FocusVisualSecondaryBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(100, 81, 43, 212));
#endif
            }
        });


        return builder;
    }
}
