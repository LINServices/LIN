namespace LIN.UI.Popups;


/// <summary>
/// Selector de un usuario
/// </summary>
public partial class UserSelector : Popup
{


    /// <summary>
    /// Elemento seleccionado
    /// </summary>
    private readonly List<SessionModel<ProfileModel>> SelectedItems = new();



    /// <summary>
    /// Constructor
    /// </summary>
    public UserSelector()
    {
        InitializeComponent();
        this.CanBeDismissedByTappingOutsideOfPopup = false;
    }




    /// <summary>
    /// Busca un usuario
    /// </summary>
    private async void Buscar(string pattern)
    {

        // Prepara la vista
        content.Clear();
        indicador.Show();
        displayInfo.Hide();

        if (pattern.Trim().Length <= 0)
        {
            indicador.Hide();
            displayInfo.Show();
            displayInfo.Text = $"Ingresa un usuario valido";
            return;
        }

        // Encuentra el usuario
        var user = await LIN.Access.Inventory.Controllers.Profile.SearhByPattern(pattern, Session.Instance.AccountToken);


        // Analisis de respuesta
        switch (user.Response)
        {
            case Responses.Success:
                break;

            case Responses.InvalidUser:
                indicador.Hide();
                displayInfo.Show();
                displayInfo.Text = $"Hubo un error con tu cuenta.";
                return;

            case Responses.InvalidParam:
                indicador.Hide();
                displayInfo.Show();
                displayInfo.Text = $"El texto es invalido";
                return;

            case Responses.NotRows:
                indicador.Hide();
                displayInfo.Show();
                displayInfo.Text = $"No se encontraron resultados para '{pattern}'";
                return;

            default:
                indicador.Hide();
                displayInfo.Show();
                displayInfo.Text = $"Hubo un error";
                return;

        }

        
        // Renderiza una lista de modelos
        RenderModels(user.Models);

        // Vista
        indicador.Hide();
        displayInfo.Show();
        displayInfo.Text = $"Resultados para '{pattern}'";

    }



    /// <summary>
    /// Renderiza una lista de modelos
    /// </summary>
    private void RenderModels(List<SessionModel<ProfileModel>> models)
    {
        // Recorre los modelos
        foreach (var item in models)
        {

            // Obtiene la cantidad de veces que se repite el modelo en los seleccionados
            var hasSelectedCount = SelectedItems.Where(T => T.Profile.ID == item.Profile.ID).Count();

            // Carga el modelo a la vista
            var control = new Controls.UserForPick(item)
            {
                Margin = new(0, 5, 0, 0)
            };

            // Evento click sobre el control de Pick
            control.Clicked += PickItemClick!;

            if (hasSelectedCount > 0)
            {
                control.Select();
            }

            // Agrega el control a la vista
            content.Add(control);

        }
    }



    // <summary>
    // Evento click sobre Pick
    // </summary>
    private void PickItemClick(object sender, EventArgs e)
    {

        // Control
        UserForPick control = (UserForPick)sender;

        // Accion si no esta seleccionado
        var finder = SelectedItems.Where(T => T.Profile.ID == control.Modelo.Profile.ID).ToList();
        if (!control.IsSelected & finder.Count <= 0)
        {
            var obj = (UserForPick)sender;
            SelectedItems.Add(obj?.Modelo ?? new());
            obj?.Select();
            return;
        }

        // deselecciona los items
        foreach (var item in finder)
        {
            control.UnSelect();
            SelectedItems.Remove(item);
        }


    }



   // <summary>
    // Boton de cancelar
    // </summary>
    private void BtnCancelClick(object sender, EventArgs e)
    {
        this.Close(new List<SessionModel<ProfileModel>>());
    }



    // <summary>
    // Boton de aceptar
    // </summary>
    private void BtnSelectClick(object sender, EventArgs e)
    {
        if (SelectedItems.Count <= 0)
        {
            displayInfo.Text = "Selecciona minimo un usuario";
            return;
        }

        this.Close(SelectedItems);
    }



    /// <summary>
    /// Boton de buscar
    /// </summary>
    private void ButtonBuscarClick(object sender, EventArgs e)
    {
        Buscar(buscador.Text ?? "");
    }



    // <summary>
    // Boton de listar
    // </summary>
    private async void ButtonListar_Clicked(object sender, EventArgs e)
    {
        // Elementos seleccionados
        content.Clear();
        indicador.Show();
        displayInfo.Hide();
        await Task.Delay(10);

        // Renderiza la lista
        RenderModels(SelectedItems);

        indicador.Hide();
        displayInfo.Show();
        displayInfo.Text = $"{SelectedItems.Count} elementos seleccionados";

    }


}