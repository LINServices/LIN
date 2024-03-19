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



}