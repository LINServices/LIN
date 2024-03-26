using LIN.Access.Inventory.Controllers;
using LIN.Components.Layout;
using LIN.Types.Inventory.Models;

namespace LIN.Pages;


public partial class Contactos
{



    public static Contactos? _instance = null;


    public Contactos()
    {
        _instance = this;
    }




    /// <summary>
    /// Respuesta.
    /// </summary>
    public static ReadAllResponse<ContactModel>? Response { get; set; } = null;







    public static ContactModel? Selected { get; set; } = null;



    public static void ToUpdate()
    {
        Response = null;
        _instance?.A();
    }






    void A()
    {
        InvokeAsync(() =>
        {
            GetData(true);
        });
    }


    protected override void OnInitialized()
    {
        base.OnInitialized();

        GetData();
    }





    bool IsLoading = false;

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










    //******** Eventos ********//





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




    void Go(Types.Contacts.Models.ContactModel e)
    {
        LIN.Components.Layout.MainLayout.ContactPop.Model = e;
        LIN.Components.Layout.MainLayout.ContactPop.Show();
    }


    void OpenCreate()
    {
        LIN.Components.Layout.MainLayout.NewContactPopup.Show();
    }

}
