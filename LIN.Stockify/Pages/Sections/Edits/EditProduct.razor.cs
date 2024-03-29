using SILF.Script;

namespace LIN.Pages.Sections.Edits;


public partial class EditProduct
{


    /// <summary>
    /// Id del producto.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;



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

        Product = result;
        Category = (int)Product.Category;

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

            return;

            Section = 3;
            StateHasChanged();

            //Product.Provider = 1;
           // Product.InventoryId = Contexto?.Inventory.ID ?? 0;
            Product.Category = (ProductCategories)Category;
            Product.Statement = ProductBaseStatements.Normal;
            Product.Image = Photo;

            // Respuesta del controlador
            var response = await Access.Inventory.Controllers.Product.Create(Product, LIN.Access.Inventory.Session.Instance.Token);


            if (response.Response != Responses.Success)
            {
                Section = 2;
                StateHasChanged();
                return;
            }


            Section = 1;
            StateHasChanged();



            _ = Services.Realtime.InventoryAccessHub.SendCommand(new()
            {
                Command = $"addProduct({response.LastID})",
                //Inventory = Contexto?.Inventory.ID ?? 0
            });


            await Task.Delay(3000);
            Section = 0;
            StateHasChanged();

        }
        catch (Exception)
        {
        }

    }



}