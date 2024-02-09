
namespace LIN.UI.Views.Inflows;

public partial class Index : ContentPage
{

    //******* HUB *******//

    /// <summary>
    /// Hub de SignalR
    /// </summary>
    private InventoryAccessHub? Hub;



    /// <summary>
    /// Id del inventario
    /// </summary>
    private int InventarioID { get; set; }



    //******* Propiedades *******//

    /// <summary>
    /// Modelos
    /// </summary>
    private List<InflowDataModel> Modelos { get; set; } = new();



    /// <summary>
    /// Controles
    /// </summary>
    private List<Inflow> Controles = new();





    /// <summary>
    /// Constructor
    /// </summary>
    public Index(int inventario, InventoryAccessHub? hub)
    {
        InitializeComponent();
        this.Hub = hub;
        this.InventarioID = inventario;

        //if (hub != null)
        //    hub.OnReciveInflow += Hub_OnReciveInflow;

        Load();
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
        var modelos = await Access.Inventory.Controllers.Inflows.ReadAll(InventarioID);

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
    private async void RenderControls(List<Inflow> lista)
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
    private void RenderOneControl(Inflow control)
    {
        control.Show();
        content.Insert(0, control);
    }



    /// <summary>
    /// Construlle los controles apartir de una lista de modelos
    /// </summary>
    private void BuildControls(List<InflowDataModel> lista)
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
    private static Inflow BuildOneControl(InflowDataModel modelo)
    {
        var control = new Controls.Inflow(modelo ?? new());
        control.Clicked += (obj, args) =>
        {
            new ViewItem(modelo ?? new()).Show();
        };
        return control;
    }



    /// <summary>
    /// Agrega un nuevo modelo (Cache y vista)
    /// </summary>
    public void AppendModel(InflowDataModel modelo)
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
        ShowInfo($"Se agrego una nueva entrada");

        // Recarga las metricas
        Calculate();

    }



    /// <summary>
    /// Prepara la vista de carga
    /// </summary>
    private void PrepareChargeView()
    {
        // Info
        indicador.Show();
        lbInfo.Hide();

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

        displayRegalos.Contenido = Modelos.Where(T => T.Type == InflowsTypes.Regalo).Count().ToString() + " elementos";
        displayCompras.Contenido = Modelos.Where(T => T.Type == InflowsTypes.Compra).Count().ToString() + " elementos";
        displayDevolucion.Contenido = Modelos.Where(T => T.Type == InflowsTypes.Devolucion).Count().ToString() + " elementos";

        var elementsToday = Modelos.Where(T => T.Date.IsToday());

        var count = elementsToday.Count();
        int c = 0;

        foreach (var e in elementsToday)
        {
            c += e.CountDetails;
        }

        cardEntradas.ChartText = $"{c} Productos";
        cardEntradas.Contenido = count.ToString();

    }



    /// <summary>
    /// Limpia la interfaz
    /// </summary>
    private void ClearInterface()
    {
        lbInfo.Hide();
        indicador.Hide();
        content.Clear();
        displayCompras.Contenido = "Cargando...";
        displayDevolucion.Contenido = "Cargando...";
        displayRegalos.Contenido = "Cargando...";
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
            ShowInfo($"Se encontraron {Controles.Count} elementos.");
            lbInfo.Show();
            indicador.Hide();
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
            lbInfo.Text = $"No se encontraron resultados para '{pattern}'";
            RenderControls(new());
            return;
        }


        // Renderiza los elementos
        RenderControls(items);
        ShowInfo($"Resultados para '{pattern}'");

    }




    //******* Eventos *******//


    /// <summary>
    /// Cuando se recibe una nueva entrada (Realtime)
    /// </summary>
    private void Hub_OnReciveInflow(object? sender, InflowDataModel e)
    {
        Dispatcher.DispatchAsync(() =>
        {
            AppendModel(e);
        });
    }



    /// <summary>
    /// Boton de agregar
    /// </summary>
    private void btnAdd_Clicked(object sender, EventArgs e)
    {
        new AddItem(InventarioID, Hub).Show();
    }



    /// <summary>
    /// Boton actualizar
    /// </summary>
    private void btnUpdate_Clicked(object sender, EventArgs e)
    {
        Load();
    }



    /// <summary>
    /// Evento de busqueda
    /// </summary>
    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        Searh(e.NewTextValue);
    }


}