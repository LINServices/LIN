using LIN.Types.Inventory.Models;

namespace LIN.Pages.Sections.Viewer;

public partial class Salida
{

    [Parameter]
    public string Id { get; set; }


    /// <summary>
    /// Lista de modelos
    /// </summary>
    private OutflowDataModel? Modelo { get; set; } = new();




    protected override async Task OnInitializedAsync()
    {

        var a = Services.InventoryContext.Dictionary
            .Where(t => t.Value.Outflows.Models.Where(t => t.ID == int.Parse(Id)).Any()).FirstOrDefault();

        if (a.Value != null)
        {
            var inflow = a.Value.Outflows.Models.Where(t => t.ID == int.Parse(Id)).FirstOrDefault();

            if (inflow.Details.Count <= 0)
            {
                var inflowDetails = await LIN.Access.Inventory.Controllers.Outflows.Read(inflow.ID, LIN.Access.Inventory.Session.Instance.Token, false);

                if (inflowDetails.Response == Responses.Success)
                    inflow.Details = inflowDetails.Model.Details;
            
            }
            Modelo = inflow;

        }
        else
        {

            var inflowDetails = await LIN.Access.Inventory.Controllers.Outflows.Read(int.Parse(Id), LIN.Access.Inventory.Session.Instance.Token, true);

            if (inflowDetails.Response == Responses.Success)
                Modelo = inflowDetails.Model;
        }

        await base.OnInitializedAsync();
    }


}
