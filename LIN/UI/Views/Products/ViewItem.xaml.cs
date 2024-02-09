

namespace LIN.UI.Views.Products;


public partial class ViewItem : ContentPage, IProductViewer
{


    //====== Propiedades ======//

    /// <summary>
    /// Modelo del producto
    /// </summary>
    public ProductModel Modelo { get; set; }


    public string? ContextKey { get; init; } = "VI";


    InventoryDataModel? Inventario;

    InventoryAccessHub? Hub = null;


    /// <summary>
    /// Constructor
    /// </summary>
    public ViewItem(ProductModel modelo, InventoryDataModel? inventario = null, InventoryAccessHub? hub = null)
    {
        // Suscribe la vista
        ProductObserver.Add(this);
        this.Inventario = inventario;
        Hub = hub;
        this.Appearing += ViewItem_Appearing;
        this.Disappearing += ViewItem_Disappearing;
        InitializeComponent();
        Modelo = modelo;
        Modelo.Normalize();
        LoadModelVisible();
        AppShell.ActualPage = this;
        LoadPermissions();

    }




    /// <summary>
    /// Reaccion al permiso actual
    /// </summary>
    private void LoadPermissions()
    {

        if (Inventario == null)
        {
            btnEditar.Hide();
            btnEliminar.Hide();
            return;
        }


        if (Inventario.MyRol.HasMovementUpdatePermissions())
        {
            btnEditar.Show();
            btnEliminar.Show();
        }

        else
        {
            btnEditar.Hide();
            btnEliminar.Hide();
        }

    }


    private void ViewItem_Disappearing(object? sender, EventArgs e)
    {

    }

    private void ViewItem_Appearing(object? sender, EventArgs e)
    {
        AppShell.ActualPage = this;
        ProductObserver.Add(this);
    }




    /// <summary>
    /// Hace el modelo visible en la UI
    /// </summary>
    public void LoadModelVisible()
    {

        if (Modelo.Estado == ProductBaseStatements.Deleted)
        {
            _ = DisplayAlert("Eliminado", "Este producto fue eliminado", "OK");
            this.Close();
        }

        Detalles.Clear();

        // Muestra los datos
        displayName.Text = Modelo.Name;
        displayCategory.Text = Modelo.Category.Humanize();
        displayDescripcion.Text = Modelo.Description;
        barcode.Text = Modelo.Code;

        // Imagen
        displayImagen.Source = (Modelo.Image.Length <= 0) ? ImageSource.FromFile("caja.png") : ImageEncoder.Decode(Modelo.Image);


#if WINDOWS
        var control = new Controls.Others.ProductDetail(Modelo);
        Detalles.Add(control);
#elif ANDROID
        var control = new Controls.Others.ProductDetailMobile(Modelo);
        Detalles.Add(control);
#endif


    }






    private void Label_Clicked(object sender, EventArgs e)
    {
        AppShell.OnViewON($"openPr({Modelo.ProductID})");
    }


    public async void RenderNewData(From from)
    {
        if (from == From.OtherDevice && AppShell.ActualPage == this)
        {
            var x = await DisplayAlert("Actualización", "Este producto fue actualizado desde otro dispositivo, ¿Quieres ver los nuevos cambios?", "Si", "No");
            if (!x)
                return;
        }
        LoadModelVisible();
    }


    // Click editar
    private async void ToggleButton_Clicked(object sender, EventArgs e)
    {
        await new Popups.ProductEdit(Modelo, Hub).Show();
    }

    public void ModelHasChange()
    {
        ProductObserver.Update(this, From.SameDevice);
    }

    private async void btnEliminar_Clicked(object sender, EventArgs e)
    {

        bool answer = await DisplayAlert("¿Desea eliminar?", $"Realmente quiere eliminar el producto '{Modelo.Name}'", "Si, eliminar", "Cancelar");

        if (!answer)
            return;

        var response = await Access.Inventory.Controllers.Product.Delete(Modelo.ProductID);

        if (response.Response != Responses.Success)
        {
            await DisplayAlert("Error", "Hubo un error al eliminar el producto", "OK");
            return;
        }

        //Hub?.DeleteProducto(Modelo.Inventory, Modelo.ProductID);
        ProductObserver.Remove(this);
        Modelo.Estado = ProductBaseStatements.Deleted;
        ProductObserver.Update(Modelo, From.SameDevice);

        this.Close();

    }

}