using LIN.Services;

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
        int id = Model.ID;
        section = 0;
        StateHasChanged();
        var response = await LIN.Access.Inventory.Controllers.InventoryAccess.UpdateState(Session.Instance.Token, id, InventoryAccessState.Accepted);

        if (response.Response != Responses.Success)
        {
            section = 2;
            StateHasChanged();
        }

        Models.Remove(Model);
        OnRemove();

        section = 1;
        UpRealTime(id);
        StateHasChanged();

    }


    async void Decline()
    {

        int id = Model.ID;

        section = 0;
        StateHasChanged();
        var response = await LIN.Access.Inventory.Controllers.InventoryAccess.UpdateState(Session.Instance.Token, id, InventoryAccessState.Deleted);

        if (response.Response != Responses.Success)
        {
            section = 2;
            StateHasChanged();
        }

        Models.Remove(Model);
        OnRemove();

        section = 1;
        UpRealTime(id);
        StateHasChanged();

    }


    async static void UpRealTime(int id)
    {
        await Realtime.InventoryAccessHub!.SendCommand(new CommandModel()
        {
            Command = $"newStateInvitation({id})",
            Inventory = 0
        });
    }

}
