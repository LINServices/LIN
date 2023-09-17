using LIN.Access.Inventory.Controllers;
using LIN.UI.Popups;

namespace LIN.UI.Views.Inventorys;

public partial class Integrantes : ContentPage
{


    /// <summary>
    /// Lista de modelos
    /// </summary>
    private List<IntegrantDataModel> Modelos { get; set; } = new();


    /// <summary>
    /// Lista de los controles
    /// </summary>
    private List<Controls.Integrant> Controles { get; set; } = new();


    /// <summary>
    /// Modelo del inventario
    /// </summary>
    private InventoryDataModel Inventory { get; set; }



    /// <summary>
    /// Constructor
    /// </summary>
    public Integrantes(InventoryDataModel model)
    {
        InitializeComponent();

        Appearing += AppearingEvent;

        Inventory = model;

        var canLoad = LoadPermissions();

        if (canLoad)
        {
            Reload();
        }
        else
        {
            ShowInfo("Lo siento, no tienes permiso para ver el inventario.");
            DisplayAlert("Sin permisos", "Lamentablemente, no tienes los permisos necesarios para acceder y visualizar el inventario. Si crees que esto es un error o necesitas acceso, por favor comunícate con el encargado o supervisor correspondiente para que puedan ayudarte.", "Continuar");
        }

    }



    private void AppearingEvent(object? sender, EventArgs e)
    {
        AppShell.ActualPage = this;
    }




    /// <summary>
    /// Reaccion al permiso actual
    /// </summary>
    private bool LoadPermissions()
    {


        if (Inventory.MyRol != InventoryRoles.Administrator)
            btnAdd.Hide();
        else
            btnAdd.Show();


        return Inventory.MyRol.HasReadPermissions();

    }





    /// <summary>
    /// Operacion de cargar
    /// </summary>
    public async void Reload()
    {
        // Prepara la vista de carga
        PrepareChargeView();
        content.Clear();
        await Task.Delay(100);


        // Rellena los datos
        var dataRes = await RetrieveData();

        // Comprueba si se rellenaron los datos
        switch (dataRes)
        {
            // Correcto
            case Responses.Success:
                break;

            // Sin permisos
            case Responses.Unauthorized:
                ShowInfo("Sin permisos");
                return;

            default:
                ShowInfo("Hubo unn error");
                return;
        }


        // Carga los controles
        BuildControls(Modelos);

        // Carga los controles a la vista
        RenderControls(Controles);
        Calculate();

        // Muestra el mensaje
        indicador.Hide();
        lbInfo.Show();

    }



    /// <summary>
    /// Obtiene información desde el servidor
    /// </summary>
    private async Task<Responses> RetrieveData()
    {

        var response = await Inventories.GetIntegrants(Inventory.ID, Session.Instance.Informacion.ID, Session.Instance.AccountToken);

        // Análisis de respuesta
        if (response.Response != Responses.Success)
            return response.Response;

        // Rellena los items
        Modelos = response.Models;

        return Responses.Success;

    }



    /// <summary>
    /// Renderiza los controles a la vista
    /// </summary>
    private async void RenderControls(List<Controls.Integrant> lista)
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
    private void RenderOneControl(Controls.Integrant control)
    {
        control.Show();
        content.Add(control);
    }



    /// <summary>
    /// Construlle los controles apartir de una lista de modelos
    /// </summary>
    private void BuildControls(List<IntegrantDataModel> lista)
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
    private Controls.Integrant BuildOneControl(IntegrantDataModel modelo)
    {

        var control = new Controls.Integrant(modelo ?? new(), Inventory.MyRol, Inventory.ID);

        control.OnDelete += async (sender, e) =>
        {

            // Respuesta del usuario
            var delete = await AppShell.ActualPage!.DisplayAlert("Eliminar", $"¿Desea eliminar a {modelo?.Nombre.Trim()} de este inventario?", "Si, Eliminar", "Cancelar");

            if (!delete)
                return;

            var inventario = Inventory.ID;
            var me = Session.Instance.Informacion.ID;
            var user = modelo?.ID ?? 0;

            // Solicitud de eliminar
            var response = await Inventories.DeleteSomeOne(inventario, user, me);


            if (response.Response != Responses.Success)
            {
                await new Popups.DefaultPopup($"No se pudo eliminar a {modelo?.Nombre}", "cerrar.png").Show();
                return;
            }


            await new Popups.DefaultPopup("Correcto").Show();
            this.Reload();

        };


        control.OnRolClick += async (sender, e) =>
        {

            if (Inventory.MyRol != InventoryRoles.Administrator)
                return;

            var result = await new Popups.UserPopup(modelo!).Show();

            if (result is not null and InventoryRoles)
            {
                control.Modelo.Rol = (InventoryRoles)result;
                control.LoadModelVisible();
            }

        };

        return control;
    }



    /// <summary>
    /// Calcula
    /// </summary>
    private void Calculate()
    {
        var count = Modelos.Count;

        cardIntegrant.Contenido = count.ToString();
        cardIntegrant.ChartText = $"{Modelos.Where(T => T.Rol == InventoryRoles.Administrator).Count()} Admins";

    }



    /// <summary>
    /// Agrega un nuevo modelo (Cache y vista)
    /// </summary>
    public void AppendModel(IntegrantDataModel modelo)
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
        ShowInfo($"Se agrego a '{modelo.Nombre}' a la lista.");

    }



    /// <summary>
    /// Prepara la vista de carga
    /// </summary>
    private void PrepareChargeView()
    {
        cardIntegrant.Contenido = "0";
        cardIntegrant.ChartText = "Cargando...";
        indicador.Show();
        lbInfo.Hide();
        content.Clear();
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
            ShowInfo("No hay ninguna persona asociado");
            return;
        }

        // Solo uno
        else if (cantidad == 1)
        {
            ShowInfo("Hay 1 integrante");
            return;
        }

        // Mas de uno
        ShowInfo($"Hay {cantidad} integrantes asociados");

    }









    //******** Eventos ********//



    /// <summary>
    /// Boton de agregar
    /// </summary>
    private void btnAdd_Clicked(object sender, EventArgs e)
    {
        new IntegrantsAdd(Inventory).Show();
    }



    /// <summary>
    /// Boton de actualizar
    /// </summary>
    private void btnUpdate_Clicked(object sender, EventArgs e)
    {
        Reload();
    }


    /// <summary>
    /// Boton de 
    /// </summary>
    private async void btnInformes_Clicked(object sender, EventArgs e)
    {
#if WINDOWS
        new Informes(Inventory.ID).Show();
#elif ANDROID

        var deviceSelector = new DeviceSelector($"openInformesScreen({Inventory.ID})", new(false, true)
        {
            App = new[]
          {
                Applications.Inventory
            },
            Plataformas = new[]
          {
                Platforms.Windows
            }
        });

        var response = await deviceSelector.Show();
#endif
    }




}