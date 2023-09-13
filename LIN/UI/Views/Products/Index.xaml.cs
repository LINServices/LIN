

using LIN.Services;


namespace LIN.UI.Views.Products;

public partial class Index : ContentPage
{

    /// <summary>
    /// HUB de productos
    /// </summary>
    public InventoryAccessHub? HubConnection { get; set; }



    /// <summary>
    /// Lista de modelos
    /// </summary>
    private List<ProductDataTransfer> Modelos { get; set; } = new();



    /// <summary>
    /// Lista de los controles
    /// </summary>
    private List<Controls.Product> Controles { get; set; } = new();



    /// <summary>
    /// ID del inventario asociado
    /// </summary>
    private InventoryDataModel Inventario { get; set; }




    /// <summary>
    /// Constructor
    /// </summary>
	public Index(InventoryDataModel inventario, string name)
    {
        InitializeComponent();
        base.Appearing += AppearingEvent;
        lbTitle.Text = name.Short(20);


        this.Inventario = inventario;

        var canLoad = LoadPermissions();

        if (canLoad)
        {
            Reload();
            SuscribeToHub();
        }
        else
        {
            ShowInfo("Lo siento, no tienes permiso para ver el inventario.");
            DisplayAlert("Sin permisos", "Lamentablemente, no tienes los permisos necesarios para acceder y visualizar el inventario. Si crees que esto es un error o necesitas acceso, por favor comunícate con el encargado o supervisor correspondiente para que puedan ayudarte.", "Continuar");
        }
    }




    /// <summary>
    /// Reaccion al permiso actual
    /// </summary>
    private bool LoadPermissions()
    {

        // Si no hay permisos para leer el inventario
        if (!Inventario.MyRol.HasReadPermissions())
        {
            btnEntradas.Hide();
            btnSalidas.Hide();
            btnAdd.Hide();
            return false;
        }


        // No hay permisos para actualizar o crear un producto
        if (!Inventario.MyRol.HasProductUpdatePermissions())
        {
            btnAdd.Hide();
        }

        // No hay permisos para crear / actualizar movimientos
        if (!Inventario.MyRol.HasMovementUpdatePermissions())
        {
            btnEntradas.Hide();
            btnSalidas.Hide();
        }

        return true;

    }




    private void AppearingEvent(object? sender, EventArgs e)
    {
        AppShell.ActualPage = this;
    }




    /// <summary>
    /// Suscribe al Hub
    /// </summary>
    private void SuscribeToHub()
    {
        HubConnection = new InventoryAccessHub(Inventario.ID);
        HubConnection.On += HubConnection_On;
        HubConnection.OnDeleteProducto += HubConnection_OnDeleteProducto;
        HubConnection.OnUpdate += HubConnection_OnUpdate;
    }

    private void HubConnection_OnUpdate(object? sender, ProductDataTransfer e)
    {
        Dispatcher.DispatchAsync(() => {

            ProductObserver.FillWith(e.ProductID, e);
            ProductObserver.Update(e, From.OtherDevice);

        });
    }

    private void HubConnection_OnDeleteProducto(object? sender, int e)
    {
        Dispatcher.DispatchAsync(() => {

            var modelo = new ProductDataTransfer
            {
                ProductID = e,
                Estado = ProductBaseStatements.Deleted
            };

            ProductObserver.Update(modelo,From.OtherDevice);
        
        });
    }




    /// <summary>
    /// Operacion de cargar
    /// </summary>
    public async void Reload()
    {
        // Prepara la vista de carga
        content.Clear();
        PrepareChargeView();
        await Task.Delay(100);

        // Rellena los datos
        var dataRes = await RetrieveData();

        // Comprueba si se rellenaron los datos
        if (!dataRes)
        {
            indicador.Hide();
            lbInfo.Text = "Hubo un pequeño error";
            lbInfo.Show();
            return;
        }

        // Carga los controles
        BuildControls(Modelos);

        // Carga los controles a la vista
        RenderControls(Controles);

        // Calcula las metricas
        Calculate();

        // Muestra el mensaje
        indicador.Hide();
        lbInfo.Show();
    }



    /// <summary>
    /// Obtiene informacion desde el servidor
    /// </summary>
    private async Task<bool> RetrieveData()
    {
        // Items
        var response = await Access.Inventory.Controllers.Product.ReadAll(Inventario.ID, Session.Instance.Token);

        // Análisis de respuesta
        if (response.Response != Responses.Success)
            return false;

        // Rellena los items
        Modelos = response.Models.OrderBy(model => model.Name).ToList();
        return true;

    }



    /// <summary>
    /// Renderiza los controles a la vista
    /// </summary>
    private async void RenderControls(List<Controls.Product> lista)
    {

        // Vacia los elementos
        content.Clear();

        // Mensaje
        ShowQuantityInfo(lista.Count);



        // Agrega los controles
        int counter = 0;
        foreach (var control in lista)
        {
            RenderOneControl(control);
            counter++;
            if (counter == 50)
            {
                await Task.Delay(100);
                counter = 0;
            }
        }

    }



    /// <summary>
    /// Renderiza los controles a la vista
    /// </summary>
    private void RenderOneControl(Controls.Product control, bool animate = false)
    {
        control.Show();
        content.Add(control);
    }



    /// <summary>
    /// Construlle los controles apartir de una lista de modelos
    /// </summary>
    private void BuildControls(List<ProductDataTransfer> lista)
    {

        // Limpia los controles
        Controles.Clear();

        // Agrega los controles
        foreach (var model in lista)
        {
            var control = BuildOneControl(model);
            Controles.Add(control);
        }

    }



    /// <summary>
    /// Construlle un control
    /// </summary>
    private  Controls.Product BuildOneControl(ProductDataTransfer modelo)
    {
        var control = new Controls.Product(modelo ?? new());
        control.Clicked += (sender, e) =>
        {
            new ViewItem(control.Modelo, Inventario, HubConnection).Show();
        };
        return control;
    }



    /// <summary>
    /// Agrega un nuevo modelo (Cache y vista)
    /// </summary>
    public void AppendModel(ProductDataTransfer modelo)
    {
        // Modelo nulo
        if (modelo == null)
            return;

        // Cuenta si existen elementos
        var count = Modelos.Where(element => element.ProductID == modelo.ProductID).Count();

        if (count > 0)
            return;

        // Agrega el nuevo modelo
        Modelos.Add(modelo);

        // Nuevo control
        var control = BuildOneControl(modelo);
        Controles.Add(control);
        RenderOneControl(control);

        // Nuevo mensaje
        ShowInfo($"Se agrego a '{modelo.Name}' a la lista.");

        // Recarga las metricas
        Calculate();

    }



    /// <summary>
    /// Prepara la vista de carga
    /// </summary>
    private void PrepareChargeView()
    {
        lbInfo.Hide();
        indicador.Show();
        content.Clear();
        displayConStock.Contenido = "Cargando...";
        displayLimitadoStock.Contenido = "Cargando...";
        displayNoStock.Contenido = "Cargando...";
        cardProducto.Contenido = "0";
        cardProducto.ChartText = "Cargando...";
    }



    /// <summary>
    /// Muestra un mensaje de informacion
    /// </summary>
    private void ShowInfo(string message)
    {
        indicador.Hide();
        lbInfo.Show();
        lbInfo.Text = message ?? "";
    }



    /// <summary>
    /// Muestra un mensaje de informacion de cantidad
    /// </summary>
    private void ShowQuantityInfo(int cantidad)
    {
        // No hay elementos
        if (cantidad <= 0)
        {
            ShowInfo("No hay nada por aqui");
            return;
        }

        // Solo uno
        else if (cantidad == 1)
        {
            ShowInfo("Se encontro 1 elemento");
            return;
        }

        // Mas de uno
        ShowInfo($"Se encontraron {cantidad} elementos");

    }



    /// <summary>
    /// Calcula algunas metricas
    /// </summary>
    private async void Calculate()
    {

        displayNoStock.Contenido = "Cargando...";
        displayConStock.Contenido = "Cargando...";
        displayLimitadoStock.Contenido = "Cargando...";

        cardProducto.Contenido = $"{Modelos.Count}";
        cardProducto.ChartText = $"{Modelos.Where(T=>T.Quantity == 0).Count()} agotados.";
        await Task.Delay(1);

        var SinStock = 0;
        var LimitadoStock = 0;
        var ConStock = 0;
        foreach (var item in Modelos)
        {
            if (item.Quantity <= 0)
            {
                SinStock++;
                continue;
            }
            else if (item.Quantity > 0 & item.Quantity < 10)
            {
                LimitadoStock++;
                continue;
            }

            ConStock++;

        }



        displayNoStock.Contenido = $"{SinStock} productos";
        displayLimitadoStock.Contenido = $"{LimitadoStock} productos";
        displayConStock.Contenido = $"{ConStock} productos";

    }



    /// <summary>
    /// Busqueda de un contacto
    /// </summary>
    private async void Searh(string pattern)
    {

        // Prepara la vista
        PrepareChargeView();
        await Task.Delay(1);


        // Patron nulo o vacio
        if (pattern == null || pattern.Trim() == "")
        {
            // Renderiza los controles
            RenderControls(Controles);
            ShowQuantityInfo(Controles.Count);
            return;
        }

        // Normaliza el patron
        pattern = pattern.Trim().ToLower().Normalize();

        // Encuentra los elementos por medio del patron
        var items = (from A in Controles
                     where A.Modelo.Name.ToLower().Contains(pattern) ||
                        A.Modelo.Category.ToString().ToLower().Contains(pattern)
                     select A).ToList();

        // Si no hay elementos
        if (!items.Any())
        {
            ShowInfo($"No se encontraron resultados para '{pattern}'");
            RenderControls(new());
            return;
        }


        // Renderiza los elementos de la busqueda
        RenderControls(items);
        ShowInfo($"Resultados para {pattern}");

    }




    //******** Eventos ********//


    /// <summary>
    /// Cuando se recibe informacion del HUB
    /// </summary>
    private void HubConnection_On(object? sender, ProductDataTransfer e)
    {
        this.Dispatcher.DispatchAsync(() =>
        {
            AppendModel(e);
        });
    }



    /// <summary>
    /// BTN ADD
    /// </summary>
    private void btnAdd_Clicked(object sender, EventArgs e)
    {
        new Add(Inventario.ID, HubConnection).Show();
    }



    /// <summary>
    /// BTN Update
    /// </summary>
    private void btnUpdate_Clicked(object sender, EventArgs e)
    {
        Reload();
    }



    /// <summary>
    /// Boton de entradas
    /// </summary>
    private void BtnEntradas_Clicked(object sender, EventArgs e)
    {
        new Inflows.Index(Inventario.ID, HubConnection).Show();
    }



    /// <summary>
    /// Boton de salidas
    /// </summary>
    private void BtnSalidas_Clicked(object sender, EventArgs e)
    {
        new Outflows.Index(Inventario.ID, HubConnection).Show();
    }

    private void btnIntegrantes_Clicked(object sender, EventArgs e)
    {
        new Views.Inventorys.Integrantes(Inventario).Show();
    }
}