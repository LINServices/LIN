
using LIN.UI.Views;

namespace LIN.UI.Popups;


public partial class UserPassEdit : Popup
{


    /// <summary>
    /// Constructor
    /// </summary>
    public UserPassEdit()
    {
        InitializeComponent();
        this.CanBeDismissedByTappingOutsideOfPopup = false;
    }







    private void BtnCancelClick(object sender, EventArgs e)
    {
        this.Close(null);
    }


    private async void BtnSelectClick(object sender, EventArgs e)
    {


        // Validadar las contraseņas
        var oldPassword = txtOldPass.Text ?? "";
        var newPassword = txtNewPass.Text ?? "";
        var newPasswordRepit = txtNewPass2.Text ?? "";


        if (oldPassword.Length < 4 || newPassword.Length < 4 || newPasswordRepit.Length < 4)
        {
            displayInfo.Text = "Completa los campos requeridos";
            displayInfo.Show();
            return;
        }

        if (newPassword != newPasswordRepit)
        {
            displayInfo.Text = "Las contraseņas no coinciden";
            displayInfo.Show();
            return;
        }


        var modelo = new UpdatePasswordModel
        {
            Account = Session.Instance.Account.ID,
            NewPassword = newPassword,
            OldPassword = oldPassword
        };


        var response = await LIN.Access.Auth.Controllers.Account.UpdatePassword(modelo);

        if (response.Response != Responses.Success)
        {
            displayInfo.Text = "No se pudo cambiar la contraseņa";
            displayInfo.Show();
            return;
        }


        this.Close();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        this.Close();
    }

}