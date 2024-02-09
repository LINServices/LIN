using LIN.Types.Contacts.Models;

namespace LIN.UI.Popups;

public partial class ContactPopup : Popup
{


    public ContactModel Modelo { get; set; }



    public ContactPopup(ContactModel modelo)
    {
        InitializeComponent();
        this.Modelo = modelo;
        LoadModelVisible();
    }


    public void LoadModelVisible()
    {

        lbName.Text = Modelo.Nombre;


        displayEmail.SubTitulo = Modelo.Mails[0].Email;
        //displayDir.SubTitulo = Modelo.Direction;
        displayTel.SubTitulo = Modelo.Phones[0].Number;

        // Si no hay imagen que mostar
        //if (Modelo.Picture.Length == 0)
        //{
        //    img.Hide();
        //    lbPic.Show();
        //    lbPic.Text = lbName.Text[0].ToString().ToUpper();
        //    bgImg.BackgroundColor = Services.RandomColor.GetRandomColor();
        //}
        //else
        //{
        //    lbPic.Hide();
        //    img.Show();
        //    bgImg.BackgroundColor = Microsoft.Maui.Graphics.Colors.Transparent;
        //    img.Source = ImageEncoder.Decode(Modelo.Picture);
        //}

    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        AppShell.OnViewON($"openCt({Modelo.Id})", Applications.CloudConsole);
    }

    private async void Button_Clicked_1(object sender, EventArgs e)
    {

#if ANDROID
        if (PhoneDialer.IsSupported)
            PhoneDialer.Default.Open(Modelo.Phones[0].Number);

#elif WINDOWS

        var pop = new Popups.DeviceSelector($"""call("{Modelo.Phones[0]}")""",
            new() { App = new[] { Applications.Inventory }, Plataformas = new[] { Platforms.Android }, AutoSelect = true });

        await AppShell.ActualPage!.ShowPopupAsync(pop);
#endif


    }

    private void ToggleButton_Clicked(object sender, EventArgs e)
    {
        this.Close();
        _ = new ContactEdit(Modelo).Show();
    }

    private async void ToggleButton_Clicked_1(object sender, EventArgs e)
    {

        //var response = await ((Modelo.State == ContactStatus.Normal) ?
        //                    Access.Inventory.Controllers.Contact.ToTrash(Modelo.Id, Session.Instance.Token) :
        //                    Access.Inventory.Controllers.Contact.Delete(Modelo.Id, Session.Instance.Token));

        //this.Close();

        //if (response.Response == Responses.Success)
        //{
        //    Modelo.State = (Modelo.State == ContactStatus.Normal) ? ContactStatus.OnTrash : ContactStatus.Deleted;
        //    ContactObserver.Update(Modelo);
        //}


    }

    private async void EmailSend(object sender, EventArgs e)
    {
        try
        {
            if (Email.Default.IsComposeSupported)
            {

                string subject = "Hello!";
                string body = "";
                string[] recipients = new[] { Modelo.Mails[0].Email };

                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    BodyFormat = EmailBodyFormat.PlainText,
                    To = new List<string>(recipients)
                };

                await Email.Default.ComposeAsync(message);
            }
        }
        catch
        {
        }
    }
}