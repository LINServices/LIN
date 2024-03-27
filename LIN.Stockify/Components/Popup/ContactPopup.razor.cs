namespace LIN.Components.Popup;


public partial class ContactPopup
{

    /// <summary>
    /// Modelo de contacto.
    /// </summary>
    private ContactModel? _data;



    string Img64 => Convert.ToBase64String(Model?.Picture ?? []);



    /// <summary>
    /// Obtener / Establecer el modelo.
    /// </summary>
    [Parameter]
    public ContactModel? Model
    {
        get => _data;
        set
        {
            _data = value;
            InvokeAsync(StateHasChanged);
        }
    }



    /// <summary>
    /// Abrir el popup.
    /// </summary>
    public async void Show()
    {

        try
        {
            // Abrir el popup.
            await Js.InvokeVoidAsync("ShowModal", "small-modal", "closeee", "close-btn-send");
        }
        catch
        {
            MainLayout.Update();
        }

    }



    /// <summary>
    /// Abrir el popup.
    /// </summary>
    public async void Show(ContactModel contact)
    {

        Model = contact;
        Show();

    }



    /// <summary>
    /// Enviar el comando al selector.
    /// </summary>
    void Send()
    {
        // Nuevo onInvoque.
        Layout.MainLayout.DevicesSelector.OnInvoke = (e) =>
        {
            Services.Realtime.InventoryAccessHub.SendToDevice(e.Id, new()
            {
                Command = $"viewContact({_data?.Id})"
            });
        };

        Components.Layout.MainLayout.DevicesSelector.Show();
    }


}