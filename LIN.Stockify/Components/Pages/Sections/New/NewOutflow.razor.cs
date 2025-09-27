using LIN.Inventory.Realtime.Manager.Models;
using LIN.Types.Inventory.Enumerations;
using QRCoder;

namespace LIN.Components.Pages.Sections.New;

public partial class NewOutflow
{
    /// <summary>
    /// Id del inventario.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Incluir la información del cliente.
    /// </summary>
    private bool _includeClientInformation = false;

    /// <summary>
    /// Alerta roja (Mensajes de error)
    /// </summary>
    private RedAlert Alert { get; set; } = default!;

    /// <summary>
    /// Cajon de buscar cliente.
    /// </summary>
    private ClientsDrawer ClientDrawer { get; set; } = default!;

    /// <summary>
    /// Drawer de productos.
    /// </summary>
    private DrawerProducts DrawerProducts { get; set; } = null!;

    /// <summary>
    /// Categoría.
    /// </summary>
    private int Category { get; set; }

    /// <summary>
    /// Nombre del cliente.
    /// </summary>
    private string OutsiderName = string.Empty;

    /// <summary>
    /// Correo electronico del cliente.
    /// </summary>
    private string OutsiderMail = string.Empty;

    /// <summary>
    /// Documento del cliente.
    /// </summary>
    private string OutsiderDoc = string.Empty;

    /// <summary>
    /// Sección actual.
    /// </summary>
    private int section = 0;

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
    private readonly Dictionary<int, int> Values = [];

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
    private int GetValue(int product)
    {
        Values.TryGetValue(product, out var value);
        return value;
    }


    private string ErrorMessage = "";

    private string qr = "";
    private string qrText = "";

    /// <summary>
    /// Crear.
    /// </summary>
    private async void Create(bool isOnline = false)
    {
        // Establecer la pantalla de carga.
        section = 3;
        StateHasChanged();

        // Modelo.
        OutflowsTypes type = (OutflowsTypes)Category;

        // No tiene tipo.
        if (type == OutflowsTypes.None)
        {
            ShowMessage("Debe seleccionar un tipo de movimiento.");
            return;
        }

        // Variables
        List<OutflowDetailsDataModel> details = [];
        OutflowDataModel entry;

        // Rellena los detalles.
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

        // Si no hay detalles.
        if (details.Count <= 0)
        {
            ShowMessage("Para realizar un movimiento, debe haber minimo un producto.");
            return;
        }

        // Modelo de salida.
        entry = new()
        {
            Details = details,
            Date = Date,
            Type = type,
            Inventory = new()
            {
                Id = Contexto?.Inventory?.Id ?? 0
            },
            InventoryId = Contexto?.Inventory?.Id ?? 0,
            ProfileId = Session.Instance.Information.Id
        };

        // Si se debe incluir información del cliente.
        if (_includeClientInformation)
        {
            if (string.IsNullOrWhiteSpace(OutsiderDoc))
            {
                ShowMessage("El cliente debe tener un documento valido.");
                return;
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

        // Crear respuesta.
        CreateResponse response;

        // Si es una venta online.
        if (isOnline)
        {
            response = await Access.Inventory.Controllers.OpenStore.CreateOnline(entry, Access.Inventory.Session.Instance.Token);
            qr = NewOutflow.GetQr(response.LastUnique);
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
                    if (isOnline)
                        backSection = 4;
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
    private void ValueChange(int product, int q)
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

    /// <summary>
    /// Ir a la sección principal.
    /// </summary>
    private void GoNormal()
    {
        section = 0;
        StateHasChanged();
    }

    /// <summary>
    /// Mostrar el cajon de cliente.
    /// </summary>
    private void ShowClient()
    {
        ClientDrawer.Show();
    }

    /// <summary>
    /// Seleccionar un cliente.
    /// </summary>
    private void SelectClient()
    {
        var client = ClientDrawer.Selected;

        if (client is null)
            return;

        OutsiderName = client.Name;
        OutsiderDoc = client.Document;
        StateHasChanged();
    }

    /// <summary>
    /// Obtener la imagen QR.
    /// </summary>
    private static string GetQr(string text)
    {
        using QRCodeGenerator qrGenerator = new();
        using QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        using PngByteQRCode qrCode = new(qrCodeData);
        byte[] qrCodeBytes = qrCode.GetGraphic(20);

        string base64Qr = Convert.ToBase64String(qrCodeBytes);
        return $"data:image/png;base64,{base64Qr}";
    }

    /// <summary>
    /// Mostrar un mensaje de alerta.
    /// </summary>
    private void ShowMessage(string text)
    {
        Alert.Show(text);
        section = 0;
        StateHasChanged();
    }
}