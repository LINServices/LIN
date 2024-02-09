


namespace LIN.UI.Views.Outflows;

public partial class Index : ContentPage
{


    /// <summary>
    /// Hub de SignalR
    /// </summary>
    private InventoryAccessHub? Hub;



    /// <summary>
    /// Lista de modelos
    /// </summary>
    private List<OutflowDataModel> Modelos { get; set; } = new();



    /// <summary>
    /// Lista de controles
    /// </summary>
    private List<Controls.Outflow> Controles { get; set; } = new();



    /// <summary>
    /// Id del inventario
    /// </summary>
    public int InventarioID { get; set; }



    /// <summary>
    /// Constructor
    /// </summary>
	public Index(int inventario, InventoryAccessHub? hub)
    {
        InitializeComponent();
        this.InventarioID = inventario;
        this.Hub = hub;

        //if (Hub != null)
        //    Hub.OnReciveOutflow += Hub_OnReciveOutflow;

        Load();
    }



    private void Hub_OnReciveOutflow(object? sender, OutflowDataModel e)
    {
        Dispatcher.DispatchAsync(() =>
        {
            AppendModel(e);
        });
    }




    /// <summary>
    /// Operacion de cargar
    /// </summary>
    public async void Load()
    {
        // Prepara la vista de carga
        content.Clear();
        PrepareChargeView();

        // Rellena los datos
        var dataRes = await ReceiveData();

        // Comprueba si se rellenaron los datos
        if (dataRes != Responses.Success)
        {
            ShowInfo("Hubo un pequeño error");
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
    /// Obtiene los nuevos datos
    /// </summary>
    private async Task<Responses> ReceiveData()
    {

        // respuesta desde la API
        var modelos = await Access.Inventory.Controllers.Outflows.ReadAll(InventarioID);

        // Validacion
        if (modelos.Response != Responses.Success)
            return modelos.Response;


        // Modelos
        Modelos = new();
        Modelos.AddRange(modelos.Models);

        // Correcto
        return Responses.Success;
    }



    /// <summary>
    /// Renderiza los controles a la vista
    /// </summary>
    private async void RenderControls(List<Controls.Outflow> lista)
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
    private void RenderOneControl(Controls.Outflow control)
    {
        control.Show();
        content.Insert(0, control);
    }



    /// <summary>
    /// Construlle los controles apartir de una lista de modelos
    /// </summary>
    private void BuildControls(List<OutflowDataModel> lista)
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
    /// Renderiza un control
    /// </summary>
    private Controls.Outflow BuildOneControl(OutflowDataModel modelo)
    {
        var control = new Controls.Outflow(modelo ?? new());
        control.Clicked += (obj, args) =>
        {
            new ViewItem(modelo ?? new()).Show();
        };
        return control;
    }



    /// <summary>
    /// Agrega un nuevo modelo (Cache y vista)
    /// </summary>
    public void AppendModel(OutflowDataModel modelo)
    {
        // Modelo nulo
        if (modelo == null)
            return;

        // Cuenta si existen elementos
        var count = Modelos.Where(element => element.ID == modelo.ID).Count();

        if (count > 0)
            return;

        // Agrega el nuevo modelo
        Modelos.Add(modelo);

        // Nuevo control
        var control = BuildOneControl(modelo);
        Controles.Add(control);
        RenderOneControl(control);

        // Nuevo mensaje
        ShowInfo($"Se agrego una salida a la lista.");

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
        displayDonacion.Contenido = "Cargando...";
        displayLost.Contenido = "Cargando...";
        displayVentas.Contenido = "Cargando...";
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
    private void Calculate()
    {

        var donaciones = Modelos.Where(T => T.Type == OutflowsTypes.Donacion).Count();
        var ventas = Modelos.Where(T => T.Type == OutflowsTypes.Venta).Count();
        var perdidas = Modelos.Where(T => T.Type != OutflowsTypes.Venta || T.Type != OutflowsTypes.Donacion).Count();

        displayLost.Contenido = perdidas.ToString() + " perdidas.";
        displayVentas.Contenido = ventas.ToString() + " ventas.";
        displayDonacion.Contenido = donaciones.ToString() + " donaciones.";

        var elementsToday = Modelos.Where(T => T.Date.IsToday());

        var count = elementsToday.Count();
        int c = 0;

        foreach (var e in elementsToday)
            c += e.CountDetails;


        cardEntradas.ChartText = $"{c} Productos";
        cardEntradas.Contenido = count.ToString();


    }



    /// <summary>
    /// Busqueda de un elemento
    /// </summary>
    private async void Searh(string pattern)
    {

        // Prepara la vista
        lbInfo.Hide();
        indicador.Show();
        await Task.Delay(1);


        // Patron nulo o vacio
        if (pattern == null || pattern.Trim() == "")
        {
            RenderControls(Controles);
            ShowQuantityInfo(Controles.Count);
            return;
        }


        // Normaliza el patron
        pattern = pattern.Trim().ToLower().Normalize();


        // Encuentra los elementos por medio del patron
        var items = (from A in Controles
                     where A.Modelo.Type.ToString().Contains(pattern)
                     select A).ToList();

        // Si no hay elementos del patron
        if (!items.Any())
        {
            RenderControls(new());
            ShowInfo($"No se encontraron resultados para '{pattern}'");
            return;
        }


        // Renderiza los elementos
        RenderControls(items);
        ShowInfo($"Resultados para '{pattern}'");

    }



    /// <summary>
    /// Boton de agregar
    /// </summary>
    private void btnAdd_Clicked(object sender, EventArgs e)
    {
        new AddItem(InventarioID, Hub).Show();
    }



    /// <summary>
    /// Boton de actualizar
    /// </summary>
    private void btnUpdate_Clicked(object sender, EventArgs e)
    {
        Load();
    }



    /// <summary>
    /// Evento busqueda
    /// </summary>
    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        Searh(e.NewTextValue);
    }


}