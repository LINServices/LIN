
namespace LIN.Components.Popup;


public partial class NewContactPopup
{

    /// <summary>
    /// Nombre.
    /// </summary>
    private string Name { get; set; } = string.Empty;


    /// <summary>
    /// Email.
    /// </summary>
    private string Mail { get; set; } = string.Empty;


    /// <summary>
    /// Teléfono.
    /// </summary>
    private string Phone { get; set; } = string.Empty;


    /// <summary>
    /// Dirección.
    /// </summary>
    private string Dirección { get; set; } = string.Empty;


    /// <summary>
    /// Perfil.
    /// </summary>
    private byte[] Profile { get; set; } = [];



    string Img64 => Convert.ToBase64String(Profile);



    /// <summary>
    /// Estado.
    /// </summary>
    private int state = 0;


    /// <summary>
    /// Key.
    /// </summary>
    public string Key = Guid.NewGuid().ToString();



    /// <summary>
    /// Abrir el modal.
    /// </summary>
    public async void Show()
    {
        state = 0;
        StateHasChanged();
        await Js.InvokeVoidAsync("ShowModal", $"small-modal-{Key}", $"close-{Key}");
    }



    /// <summary>
    /// Evento: Crear.
    /// </summary>
    public async void Create()
    {

        // Actualizar el estado.
        state = 1;
        StateHasChanged();

        // Validar.
        if (Name.Trim() == string.Empty)
        {
            // Mostrar el error.
            return;
        }

        // Modelo.
        var modelo = new ContactModel
        {
            Type = ContactTypes.Provider,
            Nombre = Name,
            Picture = Profile,
            Mails = [new MailModel()
            {
                Email = string.IsNullOrWhiteSpace(Mail) ? "Sin email" : Mail
            }],
            Phones = [new PhoneModel()
            {
                Number = string.IsNullOrWhiteSpace(Phone) ? "Sin teléfono" : Phone
            }]
        };


        // Respuesta del controlador
        var response = await Access.Inventory.Controllers.Contact.Create(LIN.Access.Inventory.Session.Instance.ContactsToken, modelo);

        // Si fue error.
        if (response.Response != Responses.Success)
        {
            state = 3;
            StateHasChanged();
            return;
        }

        // Cambiar el estado.
        state = 2;
        StateHasChanged();

    }



}