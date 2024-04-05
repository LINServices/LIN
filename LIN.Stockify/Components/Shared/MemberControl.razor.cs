namespace LIN.Components.Shared;


public partial class MemberControl
{

    /// <summary>
    /// Modelo.
    /// </summary>
    [Parameter]
    public IntegrantDataModel? Model { get; set; }


    /// <summary>
    /// Evento al hacer click.
    /// </summary>
    [Parameter]
    public Action<IntegrantDataModel?>? OnClick { get; set; }



    [Parameter]
    public bool State { get; set; }



    /// <summary>
    /// Enviar el evento.
    /// </summary>
    private void SendEvent()
    {
        OnClick?.Invoke(Model);
    }
}
