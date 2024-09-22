using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace LIN;

public partial class App : Microsoft.Maui.Controls.Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new MainPage();
        App.Current!.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
    }
}