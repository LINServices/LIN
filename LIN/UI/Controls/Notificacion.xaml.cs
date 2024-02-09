using LIN.Access.Inventory.Controllers;

namespace LIN.UI.Controls;

public partial class Notificacion : Grid
{

    //======== Eventos =========//

    public event EventHandler<EventArgs>? Clicked;


    public Types.Inventory.Transient.Notificacion Modelo { get; set; }


    public Notificacion(LIN.Types.Inventory.Transient.Notificacion modelo)
    {
        InitializeComponent();
        this.Modelo = modelo;
        //ContactSubscriber.Suscribe(this);
        LoadModelVisible();
    }



    public void LoadModelVisible()
    {

        lbUser.Text = $"@{Modelo.UsuarioInvitador}";
        lbTime.Text = LIN.Services.Date.TiempoTranscurrido(Modelo.Fecha);
        lbInventario.Text = Modelo.Inventario;
        //lbMail.Text = $"Invitacion de @{Modelo.UsuarioInvitador}";
        //lbFecha.Text = ;
        //lbTelefono.Text = Modelo.Phone;
    }




    private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
        Clicked?.Invoke(this, new());
    }


    /// <summary>
    /// Declinar
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ButtonCancel(object sender, EventArgs e)
    {
        this.Hide();
        await Inventories.UpdateState(Modelo.ID, InventoryAccessState.Deleted);
    }



    /// <summary>
    /// Aceptar
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ButtonAcepted(object sender, EventArgs e)
    {
        this.Hide();
        await Inventories.UpdateState(Modelo.ID, InventoryAccessState.Accepted);
    }

    private void PointerGestureRecognizer_PointerEntered(object sender, PointerEventArgs e)
    {
        //hhn.Stroke = new Color(0, 109, 191);
    }

    private void PointerGestureRecognizer_PointerExited(object sender, PointerEventArgs e)
    {
        //hhn.Stroke = new Color(200, 200, 200);
    }
}