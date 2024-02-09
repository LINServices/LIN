using LIN.Access.Inventory.Controllers;
using LIN.UI.Popups;

namespace LIN.UI.Views.Inventorys;

public partial class AddInventory : ContentPage
{

    /// <summary>
    /// Lista de participantes
    /// </summary>
    private readonly List<SessionModel<ProfileModel>> Participantes = new();



    /// <summary>
    /// Constructor
    /// </summary>
    public AddInventory()
    {
        InitializeComponent();
        this.Appearing += Add_Appearing;
    }



    /// <summary>
    /// Evento Click sobre el selector de contactos
    /// </summary>
    private async void ContactMini_Clicked(object sender, EventArgs e)
    {
        
        // Nuevo popup
        var pop = new UserSelector();
        var models = (List<SessionModel<ProfileModel>>?)await this.ShowPopupAsync(pop) ?? new();

        // Recorre los resultados
        foreach (var model in models)
        {
            // Busca usuarios repetidos
            var found = Participantes.Where(T => T.Profile.ID == model.Profile.ID).Count();

            if (found > 0)
                continue;

            // Agrega el modelo a la vista
            Participantes.Add(model);
            var x = new Controls.UserForPick(model);
            conte.Add(x);

        }

    }



    /// <summary>
    /// Comprueba si los datos estan completos
    /// </summary>
    private bool IsDataComplete()
    {

        // Name
        if (string.IsNullOrWhiteSpace(txtName.Text))
            return false;

        // Descripcion
        if (string.IsNullOrWhiteSpace(txtDireccion.Text))
            return false;


        return true;

    }



    /// <summary>
    /// Evento click sobre el boton de crear
    /// </summary>
    private async void Button_Clicked(object sender, EventArgs e)
    {
        // Organiza la vista
        lbInfo.Hide();
        btn.Hide();
        indicador.Show();

        // Si los datos estan incompletos
        var isComplete = IsDataComplete();

        // Retorna
        if (!isComplete)
        {
            lbInfo.Show();
            lbInfo.Text = "Por favor, aseg�rate de llenar todos los campos requeridos.";
            indicador.Hide();
            btn.Show();
            return;
        }


        // Creacion del modelo
        var modelo = new InventoryDataModel()
        {
            Nombre = txtName.Text,
            Direccion = txtDireccion.Text,
            Creador = Session.Instance.Informacion.ID,
            UltimaModificacion = DateTime.Now
        };


        List<int> notificationList = new();
        // Participantes
        {

            // Acceso del usuario creador
            modelo.UsersAccess.Add(new()
            {
                ProfileID = Session.Instance.Informacion.ID
            });

            // Otros participantes
            foreach (var user in Participantes)
            {
                notificationList.Add(user.Profile.ID);
                modelo.UsersAccess.Add(new()
                {
                    ProfileID = user.Profile.ID,
                    Rol = InventoryRoles.Member
                });
            }

        }


        // Respuesta del controlador
        var response = await Inventories.Create(modelo, Session.Instance.Informacion.ID);


        // Organizacion de la interfaz
        indicador.Hide();
        btn.Show();

        if (response.Response != Responses.Success)
        {
            lbInfo.Text = response.ToString();
            lbInfo.Show();
            return;
        }



        //AppShell.Hub.SendNotificacion(notificationList);

        // Muestra el popup de agregado
        await this.ShowPopupAsync(new Popups.DefaultPopup());


    }


    /// <summary>
    /// Evento (Appearing)
    /// </summary>
    private void Add_Appearing(object? sender, EventArgs e)
    {
        AppShell.ActualPage = this;
    }

    private void ToggleButton_Clicked(object sender, EventArgs e)
    {
        conte.Clear();
        Participantes.Clear();
    }
}