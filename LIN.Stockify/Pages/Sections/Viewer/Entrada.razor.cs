namespace LIN.Pages.Sections.Viewer;


public partial class Entrada
{


    /// <summary>
    /// Id de la entrada.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;



    /// <summary>
    /// Modelo
    /// </summary>
    private InflowDataModel? Modelo { get; set; } = new();



    /// <summary>
    /// Evento al iniciar.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {

        // Obtener el Contexto.
        var inventoryContext = (from context in Services.InventoryContext.Dictionary
                                where (context.Value.Inflows ?? new()).Models.Any(t => t.ID == int.Parse(Id))
                                select context.Value).FirstOrDefault();

        // Validar.
        if (inventoryContext == null)
        {
            // Obtener los detalles.
            var inflowDetails = await LIN.Access.Inventory.Controllers.Inflows.Read(int.Parse(Id), LIN.Access.Inventory.Session.Instance.Token, true);

            // Validar respuesta.
            if (inflowDetails.Response == Responses.Success)
                Modelo = inflowDetails.Model;

            return;
        }


        // Obtener la salida.
        var inflow = (from inflowModel in (inventoryContext.Inflows ?? new()).Models
                       where inflowModel.ID == int.Parse(Id)
                       select inflowModel).FirstOrDefault();

        // Si no hay detalles.
        if (inflow?.Details.Count <= 0)
        {
            // Obtener los detalles.
            var inflowDetails = await Access.Inventory.Controllers.Inflows.Read(inflow.ID, Session.Instance.Token, false);

            if (inflowDetails.Response == Responses.Success)
                inflow.Details = inflowDetails.Model.Details;
        }

        // Establecer el modelo.
        Modelo = inflow;

        await base.OnInitializedAsync();

    }


}