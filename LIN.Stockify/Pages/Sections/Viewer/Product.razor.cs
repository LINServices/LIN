﻿namespace LIN.Pages.Sections.Viewer;


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


    public (string, string) GetClass()
    {


        switch (Modelo?.Category)
        {
            case ProductCategories.Tecnología:
                return ("Tecnología", "./img/Products/tec.png");
            case ProductCategories.Arte:
                return ("Cultura & Arte", "./img/Products/paint.png");
            case ProductCategories.Juguetes:
                return ("Juguete", "./img/Products/toy.png");
            case ProductCategories.Automóviles:
                return ("Automóviles", "./img/Products/car.png");
            case ProductCategories.Frutas:
                return ("Frutas y verduras", "./img/Products/fruit.png");
            case ProductCategories.Deporte:
                return ("Deportes", "./img/Products/fitness.png");
            case ProductCategories.Servicios:
                return ("Servicios", "./img/Products/tec.png");
            case ProductCategories.Salud:
                return ("Salud y farmacia", "./img/Products/health.png");
            case ProductCategories.Moda:
                return ("Moda", "./img/Products/fashion.png");
            case ProductCategories.Limpieza:
                return ("Limpieza", "./img/Products/clean.png");
            case ProductCategories.Animales:
                return ("Animales", "./img/Products/cat.png");
            case ProductCategories.Hogar:
                return ("Hogar y decoración", "./img/Products/home.png");
            case ProductCategories.Alimentos:
                return ("Alimentos", "./img/Products/food.png");
            case ProductCategories.Agricultura:
                return ("Hogar y decoración", "./img/Products/garden.png");
            default:
                return ("", "");
        }



    }



    public void Edit()
    {
        nav.NavigateTo($"/edit/product/{Modelo?.Id ?? 0}");
    }


}
