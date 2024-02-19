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





}