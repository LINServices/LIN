namespace LIN.Pages.Sections.Viewer;


public partial class Product
{

    static Product Instance;

  

    /// <summary>
    /// Lista de modelos
    /// </summary>
    private ProductModel? Modelo => Products.Selected;



    /// <summary>
    /// Evento al inicializar.
    /// </summary>
    protected override Task OnInitializedAsync()
    {
        Instance = this;
        MainLayout.Configure(new()
        {
            OnCenterClick = Send,
            Section = 1,
            DockIcon = 2
        });

        return base.OnInitializedAsync();
    }


    /// <summary>
    /// Enviar el comando al selector.
    /// </summary>
    void Send()
    {
        // Nuevo onInvoque.
        MainLayout.DevicesSelector.OnInvoke = (e) =>
        {
            Services.Realtime.InventoryAccessHub.SendToDevice(e.Id, new()
            {
                Command = $"viewProduct({Modelo?.InventoryId},{Modelo?.Id})"
            });
        };

        Components.Layout.MainLayout.DevicesSelector.Show();
    }



    void Render()
    {
        this.InvokeAsync(StateHasChanged);
    }


  
    public static void Show()
    {
        MainLayout.Navigate("/product");
        Instance?.Render();
    }

    public void Edit()
    {
        nav.NavigateTo($"/edit/product/{Modelo?.Id ?? 0}");
    }


}
