using CommunityToolkit.Maui.Storage;
using LIN.UI.Popups;

namespace LIN.UI.Views.Outflows;


public partial class ViewItem : ContentPage
{


    public OutflowDataModel Modelo { get; set; }

    SessionModel<ProfileModel> Creador = new();
    bool OpenExport { get; set; }
    public ViewItem(OutflowDataModel model, bool openExport = false)
    {
        InitializeComponent();
        Modelo = model;
        OpenExport = openExport;
        LoadModel();
    }


    List<ProductDataTransfer> Transfers = new();


    public async Task RequestData()
    {

        var response = await Access.Inventory.Controllers.Outflows.Read(Modelo.ID);
        var taskUser = LIN.Access.Auth.Controllers.Account.Read(Modelo.ProfileID);

        displayCategory.Text = Modelo.Type.ToString();
        indicador.Hide();
        if (response.Response != Responses.Success)
        {
            return;
        }

        foreach (var i in response.Model.Details)
        {
            var x = new ProductDetail(i, Transfers, LoadInversion);
            Detalles.Add(x);
        }

       

        lbbFecha.Text = $"{Modelo.Date:HH:mm  dd/MM/yyyy}";


        Modelo = response.Model;


        var resUser = await taskUser;
        //Creador = resUser.Model.ID;
        lbName.Text = resUser.Model.Nombre;
        picUser.Source = ImageEncoder.Decode(resUser.Model.Perfil);
        displayCategory.Text = Modelo.Type.Humanize();

        if (OpenExport)
            Export(null, null);


 LoadInversion();

    }


    
    private void LoadInversion()
    {

      
        switch (Modelo.Type)
        {
            case OutflowsTypes.Venta:
                lbInversionLabel.Text = "Ganancias";
                decimal ganancia = 0;
                foreach (var e in Transfers)
                    ganancia += (e.PrecioVenta - e.PrecioCompra) * Modelo.Details.Where(T=>T.ProductoDetail == e.IDDetail).FirstOrDefault()?.Cantidad ?? 0;
                
                lbInvercion.Text = $"{ganancia}$";
                break;

            case OutflowsTypes.Donacion:
                lbInversionLabel.Text = "Donaci�n";
                decimal donacion = 0;
                foreach (var e in Transfers)
                    donacion += (e.PrecioVenta - e.PrecioCompra) * Modelo.Details.Where(T => T.ProductoDetail == e.IDDetail).FirstOrDefault()?.Cantidad ?? 0;

                lbInvercion.Text = $"{donacion}$";
                break;

            default:
                lbInversionLabel.Text = "Perdida";
                decimal perdida = 0;
                foreach (var e in Transfers)
                    perdida += (e.PrecioCompra * Modelo.Details.Where(T => T.ProductoDetail == e.IDDetail).FirstOrDefault()?.Cantidad ?? 0) ;

                lbInvercion.Text = $"{perdida}$";
                break;
        }


    }



    public async void LoadModel()
    {
        await RequestData();
    }

    private void ToggleButton_Clicked(object sender, EventArgs e)
    {
        AppShell.OnViewON($"openOF({Modelo.ID})");
    }


    private async void Export(object sender, EventArgs e)
    {


#if WINDOWS

        CancellationToken token = new();
        var result = await FolderPicker.Default.PickAsync(token);
        if (!result.IsSuccessful)
            return;

        var folderBase = result.Folder.Path;

        await PDFService.RenderOutflow(Modelo, Session.Instance.Account.Usuario, Creador.Account.Usuario, Transfers, folderBase);
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