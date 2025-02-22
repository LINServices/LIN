using LIN.Types.Inventory.Enumerations;

namespace LIN.Components.Pages.Sections.Viewer;


public partial class Product
{
    private static Product? Instance;

    private DeletePopup? deletePopup;

    private AlertPopup? alertPopup;


    /// <summary>
    /// Lista de modelos
    /// </summary>
    private ProductModel? Modelo => Products.Selected;



    /// <summary>
    /// Evento al inicializar.
    /// </summary>
    protected override Task OnInitializedAsync()
    {
        Instance = this;
        MainLayout.Configure(new()
        {
            OnCenterClick = Send,
            Section = 1,
            DockIcon = 2
        });

        return base.OnInitializedAsync();
    }


    /// <summary>
    /// Enviar el comando al selector.
    /// </summary>
    private void Send()
    {
        // Nuevo onInvoque.
        MainLayout.DevicesSelector.OnInvoke = (e) =>
        {
            deviceManager.SendToDevice($"viewProduct({Modelo?.InventoryId},{Modelo?.Id})", e.Id);
        };

        Components.Layout.MainLayout.DevicesSelector.Show();
    }

    private void Render()
    {
        this.InvokeAsync(StateHasChanged);
    }



    public static void Show()
    {
        MainLayout.Navigate("/product");
        Instance?.Render();
    }


    public (string, string) GetClass()
    {


        return (Modelo?.Category) switch
        {
            ProductCategories.Tecnología => ("Tecnología", "./img/Products/tec.png"),
            ProductCategories.Arte => ("Cultura & Arte", "./img/Products/paint.png"),
            ProductCategories.Juguetes => ("Juguete", "./img/Products/toy.png"),
            ProductCategories.Automóviles => ("Automóviles", "./img/Products/car.png"),
            ProductCategories.Frutas => ("Frutas y verduras", "./img/Products/fruit.png"),
            ProductCategories.Deporte => ("Deportes", "./img/Products/fitness.png"),
            ProductCategories.Servicios => ("Servicios", "./img/Products/tec.png"),
            ProductCategories.Salud => ("Salud y farmacia", "./img/Products/health.png"),
            ProductCategories.Moda => ("Moda", "./img/Products/fashion.png"),
            ProductCategories.Limpieza => ("Limpieza", "./img/Products/clean.png"),
            ProductCategories.Animales => ("Animales", "./img/Products/cat.png"),
            ProductCategories.Hogar => ("Hogar y decoración", "./img/Products/home.png"),
            ProductCategories.Alimentos => ("Alimentos", "./img/Products/food.png"),
            ProductCategories.Agricultura => ("Agricultura", "./img/Products/garden.png"),
            _ => ("", ""),
        };
    }



    public void Edit()
    {
        nav.NavigateTo($"/edit/product/{Modelo?.Id ?? 0}");
    }



    public async void Delete()
    {

        var response = await LIN.Access.Inventory.Controllers.Product.Delete(Modelo?.Id ?? 0, Session.Instance.Token);

        string message;

        switch (response.Response)
        {
            case Responses.Success:
                return;

            case Responses.Unauthorized:
                message = "No tienes autorización para eliminar este producto.";
                break;
            default:

                message = "Hubo un error al eliminar este producto.";
                break;
        }


        alertPopup?.Show(message);


    }



    private string GetImage()
    {

        return Modelo?.Image.Length <= 0
            ? "./img/Products/packages.png"
            : Modelo?.Image;
    }

}
