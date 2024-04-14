namespace LIN.Components.Popup;


public partial class AlertPopup
{



    public string Content { get; set; }



    /// <summary>
    /// Key.
    /// </summary>
    private string Key { get; init; } = Guid.NewGuid().ToString();



    /// <summary>
    /// Abrir el modal.
    /// </summary>
    public void Show(string content)
    {
        _ = this.InvokeAsync(() =>
        {
            Content = content;
            StateHasChanged();
            Js.InvokeVoidAsync("ShowModal", $"popup-modal-{Key}", $"btn-accept-{Key}", $"btn-cancel-{Key}", $"btn-close-{Key}");

        });

    }


}