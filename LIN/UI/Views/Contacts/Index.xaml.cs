

using LIN.Types.Contacts.Models;

namespace LIN.UI.Views.Contacts;


public partial class Index : ContentPage
{

    /// <summary>
    /// Lista de modelos
    /// </summary>
    private List<ContactModel> Modelos { get; set; } = new();


    /// <summary>
    /// Lista de los contactos
    /// </summary>
    private List<Controls.Contact> Controles { get; set; } = new();




    /// <summary>
    /// Constructor
    /// </summary>
	public Index()
    {
        InitializeComponent();
        Load();
        SuscribeToHub();


        base.Disappearing += DisappearingEvent;
        base.Appearing += AppearingEvent;


    }





    /// <summary>
    /// Recaraga y rendira nueva informacion
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

        // Carga los controles que estan normal
        RenderControls(Controles);

        // Calcula las metricas
        Calculate();

        // Muestra el mensaje
        indicador.Hide();
        lbInfo.Show();
    }



    #region Administracion



    /// <summary>
    /// Suscribe al Hub
    /// </summary>
    private void SuscribeToHub()
    {
        if (AppShell.Hub == null)
            return;

       // AppShell.Hub.OnReceiveContact += HubConnection_On;
    }



    #endregion



    #region Datos


    /// <summary>
    /// Obtiene datos desde el servidor
    /// </summary>
    private async Task<Responses> ReceiveData()
    {

        // respuesta desde la API
        var contactos = await Access.Inventory.Controllers.Contact.ReadAll(Session.Instance.ContactsToken);

        // Validacion
        if (contactos.Response != Responses.Success)
            return contactos.Response;

        // Modelos
        Modelos.Clear();
        Modelos.AddRange(contactos.Models.OrderBy(model => model.Nombre).ToList());

        // Correcto
        return Responses.Success;
    }



    #endregion



    #region Renders



    /// <summary>
    /// Renderiza los contactos que tengan el estado NORMAL
    /// </summary>
    private void RenderNormals()
    {
        var elementos = Controles;
        RenderControls(elementos);
        ShowQuantityInfo(elementos.Count());
    }



    /// <summary>
    /// Renderiza los contactos que tengan el estado 'En La Basura'
    /// </summary>
    private void RenderTrash()
    {
        var elementos = Controles;
        RenderControls(elementos);
        ShowInfo($"{elementos.Count()} contactos en la papelera");
    }



    /// <summary>
    /// Renderiza los controles a la vista
    /// </summary>
    private async void RenderControls(IEnumerable<Controls.Contact> lista)
    {

        // Vacia los elementos
        content.Clear();

        // Mensaje
        ShowQuantityInfo(lista.Count());

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
    private void RenderOneControl(Controls.Contact control)
    {
        control.Show();
        content.Add(control);
        control.TranslationX = 0;
        control.Scale = 1;
    }



    /// <summary>
    /// Construlle los controles apartir de una lista de modelos
    /// </summary>
    private void BuildControls(List<ContactModel> lista)
    {

        // Limpia los controles
        Controles.Clear();

        // Agrega los controles
        foreach (var model in lista)
        {
            // Construlle el control
            var control = BuildOneControl(model);

            if (control != null)
                Controles.Add(control);
        }

    }



    /// <summary>
    /// construlle un control
    /// </summary>
    private Controls.Contact? BuildOneControl(ContactModel modelo)
    {
        if (modelo != null)
        {
            var control = new Controls.Contact(modelo);
            control.Clicked += (obj, args) =>
            {
                var pop = new Popups.ContactPopup(modelo);
                this.ShowPopupAsync(pop);
            };
            return control;
        }

        return null;
    }



    #endregion










































    /// <summary>
    /// Agrega un nuevo modelo (Cache y vista)
    /// </summary>
    public void AppendModel(ContactModel modelo)
    {

        // Modelo nulo
        if (modelo == null || ContactObserver.Update(modelo))
            return;

        // Cuenta si existen elementos
        var existings = Modelos.Where(element => element.Id == modelo.Id).ToList();

        if (existings.Count > 0)
        {

            var existControls = Controles.Where(T => T.Modelo.Id == modelo.Id);

            foreach (var ex in existControls)
                ex.Hide();

            var newControl = BuildOneControl(modelo);
            RenderOneControl(newControl!);

            return;
        }



        // Agrega el nuevo modelo
        Modelos.Add(modelo);

        // Nuevo control
        var control = BuildOneControl(modelo);
        Controles.Add(control!);
        RenderOneControl(control!);

        // Nuevo mensaje
        ShowInfo($"Se agrego a '{modelo.Nombre}' a la lista.");

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

        cardCantidad.Contenido = "0";
        cardCantidad.ChartText = "Cargando...";

        displayPapelera.Contenido = "Cargando...";

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

        // Si no hay productos
        if (!Modelos.Any())
        {
            ShowInfo("No hay nada que mostrar aqui");
            return;
        }

        var count = Controles.Count();
        displayPapelera.Contenido = $"{count} elementos";
        cardCantidad.Contenido = $"{Modelos.Count}";
        //cardCantidad.ChartText = $"{Modelos.Where(T => T.State == ContactStatus.OnTrash).Count()} en papelera";
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
                     where A.Modelo.Nombre.ToLower().Contains(pattern)
                     select A).ToList();

        // Si no hay elementos
        if (!items.Any())
        {
            ShowInfo($"No se encontraron resultados para '{pattern}'");
            RenderControls(new List<Controls.Contact>());
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
    private void HubConnection_On(object? sender, ContactModel e)
    {
        try
        {

#if ANDROID

            MainThread.BeginInvokeOnMainThread(() =>
                       {
                           AppendModel(e);
                       });

#elif WINDOWS
            this.Dispatcher.Dispatch(() =>
                   {
                       AppendModel(e);
                   });
#endif
        }
        catch (Exception ex)
        {
            var s = ex.Message;
        }

    }



    /// <summary>
    /// Boton de agregar
    /// </summary>
    private void ButtonADD_Click(object sender, EventArgs e)
    {
        new Add().Show();
    }



    /// <summary>
    /// Boton de actualizar
    /// </summary>
    private void ButtonUpdate_Click(object sender, EventArgs e)
    {
        Load();
    }



    /// <summary>
    /// click sobre el boton de papelera
    /// </summary>
    bool isPapelera = false;
    private void DisplayPapelera_Click(object sender, EventArgs e)
    {

        if (!isPapelera)
            RenderTrash();

        else
            RenderNormals();

        isPapelera = !isPapelera;

    }



    /// <summary>
    /// Pagina Aparece
    /// </summary>
    private void AppearingEvent(object? sender, EventArgs e)
    {
        AppShell.ActualPage = this;
    }



    /// <summary>
    /// Pagina Desparece
    /// </summary>
    private void DisappearingEvent(object? sender, EventArgs e)
    {

    }


}