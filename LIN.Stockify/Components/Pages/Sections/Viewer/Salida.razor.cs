using LIN.Inventory.Realtime.Manager.Models;
using LIN.Inventory.Shared;

namespace LIN.Components.Pages.Sections.Viewer;

public partial class Salida
{

    /// <summary>
    /// Id de la entrada.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Modelo de la salida.
    /// </summary>
    private OutflowDataModel? Model { get; set; }

    /// <summary>
    /// Establecer y obtener si se esta cargando aun información.
    /// </summary>
    private bool IsLoading { get; set; } = true;

    /// <summary>
    /// Establecer y obtener si se esta cargando aun información.
    /// </summary>
    private bool HasError { get; set; } = true;

    /// <summary>
    /// Establecer y obtener el mensaje de error.
    /// </summary>
    private string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Popup de alerta.
    /// </summary>
    private AlertPopup? Alert { get; set; }

    /// <summary>
    /// Información del cajero.
    /// </summary>
    private AccountModel? Cashier { get; set; }

    /// <summary>
    /// Obtener la imagen de perfil del cajero.
    /// </summary>
    private string CashierPicture => string.IsNullOrWhiteSpace(Cashier?.Profile)
                                     ? "./img/user.png"
                                     : Cashier.Profile;


    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {

        HasError = false;
        IsLoading = true;
        StateHasChanged();

        InventoryContext? inventoryContext = InventoryManager.FindContextByOutflow(int.Parse(Id));

        // Validar.
        if (inventoryContext is null)
        {
            // Obtener los detalles.
            var (outflowDetails, cajero) = await Access.Inventory.Controllers.Outflows.Read(int.Parse(Id), Session.Instance.Token, Session.Instance.AccountToken, true);

            if (cajero is not null && !AccountManager.Accounts.Exists(t => t.Id == cajero?.Id))
            {
                AccountManager.Accounts.Add(cajero);
                Cashier = cajero;
            }

            // Validar respuesta.
            if (outflowDetails.Response == Responses.Success)
                Model = outflowDetails.Model;
            else
            {
                ErrorMessage = "al cargar la información del movimiento.";
                HasError = true;
            }

            IsLoading = false;
            StateHasChanged();
            return;
        }

        // Obtener la salida.
        var outflow = (from outflowModel in (inventoryContext.Outflows ?? new()).Models
                       where outflowModel.Id == int.Parse(Id)
                       select outflowModel).FirstOrDefault();

        // Obtener cajero.
        Cashier = AccountManager.Accounts.FirstOrDefault(t => t.Id == outflow?.Profile?.AccountId);

        // Si no hay detalles.
        if (outflow?.Details.Count <= 0)
        {
            // Obtener los detalles.
            var (outflowDetails, cajero) = await Access.Inventory.Controllers.Outflows.Read(outflow.Id, Session.Instance.Token, Session.Instance.AccountToken, true);

            if (cajero is not null && !AccountManager.Accounts.Exists(t => t.Id == cajero?.Id))
            {
                AccountManager.Accounts.Add(cajero);
               
            } 
            Cashier = cajero;

            if (outflowDetails.Response == Responses.Success)
            {
                outflow.Details = outflowDetails.Model.Details;
                outflow.Inversion = outflowDetails.Model.Inversion;
                outflow.Ganancia = outflowDetails.Model.Ganancia;
                outflow.Utilidad = outflowDetails.Model.Utilidad;
                outflow.Outsider = outflowDetails.Model.Outsider;
            }
            else if (outflowDetails.Response == Responses.Unauthorized)
            {
                ErrorMessage = "no tienes autorización para ver este movimiento.";
                HasError = true;
            }
            else
            {
                ErrorMessage = "al cargar la información del movimiento.";
                HasError = true;
            }
        }

        // Establecer el modelo.
        IsLoading = false;
        Model = outflow;
        await base.OnParametersSetAsync();
    }


    /// <summary>
    /// Evento al iniciar.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        MainLayout.Configure(new()
        {
            OnCenterClick = Send,
            Section = 1,
            DockIcon = 2
        });
        await base.OnInitializedAsync();
    }


    /// <summary>
    /// Enviar el comando al selector.
    /// </summary>
    void Send()
    {
        // Nuevo onInvoque.
        MainLayout.DevicesSelector.OnInvoke = (device) =>
        {
            deviceManager.SendToDevice($"viewOutflow({Model?.Id})", device.Id);
        };

        MainLayout.DevicesSelector.Show();
    }


    /// <summary>
    /// Abrir ventana con información de la salida.
    /// </summary>
    /// <param name="id">Id de la salida.</param>
    public static void Show(int id)
    {
        MainLayout.Navigate($"/outflow/{id}");
    }


    async void Update()
    {
        var newdate = Model?.Date;

        await Access.Inventory.Controllers.Outflows.Update(Model.Id, newdate.Value, Session.Instance.Token);
        await InvokeAsync(StateHasChanged);
    }

    private string GetImage()
    {
        return (Model?.Type) switch
        {
            Types.Inventory.Enumerations.OutflowsTypes.Usage => "./img/Products/outflows/seller.png",
            Types.Inventory.Enumerations.OutflowsTypes.Contribution => "./img/Products/outflows/donate.png",
            Types.Inventory.Enumerations.OutflowsTypes.Fraud => "./img/Products/outflows/criminal.png",
            Types.Inventory.Enumerations.OutflowsTypes.Purchase => "./img/Products/outflows/shop.png",
            Types.Inventory.Enumerations.OutflowsTypes.Loss => "./img/Products/outflows/lost.png",
            Types.Inventory.Enumerations.OutflowsTypes.Expiry => "./img/Products/outflows/expired.png",
            _ => "./img/Products/packages.png",
        };
    }

}
