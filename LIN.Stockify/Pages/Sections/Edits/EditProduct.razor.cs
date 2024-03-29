namespace LIN.Pages.Sections.Edits;


public partial class EditProduct
{


    /// <summary>
    /// Id del producto.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;

    public ProductModel ProductBase { get; set; }



    /// <summary>
    /// Modelo del producto.
    /// </summary>
    public ProductModel Product { get; set; } = new()

    {
        Details = [new()]
    };



    /// <summary>
    /// Imagen.
    /// </summary>
    public byte[] Photo { get; set; } = [];



    /// <summary>
    /// Categoría.
    /// </summary>
    public int Category { get; set; }



    /// <summary>
    /// Sección actual.
    /// </summary>
    private int Section { get; set; }



    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {

        // Obtener el contexto.
        var result = Services.InventoryContext.GetProduct(int.Parse(Id));


        if (result == null)
        {
            nav.NavigateTo("/home");
            return;
        }

        Product = new()
        {
            Category = result.Category,
            Statement = result.Statement,
            Code = result.Code,
            Description = result.Description,
            Name = result.Name,
            Inventory = result.Inventory,
            InventoryId = result.InventoryId,
            Id = result.Id,
            Image = result.Image,
            Details = [
                new ProductDetailModel(){
                    Estado = result.DetailModel.Estado,
                    Id = result.DetailModel.Id,
                    PrecioCompra = result.DetailModel.PrecioCompra,
                    PrecioVenta = result.DetailModel.PrecioVenta,
                    Quantity = result.DetailModel.Quantity
                }
                ],
        };


        Category = (int)Product.Category;

        ProductBase = result;

        // Base.
        base.OnParametersSet();
    }



    protected override void OnInitialized()
    {
        MainLayout.Configure(new()
        {
            OnCenterClick = Update,
            Section = 1,
            DockIcon = 3
        });

        base.OnInitialized();
    }


    /// <summary>
    /// Crear.
    /// </summary>
    private async void Update()
    {
        try
        {

            Section = 3;
            StateHasChanged();


            if (!NeedUpdateDetail())
            {
                Product.Details = [];
            }

            Product.Category = (ProductCategories)Category;


            // Respuesta del controlador
            var response = await Access.Inventory.Controllers.Product.Update(Product, LIN.Access.Inventory.Session.Instance.Token);

            Product = ProductBase;

            if (response.Response != Responses.Success)
            {
                Section = 2;
                StateHasChanged();
                return;
            }


            Section = 1;
            StateHasChanged();


            await Task.Delay(3000);
            Section = 0;
            StateHasChanged();

        }
        catch (Exception)
        {
        }

    }






    bool NeedUpdateDetail()
    {

        try
        {
            if (ProductBase.DetailModel.PrecioCompra != Product.DetailModel.PrecioCompra)
                return true;


            if (ProductBase.DetailModel.PrecioVenta != Product.DetailModel.PrecioVenta)
                return true;


            return false;
        }
        catch
        {

        }

        return false;


    }


}