namespace LIN.Pages;


public partial class Contactos
{

    /// <summary>
    /// Esta cargando.
    /// </summary>
    private bool IsLoading = false;



    /// <summary>
    /// Instancia.
    /// </summary>
    private static Contactos? _instance = null;



    /// <summary>
    /// Constructor.
    /// </summary>
    public Contactos()
    {
        _instance = this;
    }




    /// <summary>
    /// Respuesta.
    /// </summary>
    public static ReadAllResponse<ContactModel>? Response { get; set; } = null;




    /// <summary>
    /// Actualizar.
    /// </summary>
    public static void ToUpdate()
    {
        Response = null;
        _instance?.Refresh();
    }



    /// <summary>
    /// Actualizar los datos.
    /// </summary>
    void Refresh()
    {
        InvokeAsync(() =>
        {
            GetData(true);
        });
    }



    /// <summary>
    /// Obtener los datos.
    /// </summary>
    /// <param name="force">Es forzado.</param>
    private async void GetData(bool force = false)
    {

        // Validación.
        if ((!force && (Response != null)) || IsLoading)
            return;

        // Cambiar el estado.
        IsLoading = true;
        StateHasChanged();

        // Obtiene los dispositivos
        var result = await Access.Inventory.Controllers.Contact.ReadAll(Session.Instance.ContactsToken);

        // Nuevos estados.
        IsLoading = false;
        Response = result;
        StateHasChanged();

    }



    /// <summary>
    /// Abrir el contacto.
    /// </summary>
    /// <param name="e">Modelo.</param>
    private static void Go(ContactModel e)
    {
        MainLayout.ContactPop.Show(e);
    }



    /// <summary>
    /// Abrir el popup de crear.
    /// </summary>
    private static void OpenCreate()
    {
        MainLayout.NewContactPopup.Show();
    }




    //******** Eventos ********//


    /// <summary>
    /// Evento: Al inicializar.
    /// </summary>
    protected override void OnInitialized()
    {
        GetData();
        base.OnInitialized();
    }



    /// <summary>
    /// Evento después de renderizar.
    /// </summary>
    protected override void OnAfterRender(bool firstRender)
    {

        MainLayout.Configure(new()
        {
            OnCenterClick = OpenCreate,
            Section = 2,
            DockIcon = 0
        });

        base.OnAfterRender(firstRender);
    }



}
