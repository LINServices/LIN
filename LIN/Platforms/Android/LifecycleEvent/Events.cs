using Android.App;

namespace LIN.LifecycleEvent;


internal class Events
{


    public static void OnStart(Activity activity)
    {
        var service = AppShell.ElementHandler?.MauiContext?.Services.GetServices<IBackgroundService>().FirstOrDefault();
        service?.Stop();

        //if (Session.IsLocalOpen)
        //    AppShell.Hub.ReconnectAndUpdate();

    }



    public static void OnStop(Activity activity)
    {
        try
        {
            var service = AppShell.ElementHandler?.MauiContext?.Services.GetServices<IBackgroundService>().FirstOrDefault();
            service?.Start();
            //if (AppShell.Hub != null)
            //    _ = AppShell.Hub.CloseSesion();
        }
        catch
        {
        }
    }


}
