namespace LIN.UI.Views.Contacts;

public partial class Add : ContentPage
{

 
    /// <summary>
    /// Evento de Agregar
    /// </summary>
    public event EventHandler<Services.AddResponse>? OnAdd;



    /// <summary>
    /// Constructor
    /// </summary>
    public Add()
    {
        InitializeComponent();
    }




    /// <summary>
    /// Comprueba si los datos estan completos
    /// </summary>
    private bool IsDataComplete()
    {

        // Nombre
        if (string.IsNullOrWhiteSpace(txtName.Text))
            return false;

        return true;

    }



    /// <summary>
    /// Evento click sobre el boton de crear
    /// </summary>
    private async void Button_Clicked(object sender, EventArgs e)
    {
        // Organiza la vista
        lbInfo.Hide();
        btn.Hide();
        indicador.Show();


        // Si los datos estan incompletos
        var isComplete = IsDataComplete();

        // Retorna
        if (!isComplete)
        {
            ShowInfo("Complete el nombre del contacto");
            btn.Show();
            return;
        }


        // Obtiene la imagen
        var imagen = Task.Run(() => { return Array.Empty<byte>(); });
        if (inputImage.IsChanged)
            imagen = inputImage.GetBytes();


        // Model
        var modelo = new ContactDataModel()
        {
            Mail = txtMail.Text ?? "Sin definir",
            Name = txtName.Text ?? "Unnamed",
            Picture =    await imagen,
            ProfileID = Session.Instance.Informacion.ID,
            Direction = txtDireccion.Text ?? "Sin definir",
            Phone = txtPhone.Text ?? "Sin definir"
        };


        // Respuesta del controlador
        var task = Access.Inventory.Controllers.Contact.Create(modelo);


        // Muestras se completa la tarea
        int seconds = 0;
        while (task.Status != TaskStatus.RanToCompletion)
        {
            seconds++;
            await Task.Delay(1000);
            if (seconds == 5)
            {
                lbInfo.Show("Espera un momento", Colors.DarkGray);
                break;
            }
        }


        var response = await task;





        // Organización de la interfaz
        indicador.Hide();
        lbInfo.Hide();
        lbInfo.TextColor = Colors.Crimson;
        btn.Show();

        if (response.Response != Responses.Success)
        {
            ShowInfo("Hubo un error");
            OnAdd?.Invoke(this, Services.AddResponse.Failure);
            return;
        }



        AppShell.Hub.SendContactModel(Session.Instance.Informacion.ID, response.LastID);

        // Muestra el popup de agregado
        OnAdd?.Invoke(this, Services.AddResponse.Success);
        await this.ShowPopupAsync(new Popups.DefaultPopup());


    }



    /// <summary>
    /// Muestra un mensaje
    /// </summary>
    private void ShowInfo(string message)
    {
        lbInfo.Text = message ?? "";
        lbInfo.Show();
        indicador.Hide();
    }



}