namespace LIN.Components.Shared;


public partial class DeviceControl
{

    /// <summary>
    /// Modelo del producto.
    /// </summary>
    [Parameter]
    public DeviceModel? Model { get; set; }



    /// <summary>
    /// Evento al hacer click.
    /// </summary>
    [Parameter]
    public Action<DeviceModel?>? OnClick { get; set; }



    /// <summary>
    /// Enviar el evento.
    /// </summary>
    private void SendEvent()
    {
        OnClick?.Invoke(Model);
    }



    /// <summary>
    /// Obtener el icono.
    /// </summary>
    private string GetImage()
    {

        // Segun.
        switch (Model?.Platform)
        {
            // Android.
            case "Android":
                return "./img/android.png";

            // Windows
            case "Windows":
                return "./img/windows.png";

        }
        return "";

    }


}
