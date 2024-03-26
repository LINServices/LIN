namespace LIN.Components.Shared;

public partial class ContactControl
{


    /// <summary>
    /// Modelo.
    /// </summary>
    [Parameter]
    public ContactModel? Model { get; set; }


    /// <summary>
    /// Evento al hacer click.
    /// </summary>
    [Parameter]
    public Action<ContactModel?>? OnClick { get; set; }


    /// <summary>
    /// Enviar el evento.
    /// </summary>
    private void SendEvent()
    {
        OnClick?.Invoke(Model);
    }


    string Img64 => Convert.ToBase64String(Model?.Picture ?? []);


}