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
    /// Abrir el popup.
    /// </summary>
    public async void Show()
    {

        try
        {
            // Abrir el popup.
            await Js.InvokeVoidAsync("ShowModal", "small-modal-member", "closeee-member", "close-btn-send-member");
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
        Show();

    }




}