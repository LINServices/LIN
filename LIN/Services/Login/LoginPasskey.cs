namespace LIN.Services.Login;

internal class LoginPasskey : ILoginStrategy
{

    Action OnWaiting;

    Action<object?, PassKeyModel> OnRecieve;


    bool isDispose = false;

    private Access.Auth.Hubs.PassKeyHub? Hub = null;

    public LoginPasskey(Action onWaiting, Action<object?, PassKeyModel> onRecieve)
    {
        this.OnWaiting = onWaiting;
        this.OnRecieve = onRecieve;
    }

    public void Dispose()
    {
        Hub?.Disconnect();
        Hub = null;
        isDispose = true;
    }


    public async Task<(string message, bool can)> Login(string username, string? val)
    {
        try
        {
            // Campos vacíos
            if (string.IsNullOrEmpty(username))
                return ("Por favor, asegúrate de llenar todos los campos requeridos.", false);

            // Connection a internet
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return ("No hay conexión a internet", false);

            // Ejecucion de OnWaiting
            OnWaiting();

            Hub = new LIN.Access.Auth.Hubs.PassKeyHub(username, "AJEESKK123", "");
            await Hub.Suscribe();


            Hub.OnReceiveResponse += Hub_OnRecieveResponse;

            var intent = new PassKeyModel()
            {
                User = username
            };

            Hub?.SendIntent(intent);


            await Task.Delay(60000);

            Hub?.Disconnect();
            Hub = null;

            return ("La sesión expiro", isDispose);

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }

        return ("Hubo un error", false);

    }



    private void Hub_OnRecieveResponse(object? sender, PassKeyModel e)
    {
        Dispose();
        OnRecieve(sender, e);
    }

}
