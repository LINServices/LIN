using LIN.Components.Layout;

namespace LIN.Pages;


public partial class Home
{


    protected override void OnInitialized()
    {  
        MainLayout.ShowNavigation = true;
        MainLayout.Configure(() => { }, 0);
        base.OnInitialized();
    }



}