namespace LIN.Pages.Sections.Viewer;


public partial class Product
{


    /// <summary>
    /// Lista de modelos
    /// </summary>
    private ProductModel? Modelo { get; set; } = new();



    /// <summary>
    /// Evento al inicializar.
    /// </summary>
    protected override Task OnInitializedAsync()
    {
        Modelo = Products.Selected;
        return base.OnInitializedAsync();
    }


}
