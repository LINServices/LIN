namespace LIN.Components.Popup;


public partial class ContactPopup
{

    /// <summary>
    /// Acción al presionar sobre el botón de editar.
    /// </summary>
    [Parameter]
    public Action<ContactModel> OnEdit { get; set; } = (e) => { };



    /// <summary>
    /// Key.
    /// </summary>
    private string Key { get; init; } = Guid.NewGuid().ToString();



    /// <summary>
    /// Modelo del contacto.
    /// </summary>
    public ContactModel? Modelo { get; set; }



    /// <summary>
    /// Abrir el modal.
    /// </summary>
    public void Show(ContactModel model)
    {

        Modelo = model;
        StateHasChanged();
        _ = this.InvokeAsync(() =>
        {
            Js.InvokeVoidAsync("ShowModal", $"modal-{Key}", $"btn-{Key}", "close-btn-edit");
        });

    }



    /// <summary>
    /// Imagen en base64.
    /// </summary>
    string Img64 => Convert.ToBase64String(Modelo?.Picture ?? []);


    
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
                Command = $"viewContact({Modelo?.Id})"
            });
        };

        Components.Layout.MainLayout.DevicesSelector.Show();
    }


}