namespace LIN.React.Models;


public interface IProductViewer
{


    /// <summary>
    /// Modelo
    /// </summary>
    public ProductModel Modelo { get; set; }


    public string? ContextKey { get; init; }



    /// <summary>
    /// Renderiza la nueva información del modelo
    /// </summary>
    public void RenderNewData(From from);



    /// <summary>
    /// Envía los cambios de modelo
    /// </summary>
    public abstract void ModelHasChange();


}

public enum From
{
    Undefined,
    SameDevice,
    OtherDevice
}