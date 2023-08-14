namespace LIN.UI.Popups;


public partial class ContactSelector : Popup
{


    /// <summary>
    /// Lista de modelos seleccionados
    /// </summary>
    private List<ContactDataModel> SelectedItems = new();


    /// <summary>
    /// Lista de modelos
    /// </summary>
    private List<ContactDataModel> Modelos = new();



    /// <summary>
    /// Lista de controles
    /// </summary>
    private List<Controls.ContactForPick> Controls = new();



    /// <summary>
    /// Permite seleccionar varios modelos
    /// </summary>
    private bool SelectMany { get; set; } = false;




    /// <summary>
    /// Constructor
    /// </summary>
    public ContactSelector(bool selectMany)
    {
        InitializeComponent();
        this.CanBeDismissedByTappingOutsideOfPopup = false;
        this.SelectMany = selectMany;
        LoadData();
    }




    /// <summary>
    /// Carga los datos y los controles
    /// </summary>
    private async void LoadData()
    {
        displayInfo.Hide();
        indicador.Show();
        await Task.Delay(10);


        var response = await RetriveData();

        if (!response)
        {
            ShowMessage("Hubo un error");
            return;
        }

        // Si no hay modelos
        if (!Modelos.Any())
        {
            indicador.Hide();
            displayInfo.Show();
            displayInfo.Text = "No hay contactos que mostrar";
            return;
        }


        Controls = BuildControls(Modelos);


        RenderList(Controls);
        displayInfo.Text = $"{Controls.Count} contactos";
        displayInfo.Show();
        indicador.Hide();
    }



    /// <summary>
    /// Obtiene los datos desde el servidor
    /// </summary>
    private async Task<bool> RetriveData()
    {

        // ID de la cuenta
        var id = Session.Instance.Informacion.ID;

        // Respuesta
        var response = await Access.Inventory.Controllers.Contact.ReadAll(id);

        // Evalua
        if (response.Response != Responses.Success)
            return false;

        // Organiza los modelos
        Modelos = response.Models.Where(T=>T.State == ContactStatus.Normal).OrderBy(x => x.Name).ToList();

        return true;
    }









    private void UnSelectAllExept(Controls.ContactForPick? excep)
    {
        foreach (var view in Controls)
        {
            if (view != excep)
                view.UnSelect();
        }
    }



    /// <summary>
    /// Renderiza una lista de controles
    /// </summary>
    private void RenderList(List<Controls.ContactForPick> lista)
    {
        content.Clear();
        foreach (var view in lista)
            content.Add(view);
    }

    private void BtnCancelClick(object sender, EventArgs e)
    {
        Controls.Clear();
        this.Close(SelectedItems);
    }

    private void BtnSelectClick(object sender, EventArgs e)
    {
        if (SelectedItems == null || SelectedItems.Count <= 0)
        {
            displayInfo.Text = "Selecciona un contacto";
            return;
        }

        this.Close(SelectedItems);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        this.Close();
        new Views.Contacts.Add().Show();
    }



    private void ShowMessage(string message)
    {
        displayInfo.Text = message ?? "";
        displayInfo.Show();
        indicador.Hide();
    }








    private List<Controls.ContactForPick> BuildControls(List<ContactDataModel> modelos)
    {
        List<Controls.ContactForPick> controles = new();
        foreach (var modelo in modelos)
            controles.Add(BuildControl(modelo));

        return controles;
    }


    private Controls.ContactForPick BuildControl(ContactDataModel modelo)
    {
        var control = new Controls.ContactForPick(modelo)
        {
            Margin = new(0, 3, 0, 0)
        };

        control.Clicked += S;

        return control;
    }




    private void S(object? sender, EventArgs e)
    {

        var obj = (Controls.ContactForPick?)sender;


        bool isSelect = SelectedItems.Where(T => T.ID == obj.Modelo.ID).Any();


        if (isSelect)
        {
            obj.UnSelect();
            SelectedItems.RemoveAll(T => T.ID == obj.Modelo.ID);
            return;
        }


        if (!SelectMany)
        {
            SelectedItems.Clear();
            UnSelectAllExept(obj);
        }

        SelectedItems.Add(obj!.Modelo);
        obj?.Select();

    }













}