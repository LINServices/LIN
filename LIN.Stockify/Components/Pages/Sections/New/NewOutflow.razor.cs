using LIN.Inventory.Realtime.Manager.Models;
using LIN.Inventory.Shared.Drawers;
using QRCoder;
using LIN.Types.Inventory.Enumerations;

namespace LIN.Components.Pages.Sections.New;

public partial class NewOutflow
{

    /// <summary>
    /// Id.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;

    public ClientsDrawer ClientDrawer { get; set; }



    /// <summary>
    /// Categoría.
    /// </summary>
    private int Category { get; set; }



    /// <summary>
    /// Sección actual.
    /// </summary>
    int section = 0;



    /// <summary>
    /// Fecha.
    /// </summary>
    private DateTime Date { get; set; } = DateTime.Now;



    /// <summary>
    /// Productos seleccionados.
    /// </summary>
    private List<ProductModel> Selected { get; set; } = [];



    /// <summary>
    /// Valores
    /// </summary>
    private Dictionary<int, int> Values = [];



    /// <summary>
    /// Drawer de productos.
    /// </summary>
    private DrawerProducts DrawerProducts { get; set; } = null!;



    /// <summary>
    /// Contexto del inventario.
    /// </summary>
    private InventoryContext? Contexto { get; set; }



    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {

        // Obtener el contexto.
        Contexto = InventoryManager.Get(int.Parse(Id));

        // Base.
        base.OnParametersSet();
    }



    /// <summary>
    /// Obtener valor.
    /// </summary>
    int GetValue(int product)
    {
        Values.TryGetValue(product, out var value);
        return value;
    }


    string ErrorMessage = "";

    string qr = "";
    string qrText = "";
    /// <summary>
    /// Crear.
    /// </summary>
    private async void Create(bool isOnline = false)
    {

        // Preparar la vista.
        section = 3;
        StateHasChanged();

        // Modelo.
        OutflowsTypes type = (OutflowsTypes)Category;

        // No tiene tipo.
        if (type == OutflowsTypes.None)
        {
            section = 0;
            StateHasChanged();
            return;
        }



        // Variables
        List<OutflowDetailsDataModel> details = new();
        OutflowDataModel entry;


        // Rellena los detalles
        foreach (var control in Selected ?? [])
        {

            Values.TryGetValue(control.Id, out int quantity);
            OutflowDetailsDataModel model = new()
            {
                Quantity = quantity,
                ProductDetail = new()
                {
                    Id = control.DetailModel?.Id ?? 0
                },
                ProductDetailId = control.DetailModel?.Id ?? 0
            };
            details.Add(model);
        }


        // Si no hay detalles
        if (details.Count <= 0)
        {

            return;
        }


        // Model de salida
        entry = new()
        {
            Details = details,
            Date = Date,
            Type = type,
            Inventory = new()
            {
                Id = Contexto?.Inventory.Id ?? 0
            },
            InventoryId = Contexto?.Inventory.Id ?? 0,
            ProfileId = Session.Instance.Information.Id
        };

        if (IsFormClient)
        {

            if (string.IsNullOrWhiteSpace(OutsiderDoc))
            {
                section = 2;
                ErrorMessage = "El cliente debe tener un documento valido.";
                StateHasChanged();
            }

            entry.Outsider = new()
            {
                Document = OutsiderDoc,
                Name = OutsiderName,
                Email = OutsiderMail
            };
        }
        else
        {
            entry.Outsider = null;
        }

        CreateResponse response;

        // Envía al servidor
        if (isOnline)
        {
            response = await Access.Inventory.Controllers.OpenStore.CreateOnline(entry, Access.Inventory.Session.Instance.Token);
            qr = GetQr(response.LastUnique);
            qrText = response.LastUnique;
        }
        else
        {
            response = await Access.Inventory.Controllers.Outflows.Create(entry, Access.Inventory.Session.Instance.Token);
        }


        int backSection = 0;

        switch (response.Response)
        {

            case Responses.Success:
                {
                    if (isOnline) backSection = 4;
                }
                break;

            case Responses.Unauthorized:
                section = 2;
                ErrorMessage = "No tienes autorización para crear movimientos en este inventario.";
                StateHasChanged();
                return;

            default:
                section = 2;
                ErrorMessage = "Hubo un error al crear este movimiento.";
                StateHasChanged();
                return;
        }




        section = 1;
        StateHasChanged();

        await Task.Delay(2000);
        section = backSection;
        StateHasChanged();

    }



    /// <summary>
    /// Cambio del valor.
    /// </summary>
    void ValueChange(int product, int q)
    {
        try
        {
            Values[product] = q;
        }
        catch
        {
            Values.Add(product, q);
        }

    }


    string OutsiderName = string.Empty;
    string OutsiderMail = string.Empty;
    string OutsiderDoc = string.Empty;


    bool IsFormClient = false;
    void SelectClient(bool value)
    {
        IsFormClient = value;
        StateHasChanged();
    }

    void GoNormal()
    {
        section = 0;
        StateHasChanged();
    }


    void CategorizeChange()
    {
        OutflowsTypes type = (OutflowsTypes)Category;





    }

    void ShowClient()
    {
        ClientDrawer.Show();
    }

    void SelectClient()
    {
        var client = ClientDrawer.Selected;

        if (client is null)
            return;

        OutsiderName = client.Name;
        OutsiderDoc = client.Document;
        StateHasChanged();
    }

    string GetQr(string text)
    {
        using QRCodeGenerator qrGenerator = new();
        using QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        using PngByteQRCode qrCode = new(qrCodeData);
        byte[] qrCodeBytes = qrCode.GetGraphic(20);

        string base64Qr = Convert.ToBase64String(qrCodeBytes);
        return $"data:image/png;base64,{base64Qr}";
    }
}