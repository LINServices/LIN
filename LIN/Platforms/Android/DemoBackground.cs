using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;

namespace LIN;


[Service(ForegroundServiceType = Android.Content.PM.ForegroundService.TypeDataSync)]
internal class DemoBackground : Service, LIN.Services.IBackgroundService
{

    public override IBinder OnBind(Intent intent)
    {
        throw new NotImplementedException();
    }




    public async void A()
    {
        while (AppShell.Hub != null)
        {
            AppShell.Hub.SendCommand(1, "a()");
            await Task.Delay(10000);
        }
    }





    bool init = false;


    [return: GeneratedEnum]
    public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
    {
        if (intent.Action == "START_SERVICE")
        {

            A();


            init = true;
            System.Diagnostics.Debug.WriteLine("Se ha iniciado el servicio");


            RegisterNotification();
        }
        else if (intent.Action == "STOP_SERVICE")
        {
            System.Diagnostics.Debug.WriteLine("Se ha detenido el servicio");
            StopForeground(true);
            init = false;
            StopSelfResult(startId);
        }
        return StartCommandResult.NotSticky;
    }

    public void Start()
    {
        init = true;
        Intent startService = new Intent(MainActivity.ActivityCurrent, typeof(DemoBackground));
        startService.SetAction("START_SERVICE");
        MainActivity.ActivityCurrent.StartService(startService);
    }

    public void Stop()
    {
        init = false;
        Intent stopIntent = new Intent(MainActivity.ActivityCurrent, this.Class);
        stopIntent.SetAction("STOP_SERVICE");
        MainActivity.ActivityCurrent.StartService(stopIntent);
    }

    private void RegisterNotification()
    {
        NotificationChannel channel = new NotificationChannel("ServicioChannel", "Demo de servicio", NotificationImportance.Max);
        NotificationManager manager = (NotificationManager)MainActivity.ActivityCurrent.GetSystemService(Context.NotificationService);
        manager.CreateNotificationChannel(channel);
        Notification notification = new Notification.Builder(this, "ServicioChannel")
            .SetContentTitle("Servicio trabajando")
            .SetSmallIcon(Resource.Drawable.abc_ab_share_pack_mtrl_alpha)
            .SetOngoing(true)
            .Build();

        StartForeground(100, notification);

    }

}
