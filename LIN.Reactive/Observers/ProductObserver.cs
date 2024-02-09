namespace LIN.React.Observers;


public sealed class ProductObserver
{


    /// <summary>
    /// Lista de modelos
    /// </summary>
    private readonly static List<IProductViewer> _data = new();



    /// <summary>
    /// Agrega al nuevo viewer
    /// </summary>
    public static void Add(IProductViewer viewer)
    {

        if (viewer.ContextKey != null && viewer.ContextKey != "")
            _data.RemoveAll(T => T.ContextKey == viewer.ContextKey);

        if (!_data.Contains(viewer))
            _data.Add(viewer);
    }


    /// <summary>
    /// Elimina un viewer
    /// </summary>
    public static void Remove(IProductViewer viewer)
    {
        _data.Remove(viewer);
    }



















    public static bool FillWith(int id, ProductModel data)
    {


        var modelo = _data.Where(T => T.Modelo.Id == id).FirstOrDefault()?.Modelo;

        if (modelo == null)
        {
            return false;
        }

        return true;

    }





    /// <summary>
    /// Envia los cambios
    /// </summary>
    public static bool UpdateQuantity(int id, int newQuantity, From from)
    {


        var data = _data.Where(x => x.Modelo.Id == id);

        if (!data.Any())
            return false;

        foreach (var e in data)
        {
            e.Modelo.DetailModel.Quantity = newQuantity;
            e.RenderNewData(from);
        }

        return true;

    }





    /// <summary>
    /// Envia los cambios
    /// </summary>
    public static bool Update(IProductViewer context, From from)
    {

        var notificates = _data.Where(x => x.Modelo.Id == context.Modelo.Id).ToList();

        if (!notificates.Any())
            return false;

        foreach (var certificate in notificates)
        {
            certificate.RenderNewData(from);
        }
        return true;

    }


    /// <summary>
    /// Envia los cambios
    /// </summary>
    public static bool Update(ProductModel context, From from)
    {

        var notificates = _data.Where(x => x.Modelo.Id == context.Id).ToList();

        if (!notificates.Any())
            return false;

        foreach (var certificate in notificates)
        {
            //certificate.Modelo.FillWith(context);
            certificate.RenderNewData(from);
        }
        return true;

    }




}
