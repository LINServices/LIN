using LIN.Types.Contacts.Models;

namespace LIN.UI.Controls;

public partial class ContactMini : Grid
{

    //========= Eventos =========//

    /// <summary>
    /// Evento Click sobre el control
    /// </summary>
    public event EventHandler<EventArgs>? Clicked;



    //========= Propiedades =========//

    /// <summary>
    /// Modelo
    /// </summary>
    public ContactModel Modelo { get; set; }


    public string Text { get => lbName.Text; set => lbName.Text = value; }


    /// <summary>
    /// Constructor con modelo
    /// </summary>
    public ContactMini(ContactModel modelo)
    {
        InitializeComponent();
        this.Modelo = modelo;
        LoadModelVisible();
    }



    /// <summary>
    /// Constructor sin modelo
    /// </summary>
    public ContactMini()
    {
        InitializeComponent();
        this.Modelo = new();
        LoadWaitModel();
    }




    /// <summary>
    /// Muestra los datos del contacto en el display
    /// </summary>
    public void LoadWaitModel()
    {

        //// Si el modelo fue eliminado
        //if (Modelo.State == ContactStatus.Deleted)
        //{
        //    this.Hide();
        //    return;
        //}


        // Datos
        lbName.Text = "Selecciona un proveedor";
        lbMail.Hide();


        img.Show();
        lbPic.Hide();
        img.Source = ImageSource.FromFile("personas.png");
        img.Aspect = Aspect.AspectFill;
        bgImg.BackgroundColor = Microsoft.Maui.Graphics.Colors.Transparent;


    }



    /// <summary>
    /// Muestra los datos del contacto en el display
    /// </summary>
    public void LoadModelVisible()
    {
        lbMail.Show();
        // Si el modelo fue eliminado
        //if (Modelo.State == ContactStatus.Deleted)
        //{
        //    this.Hide();
        //    return;
        //}


        // Datos
        lbName.Text = Modelo.Nombre;
        lbMail.Text = Modelo.Mails[0].Email;

        // Si no hay imagen que mostrar
        //if (Modelo.Picture.Length ==0)
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


    /// <summary>
    /// State: Seleccionado
    /// </summary>
    public void Select()
    {
        bg.Stroke = Colors.RoyalBlue;
        bg.StrokeThickness = 1;
    }



    /// <summary>
    /// State: Normal
    /// </summary>
    public void UnSelect()
    {
        bg.Stroke = Colors.LightGray;
        bg.StrokeThickness = 0.5;
    }



    /// <summary>
    /// Submit del evento click
    /// </summary>
    private void EventoClick(object sender, EventArgs e)
    {
        Clicked?.Invoke(this, new());
    }


}