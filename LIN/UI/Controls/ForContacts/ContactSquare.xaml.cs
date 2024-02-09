using LIN.Types.Contacts.Models;

namespace LIN.UI.Controls;

public partial class ContactSquare : Grid
{

    //========= Eventos =========//

    /// <summary>
    /// Evento Click sobre el control
    /// </summary>
    public event EventHandler<EventArgs>? Clicked;



    //========= Propiedades =========//

    /// <summary>
    /// Model del contacto
    /// </summary>
    public ContactModel Modelo { get; set; }


    public string Text { get => lbName.Text; set => lbName.Text = value; }


    /// <summary>
    /// Constructor con modelo
    /// </summary>
    public ContactSquare(ContactModel modelo)
    {
        InitializeComponent();
        this.Modelo = modelo;
        LoadModelVisible();
    }



    /// <summary>
    /// Constructor sin modelo
    /// </summary>
    public ContactSquare()
    {
        this.Modelo = new();
        InitializeComponent();
        LoadWaitModel();
    }




    /// <summary>
    /// Muestra los datos del contacto en el display
    /// </summary>
    public void LoadWaitModel()
    {

        // Si el modelo fue eliminado
        //if (Modelo.State == ContactStatus.Deleted)
        //{
        //    this.Hide();
        //    return;
        //}


        // Datos
        lbName.Text = "Selecciona un proveedor";
        //lbMail.Hide();


        img.Show();
        lbPic.Hide();
        img.Source = ImageSource.FromResource("personas.png");
        img.Aspect = Aspect.AspectFill;
        bgImg.BackgroundColor = Microsoft.Maui.Graphics.Colors.Transparent;


    }



    /// <summary>
    /// Muestra los datos del contacto en el display
    /// </summary>
    public void LoadModelVisible()
    {

        if (Modelo == null)
            return;

        // Datos
        lbName.Text = TextShortener.Short(Modelo.Nombre ?? "", 12);

        // Si no hay imagen que mostrar
        //if (Modelo.Picture.Length == 0)
        //{
        //    img.Hide();
        //    lbPic.Show();

        //    if (lbName.Text.Length > 0)
        //        lbPic.Text = lbName.Text[0].ToString().ToUpper();
        //    else
        //        lbPic.Text = "U";

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