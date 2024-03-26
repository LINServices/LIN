using LIN.Types.Inventory.Enumerations;

namespace LIN.Pages.Sections.New;

public partial class NewProduct
{


    Services.Models.InventoryContextModel? Contexto { get; set; }



    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {

        // Obtener el contexto.
        Contexto = Services.InventoryContext.Get(int.Parse(Id));

        // Base.
        base.OnParametersSet();
    }



    [Parameter]
    public string Id { get; set; } = string.Empty;



    int section = 0;

    public byte[] Photo = [];

    public ProductModel Product { get; set; } = new()
    {
        Details = [new()]
    };

    public int Category { get; set; }


    async void Create()
    {
        try
        {

            section = 3;
            StateHasChanged();

            //Product.Provider = 1;
            Product.InventoryId = Contexto.Inventory.ID;
            Product.Category = (ProductCategories)Category;
            Product.Statement = ProductBaseStatements.Normal;
            Product.Image = Photo;

            // Respuesta del controlador
            var response = await Access.Inventory.Controllers.Product.Create(Product, LIN.Access.Inventory.Session.Instance.Token);


            if (response.Response != Responses.Success)
            {
                section = 2;
                StateHasChanged();
                return;
            }


            section = 1;
            StateHasChanged();



            _ = Services.Realtime.InventoryAccess.SendCommand(new()
            {
                Command = $"addProduct({response.LastID})",
                Inventory = Contexto.Inventory.ID
            });


            await Task.Delay(4000);
            section = 0;
            StateHasChanged();




        }
        catch (Exception ex)
        {
            var s = "";
        }

    }




}