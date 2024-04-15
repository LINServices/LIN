namespace LIN.Components.Popup;


public partial class MemberPopup
{

    /// <summary>
    /// Modelo de contacto.
    /// </summary>
    private IntegrantDataModel? _data;


    /// <summary>
    /// Obtener / Establecer el modelo.
    /// </summary>
    [Parameter]
    public IntegrantDataModel? Model
    {
        get => _data;
        set
        {
            _data = value;
            InvokeAsync(StateHasChanged);
        }
    }


    /// <summary>
    /// Obtener / Establecer el modelo.
    /// </summary>
    [Parameter]
    public Action OnSuccess { get; set; } = () => { };


    [Parameter]
    public Action<int> OnDelete { get; set; } = (id) => { };



    int val = 0;
    int Type
    {
        get => val; set
        {

            val = value;
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
            await Js.InvokeVoidAsync("ShowModal", "small-modal-member", "closeee-member", "close-btn-send-member");
            StateHasChanged();
        }
        catch
        {
        }

    }



    /// <summary>
    /// Abrir el popup.
    /// </summary>
    public async void Show(IntegrantDataModel contact)
    {
        Model = contact;
        val = (int)Model.Rol;

        Show();

    }


    async void Change(Microsoft.AspNetCore.Components.ChangeEventArgs e)
    {
        var newRol = int.Parse(e.Value?.ToString() ?? "0");
        var response = await LIN.Access.Inventory.Controllers.InventoryAccess.UpdateRol(Model.AccessID, (InventoryRoles)newRol, Session.Instance.Token);

        if (response.Response == Responses.Success)
        {
            Model.Rol = (InventoryRoles)newRol;
            OnSuccess();
        }

    }


    async void Delete()
    {
        int id = Model.AccessID;
        var response = await Access.Inventory.Controllers.InventoryAccess.DeleteSomeOne(Model.InventoryID, Model.ProfileID, Session.Instance.Token);

        if (response.Response == Responses.Success)
        {
            OnDelete(id);
        }
    }

}