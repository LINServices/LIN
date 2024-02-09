using CommunityToolkit.Maui.Storage;
using LIN.UI.Popups;

namespace LIN.UI.Views.Inflows;

public partial class ViewItem : ContentPage
{

    /// <summary>
    /// Modelo
    /// </summary>
    public InflowDataModel Modelo { get; set; }

    AccountModel Creador { get; set; } = new();

    bool OpenExport { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public ViewItem(InflowDataModel model, bool openExport = false)
    {
        InitializeComponent();
        Modelo = model;
        OpenExport = openExport;
        LoadModel();

    }


    List<ProductModel> tranfers = new();


    /// <summary>
    /// Obtiene los datos
    /// </summary>
    /// <returns></returns>

    public async Task RequestData()
    {

        var response = await Access.Inventory.Controllers.Inflows.Read(Modelo.ID);

        //[Error] // Consulta Modelo.Profile
        var taskProf = await LIN.Access.Inventory.Controllers.Profile.ReadOne(Modelo.ProfileID);

        var taskUser = LIN.Access.Auth.Controllers.Account.Read(taskProf.Model.AccountID, Session.Instance.AccountToken);

        displayCategory.Text = Modelo.Type.ToString();

        if (response.Response != Responses.Success)
        {
            return;
        }

        foreach (var i in response.Model.Details)
        {
            var x = new LIN.UI.Controls.ProductDetail(i, tranfers);
            Detalles.Add(x);
        }


        if (response.Model.Type == InflowsTypes.Ajuste || response.Model.Type == InflowsTypes.Regalo || response.Model.Type == InflowsTypes.Devolucion)
        {
            lbInvercion.Text = $"El tipo '{response.Model.Type}' no tiene inversion";
        }
        else
        {
            lbInvercion.Text = response.Model.Inversion.ToString("0.##");
        }

        lbbFecha.Text = $"{Modelo.Date:HH:mm  dd/MM/yyyy}";


        Modelo = response.Model;


        var resUser = await taskUser;
        Creador = resUser.Model;

        lbName.Text = resUser.Model.Name;
        picUser.Source = ImageEncoder.Decode(resUser.Model.Profile);

        if (OpenExport)
            Export(null, null);

    }



    /// <summary>
    /// Renderiza el modelo
    /// </summary>
    public async void LoadModel()
    {
        await RequestData();
    }

    private void ToggleButton_Clicked(object sender, EventArgs e)
    {
        AppShell.OnViewON($"openIF({Modelo.ID})");
    }


    private async void Export(object sender, EventArgs e)
    {


#if WINDOWS

        CancellationToken token = new();
        var result = await FolderPicker.Default.PickAsync(token);
        if (!result.IsSuccessful)
            return;

        var folderBase = result.Folder.Path;
        await PDFService.RenderInflow(Modelo, Session.Instance.Account.Identity.Unique, Creador.Identity.Unique, tranfers, folderBase);

        await DisplayAlert("Reporte", "Reporte generado exitosamente", "OK");

#elif ANDROID

        var deviceSelector = new DeviceSelector($"exportInflow({Modelo.ID})", new(false, false)
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