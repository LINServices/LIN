namespace LIN.Components.Pages;

public partial class Register
{

    /// <summary>
    /// Usuario.
    /// </summary>
    private string User { get; set; } = string.Empty;


    /// <summary>
    /// Nombre.
    /// </summary>
    private string Name { get; set; } = string.Empty;


    /// <summary>
    /// Contraseña.
    /// </summary>
    private string Password { get; set; } = string.Empty;


    /// <summary>
    /// Error es visible.
    /// </summary>
    private bool ErrorVisible { get; set; }


    /// <summary>
    /// Mensaje de error.
    /// </summary>
    private string ErrorMessage { get; set; } = string.Empty;


    /// <summary>
    /// Sección actual.
    /// </summary>
    private int Section { get; set; } = 0;



    /// <summary>
    /// Oculta los errores
    /// </summary>
    private void HideError()
    {
        ErrorVisible = false;
        StateHasChanged();
    }


    /// <summary>
    /// Muestra un mensaje
    /// </summary>
    private async Task ShowError(string message)
    {
        ErrorVisible = true;
        ErrorMessage = message;
        Section = 2;
        StateHasChanged();
        await Task.Delay(3000);
        Section = 0;
        StateHasChanged();
    }


    /// <summary>
    /// Crear cuenta.
    /// </summary>
    private async void Start()
    {

        // Actualizar sección.
        Section = 3;
        StateHasChanged();

        // Sin información.
        if (User.Length <= 0 || Password.Length <= 0 || Name.Length <= 0)
        {
            await ShowError("Completa todos los campos");
            return;
        }

        // Sin información.
        if (Password.Length < 4)
        {
            await ShowError("La contraseña debe tener mas de 4 dígitos");
            return;
        }

        // Model
        AccountModel modelo = new()
        {
            Name = Name,
            Identity = new()
            {
                Unique = User
            },
            Password = Password
        };

        // Creación
        var result = await LIN.Access.Auth.Controllers.Account.Create(modelo);

        // Respuesta
        switch (result.Response)
        {
            case Responses.Success:
                // Obtener local db.
                LocalDataBase.Data.UserDB database = new();

                // Guardar información.
                await database.SaveUser(new() { ID = result.LastID, UserU = User, Password = Password });

                Section = 1;
                StateHasChanged();
                break;

            case Responses.NotConnection:
                await ShowError("Error de conexión");
                return;

            case Responses.ExistAccount:
                await ShowError($"Ya existe un usuario con el nombre '{User}'");
                return;

            default:
                await ShowError($"Hubo un error grave");
                return;
        }


        var waitTask = Task.Delay(3000);

        var (_, Response) = await Access.Inventory.Session.LoginWith(result.Token);

        await waitTask;

        if (Response == Responses.Success)
        {
            NavigationManager.NavigateTo("/home");
        }
        else
        {
            await ShowError($"Su cuenta fue creada, pero hubo un error al iniciar sesión");
            NavigationManager.NavigateTo("/");
        }
    }

}