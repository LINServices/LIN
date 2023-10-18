namespace LIN.React.Models;


public interface IContactViewer
{


    /// <summary>
    /// Modelo
    /// </summary>
    public LIN.Types.Contacts.Models.ContactModel Modelo { get; set; }


    public string? ContextKey { get; init; }



    /// <summary>
    /// Renderiza la nueva información del modelo
    /// </summary>
    public void RenderNewData();



    /// <summary>
    /// Envía los cambios de modelo
    /// </summary>
    public abstract void ModelHasChange();


}
