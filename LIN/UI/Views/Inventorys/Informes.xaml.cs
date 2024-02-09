using CommunityToolkit.Maui.Storage;
using LIN.Access.Inventory.Controllers;
using LIN.UI.Popups;

namespace LIN.UI.Views.Inventorys;


public partial class Informes : ContentPage
{


    //===== Propiedades =====//


    /// <summary>
    /// Nuevos participantes
    /// </summary>
    private List<SessionModel<ProfileModel>> Participantes { get; set; } = new();


    /// <summary>
    /// Id del inventario
    /// </summary>
    private int Inventario { get; set; } = new();




    /// <summary>
    /// Constructor
    /// </summary>
    public Informes(int modelo)
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
    }




    /// <summary>
    /// Evento click sobre el boton de crear
    /// </summary>
    private async void ButtonCrearClick(object sender, EventArgs e)
    {

        // Organiza la vista
        lbInfo.Hide();

        indicador.Show();
        await Task.Delay(10);

        // Retorna
        if (Participantes.Count <= 0)
        {
            ShowMessage("Debe haber almenos 1 nuevo integrante");

            return;
        }





        // Creacion del modelo
        var modelo = new InventoryDataModel
        {
            ID = Inventario,
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

            return;
        }




        // ---- EVENTO DEL HUB
        //AppShell.Hub.SendNotificacion(ids);

        // Muestra el popup de agregado
        await this.ShowPopupAsync(new DefaultPopup());

        indicador.Hide();

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

        foreach (var modelo in xx)
        {
            var usercontrol = new Controls.UserForPick(modelo);
            usercontrol.Show();
            Participantes.Add(modelo);
        }



    }

    private async void BtnInformeMensual(object sender, EventArgs e)
    {


#if WINDOWS

        Content.Hide();
        lbInfo.Hide();
        indicador.Show();

        var tipo = inpTipo.SelectedIndex;
        var mes = inpMonth.SelectedIndex;
        var year = inpYear.SelectedIndex;


        if (mes == -1 || year == -1 || tipo == -1)
        {
            indicador.Hide();
            lbInfo.Show();
            lbInfo.Text = "Completa todos los campos";
            Content.Show();
            return;
        }


        mes++;
        year = 2020 + year;

        var user = Session.Instance.Informacion.ID;


        ReadOneResponse<List<byte>> res;

        if (tipo == 0)
            res = await Access.Inventory.Controllers.Inflows.InformeMonth(user, Inventario, mes, year);
        
        else
            res = await Access.Inventory.Controllers.Outflows.InformeMonth(user, Inventario, mes, year);
        

        if (res.Response != Responses.Success)
        {

            await DisplayAlert("Error", "Actualmente el generador de PDF no esta disponible, intentalo mas tarde, si el problema persiste comunicate con soporte.", "Ok");

            Content.Show();
            indicador.Hide();
            return;
        }


        CancellationToken token = new();
        var result = await FolderPicker.Default.PickAsync(token);
        if (!result.IsSuccessful)
        {
            Content.Show();
            indicador.Hide();
            return;
        }

        var folderBase = result.Folder.Path;

        File.WriteAllBytes($"{folderBase}\\month_{DateTime.Now:yyyy_MM_dd_HH_mm}.pdf", res.Model.ToArray());


        await DisplayAlert("Reporte", "Reporte generado exitosamente", "OK");

        Content.Show();
        indicador.Hide();


#elif ANDROID

        _ = DisplayAlert("Error", "Para generar un informe necesitas conectarte desde un dispositivo Windows", "Ok");

#endif



    }


}