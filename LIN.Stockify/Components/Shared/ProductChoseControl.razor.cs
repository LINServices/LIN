using LIN.Types.Inventory.Models;

namespace LIN.Components.Shared;

public partial class ProductChoseControl
{


    /// <summary>
    /// Modelo del producto.
    /// </summary>
    [Parameter]
    public ProductModel? Model { get; set; }


    [Parameter]
    public bool IsInflow { get; set; }



    /// <summary>
    /// Evento al hacer click.
    /// </summary>
    [Parameter]
    public Action<ProductModel?>? OnClick { get; set; }




    [Parameter]
    public Action<int, int>? OnValueChange { get; set; }



    private int _cantidad = 0;


    [Parameter]
    public int Cantidad
    {
        get => _cantidad;
        set
        {
            if (Model != null)
                OnValueChange?.Invoke(Model.Id, value);

            _cantidad = value;
        }
    }


    /// <summary>
    /// Enviar el evento.
    /// </summary>
    private void SendEvent()
    {
        OnClick?.Invoke(Model);
    }



    private void Decrement()
    {
        if (Cantidad > 0)
        {
            Cantidad--;

        }

    }

    private void Increment()
    {
        Cantidad++;
    }


    private string GetImage()
    {

        if (Model.Image.Length <= 0)
        {
            return "./img/Products/packages.png";
        }

        return $"data:image/png;base64,{Convert.ToBase64String(Model.Image)}";


    }




    private (string, string, string) GetClass()
    {

        if (Model.DetailModel.Quantity <= 0)
        {
            return ("bg-red-50 border-red-500 dark:border-red-800 dark:bg-red-950/80", "fill-red-500 dark:fill-red-500", "dark:text-red-200");
        }

        else if (Model.DetailModel.Quantity <= 10)
        {
            return ("bg-orange-50 border-orange-500 dark:border-orange-800 dark:bg-orange-950/50", "fill-orange-500 dark:fill-orange-500", "dark:text-orange-200");
        }


        return ("bg-zinc-100 dark:border-zinc-600 dark:bg-zinc-800", "dark:fill-zinc-400", "dark:text-zinc-300");

    }

}