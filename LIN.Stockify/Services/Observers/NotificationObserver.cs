namespace LIN.Services.Observers;


public class NotificationObserver
{


    /// <summary>
    /// Valores.
    /// </summary>
    private readonly static List<INotificationObserver> Values = [];




    public static void Add(INotificationObserver notification)
    {
        Values.Add(notification);
    }



    /// <summary>
    /// Actualizar los observables de un inventario.
    /// </summary>
    /// <param name="id">Id del inventario.</param>
    public static void Update(int id)
    {

        // Recorrer los valores.
        foreach (var notification in Values)
            notification.Render();

    }




    /// <summary>
    /// Agregar modelo.
    /// </summary>
    /// <param name="modelo">Modelo.</param>
    public static void Append(Notificacion modelo)
    {
        // Recorrer los valores.
        foreach (var notification in Values)
            notification.Add(modelo);

    }



    /// <summary>
    /// Eliminar modelo.
    /// </summary>
    public static void Delete(int id)
    {

        // Recorrer los valores.
        foreach (var notification in Values)
            notification.Remove(id);

    }



    /// <summary>
    /// Remover observable.
    /// </summary>
    /// <param name="notification">Observable.</param>
    public static void Remove(INotificationObserver notification)
    {
        // Obtener elemento.
        var items = Values.RemoveAll(t => t == notification);

    }


}



/// <summary>
/// Interfaz observable de productos.
/// </summary>
public interface INotificationObserver
{

    /// <summary>
    /// Renderizar.
    /// </summary>
    void Render();


    /// <summary>
    /// Agregar.
    /// </summary>
    void Add(Notificacion modelo);


    /// <summary>
    /// Agregar.
    /// </summary>
    void Remove(int id);

}