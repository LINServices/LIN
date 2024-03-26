using LIN.Types.Inventory.Enumerations;

namespace LIN.Components.Shared;

public partial class InvitationControl
{


    [Parameter]
    public Notificacion Model { get; set; }


    [Parameter]
    public List<Notificacion> Models { get; set; }


    [Parameter]
    public Action OnRemove { get; set; }

    int section = 1;


    async void Accept()
    {
        section = 0;
        StateHasChanged();
        var response = await LIN.Access.Inventory.Controllers.Inventories.UpdateState(Model.ID, InventoryAccessState.Accepted);

        if (response.Response != Responses.Success)
        {
            section = 2;
            StateHasChanged();
        }

        Models.Remove(Model);
        OnRemove();

        section = 1;
        StateHasChanged();

    }


    async void Decline()
    {
        section = 0;
        StateHasChanged();
        var response = await LIN.Access.Inventory.Controllers.Inventories.UpdateState(Model.ID, InventoryAccessState.Deleted);

        if (response.Response != Responses.Success)
        {
            section = 2;
            StateHasChanged();
        }

        Models.Remove(Model);
        OnRemove();

        section = 1;
        StateHasChanged();

    }



}
