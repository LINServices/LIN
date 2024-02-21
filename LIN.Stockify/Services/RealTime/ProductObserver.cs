namespace LIN.Services.RealTime;


public class ProductObserver
{


    /// <summary>
    /// Valores.
    /// </summary>
    private readonly static Dictionary<int, List<IProduct>> Values = [];



    /// <summary>
    /// Agregar un observable.
    /// </summary>
    /// <param name="id">Id del inventario.</param>
    /// <param name="product">Observable.</param>
    public static void Add(int id, IProduct product)
    {

        // Si existe la lista.
        if (!Values.TryGetValue(id, out List<IProduct>? products))
        {
            // Nueva lista.
            products = [];

            // Agregar valor.
            Values.Add(id, products);
        }

        // Agregar a la lista el producto.
        products.Add(product);

    }



    /// <summary>
    /// Actualizar los observables de un inventario.
    /// </summary>
    /// <param name="id">Id del inventario.</param>
    public static void Update(int id)
    {

        // Recorrer los valores.
        if (Values.TryGetValue(id, out List<IProduct>? products))
            foreach (var product in products)
                product.Render();

    }



    /// <summary>
    /// Remover observable.
    /// </summary>
    /// <param name="product">Observable.</param>
    public static void Remove(IProduct product)
    {
        // Obtener elemento.
        var items = Values.FirstOrDefault(t => t.Value.Contains(product));

        // Eliminar producto.
        items.Value?.Remove(product);

    }


}



/// <summary>
/// Interfaz observable de productos.
/// </summary>
public interface IProduct
{

    /// <summary>
    /// Renderizar.
    /// </summary>
    void Render();

}