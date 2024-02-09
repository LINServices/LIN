using LIN.Access.Inventory.Controllers;
using LIN.Services;

using LIN.UI.Popups;

namespace LIN.UI.Views.Inventorys;


public partial class IntegrantsAdd : ContentPage
{


    //===== Propiedades =====//


    /// <summary>
    /// Nuevos participantes
    /// </summary>
    private List<SessionModel<ProfileModel>> Participantes { get; set; } = new();


    /// <summary>
    /// Modelo del inventario
    /// </summary>
    private InventoryDataModel Inventario { get; set; } = new();    




    /// <summary>
    /// Constructor
    /// </summary>
    public IntegrantsAdd(InventoryDataModel modelo)
    {
        InitializeComponent();
        this.Appearing += Add_Appearing;
        this.Inventario = modelo;
        Render();
    }




    /// <summary>
    /// Muestra los datos visibles
    /// </summary>
    private void Render()
    {
        displayNombre.Text = Inventario.Nombre;
        displayDireccion.Text = Inventario.Direccion;
        displayRol.Text = Inventario.MyRol.Humanize();
    }




    /// <summary>
    /// Evento click sobre el boton de crear
    /// </summary>
    private async void ButtonCrearClick(object sender, EventArgs e)
    {

        // Organiza la vista
        lbInfo.Hide();
        btn.Hide();
        indicador.Show();
        await Task.Delay(10);

        // Retorna
        if (Participantes.Count <= 0)
        {
            ShowMessage("Debe haber almenos 1 nuevo integrante");
            btn.Show();
            return;
        }





        // Creacion del modelo
        var modelo = new InventoryDataModel
        {
            ID = Inventario.ID,
            UsersAccess = new()
        };


        List<int> ids = new();

        foreach (var integrante in Participantes)
        {
            var model = new InventoryAcessDataModel()
            {
                Rol = InventoryRoles.Member,
                ProfileID = integrante.Profile.ID
            };
            modelo.UsersAccess.Add(model);
            ids.Add(integrante.Profile.ID);
        }



       

        // Respuesta del controlador
        var response = await Inventories.GenerateInvitaciones(modelo, Session.Instance.Token);


        if (response.Response != Responses.Success)
        {
            ShowMessage("Hubo un error");
            btn.Show();
            return;
        }




        // ---- EVENTO DEL HUB
        //AppShell.Hub.SendNotificacion(ids);

        // Muestra el popup de agregado
        await this.ShowPopupAsync(new DefaultPopup());

        indicador.Hide();
        btn.Show();
        
        Participantes.Clear();

    }




    /// <summary>
    /// Evento (Appearing)
    /// </summary>
    private void Add_Appearing(object? sender, EventArgs e)
    {
        AppShell.ActualPage = this;
    }



    private void ShowMessage(string message)
    {
        lbInfo.Show();
        lbInfo.Text = message ?? "";
        indicador.Hide();
    }

    private async void ToggleButton_Clicked(object sender, EventArgs e)
    {

        var xx = (List<SessionModel<ProfileModel>>?)(await new UserSelector().Show());

        if (xx == null)
            return;

        foreach(var modelo in xx)
        {
            var usercontrol = new Controls.UserForPick(modelo);
            conte.Add(usercontrol);
            usercontrol.Show();
            Participantes.Add(modelo);
        }

        
    
    }
}