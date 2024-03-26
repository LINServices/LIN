using LIN.Components.Layout;

namespace LIN.Pages;


public partial class Home
{



    EmmaDrawer EmmaOp { get; set; }

    protected override async void OnInitialized()
    {

        MainLayout.Configure(new()
        {
            OnCenterClick = () => { EmmaOp.Show(); },
            Section = 0,
            DockIcon = 1
        });
        MainLayout.ShowNavigation = true;

       await RefreshData();

        StateHasChanged();
        base.OnInitialized();
    }




    void OnRemove()
    {
        StateHasChanged();
    }



    Chart? chart { get; set; }

    void NN()
    {
        nav.NavigateTo("/contacts");
    }


    static ReadAllResponse<Types.Inventory.Transient.Notificacion> Notifications = new()
    {
        Response = Responses.IsLoading
    };





    private async Task<bool> RefreshData()
    {

        if (Notifications.Response == Responses.Success)
            return true;

        // Items
        var items = await LIN.Access.Inventory.Controllers.Inventories.ReadNotifications(LIN.Access.Inventory.Session.Instance.Token);

        // Rellena los items
        Notifications = items;
        return true;

    }


    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

    }


}