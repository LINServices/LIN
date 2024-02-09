namespace LIN.UI.Popups;


public partial class ProductSelector : Popup
{

    //****** Propiedades ******//


    /// <summary>
    /// Lista de modelos seleccionados
    /// </summary>
    private readonly List<ProductModel> SelectedItems = new();


    /// <summary>
    /// Lista de modelos
    /// </summary>
    private List<ProductModel> Modelos = new();



    /// <summary>
    /// Lista de controles
    /// </summary>
    private List<ProductForPick> Controls = new();



    /// <summary>
    /// Permite seleccionar varios modelos
    /// </summary>
    private bool SelectMany { get; set; } = false;



    /// <summary>
    /// Id del inventario
    /// </summary>
    private int Inventario { get; }




    /// <summary>
    /// Constructor
    /// </summary>
    public ProductSelector(int inventario, bool selectMany = false)
    {
        InitializeComponent();
        this.Inventario = inventario;
        this.SelectMany = selectMany;
        this.CanBeDismissedByTappingOutsideOfPopup = false;
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
            displayInfo.Text = "No hay productos";
            return;
        }


        Controls = BuildControls(Modelos);


        RenderList(Controls);
        displayInfo.Text = $"{Controls.Count} productos";
        displayInfo.Show();
        indicador.Hide();
    }



    /// <summary>
    /// Obtiene los datos desde el servidor
    /// </summary>
    private async Task<bool> RetriveData()
    {

        // Id de la cuenta
        var id = Session.Instance.Informacion.ID;

        // Respuesta
        var response = await Access.Inventory.Controllers.Product.ReadAll(Inventario, Session.Instance.Token);

        // Evalua
        if (response.Response != Responses.Success)
            return false;

        // Organiza los modelos
        Modelos = response.Models.OrderBy(x => x.Name).ToList();

        return true;
    }






    private void UnSelectAllExept(Controls.ProductForPick? excep)
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
    private void RenderList(List<Controls.ProductForPick> lista)
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








    private List<Controls.ProductForPick> BuildControls(List<ProductModel> modelos)
    {
        List<Controls.ProductForPick> controles = new();
        foreach (var modelo in modelos)
            controles.Add(BuildControl(modelo));

        return controles;
    }


    private Controls.ProductForPick BuildControl(ProductModel modelo)
    {
        var control = new Controls.ProductForPick(modelo)
        {
            Margin = new(0, 3, 0, 0)
        };

        control.Clicked += S;

        return control;
    }




    private void S(object? sender, EventArgs e)
    {

        var obj = (Controls.ProductForPick?)sender;


        bool isSelect = SelectedItems.Where(T => T.ProductID == obj.Modelo.ProductID).Any();


        if (isSelect)
        {
            obj.UnSelect();
            SelectedItems.RemoveAll(T => T.ProductID == obj.Modelo.ProductID);
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






















































    //****** Propiedades ******//



























    private void ShowInfo(string message)
    {
        displayInfo.Show();
        displayInfo.Text = message;
        indicador.Hide();
    }

}