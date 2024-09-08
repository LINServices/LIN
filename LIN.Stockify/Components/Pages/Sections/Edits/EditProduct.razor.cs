global using LIN.Inventory.Shared.Services;
using LIN.Types.Inventory.Enumerations;

namespace LIN.Components.Pages.Sections.Edits;


public partial class EditProduct
{


    /// <summary>
    /// Id del producto.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;

    public ProductModel ProductBase { get; set; }




    string Img64 => Convert.ToBase64String(Photo ?? []);


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


    string ErrorMessage = "";



    bool isNewPhoto = false;

    async void OpenImage()
    {
        Photo = await Services.File.Open();
        isNewPhoto = true;
        StateHasChanged();
    }


    async void SetImage(byte[] photo)
    {

        Photo = photo;
        isNewPhoto = false;
        StateHasChanged();
    }


    async void DeleteImage()
    {
        Photo = [];
        isNewPhoto = true;
        StateHasChanged();
    }



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
        var result = InventoryContext.GetProduct(int.Parse(Id));


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


        SetImage(result.Image);
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
                ProductBase.Details = Product.Details;
                Product.Details = [];
            }

            Product.Category = (ProductCategories)Category;


            if (!isNewPhoto)
                Product.Image = null!;
            else
                Product.Image = Photo;

            // Respuesta del controlador
            var response = await Access.Inventory.Controllers.Product.Update(Product, LIN.Access.Inventory.Session.Instance.Token);

            Product.Image = Photo;
            Product.Details = ProductBase.Details;


            switch (response.Response)
            {

                case Responses.Success:
                    break;

                case Responses.Unauthorized:
                    Section = 2;
                    ErrorMessage = "No tienes autorización para modificar este producto.";
                    StateHasChanged();
                    return;

                default:
                    Section = 2;
                    ErrorMessage = "Hubo un error al modificar este producto.";
                    StateHasChanged();
                    return;
            }


            // Actualizar modelo existente.
            ProductBase.Category = Product.Category;
            ProductBase.Code = Product.Code;
            ProductBase.Description = Product.Description;
            ProductBase.Name = Product.Name;
            ProductBase.Image = Product.Image;

            if (Product.DetailModel != null && ProductBase.DetailModel != null)
            {
                ProductBase.DetailModel.PrecioCompra = Product.DetailModel.PrecioCompra;
                ProductBase.DetailModel.PrecioVenta = Product.DetailModel.PrecioVenta;
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



    void GoNormal()
    {
        Section = 0;
        StateHasChanged();
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