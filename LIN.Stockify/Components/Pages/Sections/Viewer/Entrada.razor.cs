

using LIN.Inventory.Realtime.Manager.Models;
using LIN.Inventory.Shared;

namespace LIN.Components.Pages.Sections.Viewer;


public partial class Entrada
{
    /// <summary>
    /// Establecer y obtener si se esta cargando aun información.
    /// </summary>
    private bool IsLoading { get; set; } = true;

    /// <summary>
    /// Establecer y obtener si se esta cargando aun información.
    /// </summary>
    private bool HasError { get; set; } = true;

    /// <summary>
    /// Obtener la imagen de perfil del cajero.
    /// </summary>
    private string CashierPicture => string.IsNullOrWhiteSpace(Cashier?.Profile)
                                     ? "./img/user.png"
                                     : Cashier.Profile;

    /// <summary>
    /// Establecer y obtener el mensaje de error.
    /// </summary>
    private string ErrorMessage { get; set; } = string.Empty;




    AlertPopup Alerta;

    /// <summary>
    /// Id de la entrada.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;


    bool edit = false;


    /// <summary>
    /// Modelo
    /// </summary>
    private InflowDataModel? Model { get; set; } = new();


    private AccountModel? Cashier { get; set; }


    protected override async Task OnParametersSetAsync()
    {

        HasError = false;
        IsLoading = true;
        StateHasChanged();

        InventoryContext? inventoryContext = InventoryManager.FindContextByInflow(int.Parse(Id));

        // Validar.
        if (inventoryContext == null)
        {
            // Obtener los detalles.
            var (inflowDetails, cajero) = await Access.Inventory.Controllers.Inflows.Read(int.Parse(Id), Access.Inventory.Session.Instance.Token, Session.Instance.AccountToken, true);

            if (cajero is not null && !AccountManager.Accounts.Exists(t => t.Id == cajero?.Id))
            {
                AccountManager.Accounts.Add(cajero);
                Cashier = cajero;
            }

            // Validar respuesta.
            if (inflowDetails.Response == Responses.Success)
            {
                Model = inflowDetails.Model;
            }

            IsLoading = false;
            StateHasChanged();
            return;
        }
        else
        {
            Cashier = AccountManager.Accounts.FirstOrDefault(t => t.Id == Model?.Profile?.AccountId);
        }


        // Obtener la salida.
        var inflow = (from inflowModel in (inventoryContext.Inflows ?? new()).Models
                      where inflowModel.Id == int.Parse(Id)
                      select inflowModel).FirstOrDefault();

        // Si no hay detalles.
        if (inflow?.Details.Count <= 0)
        {
            // Obtener los detalles.
            var (inflowDetails, cajero) = await Access.Inventory.Controllers.Inflows.Read(inflow.Id, Session.Instance.Token, Session.Instance.AccountToken, true);

            if (cajero is not null && !AccountManager.Accounts.Exists(t => t.Id == cajero?.Id))
            {
                AccountManager.Accounts.Add(cajero);
                Cashier = cajero;
            }

            if (inflowDetails.Response == Responses.Success)
            {
                inflow.Details = inflowDetails.Model.Details;
                inflow.Inversion = inflowDetails.Model.Inversion;
                inflow.Prevision = inflowDetails.Model.Prevision;
            }
            else if (inflowDetails.Response == Responses.Unauthorized)
            {
                Alerta.Show("No tienes autorización para visualizar los movimientos.");
            }

        }
        else
        {
            Cashier = AccountManager.Accounts.FirstOrDefault(t => t.Id == Model?.Profile?.AccountId);
        }

        // Establecer el modelo.
        IsLoading = false;
        Model = inflow;

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
        MainLayout.DevicesSelector.OnInvoke = (e) =>
        {
            deviceManager.SendToDevice($"viewInflow({Model?.Id})", e.Id);
        };

        MainLayout.DevicesSelector.Show();
    }






    public static void Show(int id)
    {
        MainLayout.Navigate($"/inflow/{id}");
    }

    void ControllerDate()
    {
        edit = !edit;
        StateHasChanged();
    }


    async void Update()
    {
        var newdate = Model?.Date;

        await Access.Inventory.Controllers.Inflows.Update(Model.Id, newdate.Value, Session.Instance.Token);
        edit = false;
        await InvokeAsync(StateHasChanged);
    }




    private string GetImage()
    {



        switch (Model?.Type)
        {
            case Types.Inventory.Enumerations.InflowsTypes.Purchase:
                return "./img/Products/inflows/cart.png";
            case Types.Inventory.Enumerations.InflowsTypes.Refund:
                return "./img/Products/inflows/return.png";
            case Types.Inventory.Enumerations.InflowsTypes.Gift:
                return "./img/Products/inflows/gift.png";
            case Types.Inventory.Enumerations.InflowsTypes.Correction:
                return "./img/Products/inflows/setting.png";
            default:
                return "./img/Products/packages.png";
        }

    }

}