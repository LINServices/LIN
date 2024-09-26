using Android.App;
using Android.Runtime;

namespace LIN.Platforms.Android;

[Application(UsesCleartextTraffic = true)]
public class MainApplication(nint handle, JniHandleOwnership ownership) : MauiApplication(handle, ownership)
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
