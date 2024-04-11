namespace LIN.Pages.Sections.Viewer;


public partial class Entrada
{


    /// <summary>
    /// Id de la entrada.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;


    bool edit = false;


    /// <summary>
    /// Modelo
    /// </summary>
    private InflowDataModel? Modelo { get; set; } = new();



    protected override async Task OnParametersSetAsync()
    {


        // Obtener el Contexto.
        var inventoryContext = (from context in Services.InventoryContext.Dictionary
                                where (context.Value.Inflows ?? new()).Models.Any(t => t.ID == int.Parse(Id))
                                select context.Value).FirstOrDefault();

        // Validar.
        if (inventoryContext == null)
        {
            // Obtener los detalles.
            var inflowDetails = await LIN.Access.Inventory.Controllers.Inflows.Read(int.Parse(Id), LIN.Access.Inventory.Session.Instance.Token, true);

            // Validar respuesta.
            if (inflowDetails.Response == Responses.Success)
            {
                Modelo = inflowDetails.Model;
            }


            return;
        }


        // Obtener la salida.
        var inflow = (from inflowModel in (inventoryContext.Inflows ?? new()).Models
                      where inflowModel.ID == int.Parse(Id)
                      select inflowModel).FirstOrDefault();

        // Si no hay detalles.
        if (inflow?.Details.Count <= 0)
        {
            // Obtener los detalles.
            var inflowDetails = await Access.Inventory.Controllers.Inflows.Read(inflow.ID, Session.Instance.Token, true);

            if (inflowDetails.Response == Responses.Success)
            {
                inflow.Details = inflowDetails.Model.Details;
                inflow.Inversion = inflowDetails.Model.Inversion;
            }

        }

        // Establecer el modelo.
        Modelo = inflow;



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
            Services.Realtime.InventoryAccessHub.SendToDevice(e.Id, new()
            {
                Command = $"viewInflow({Modelo?.ID})"
            });
        };

        Components.Layout.MainLayout.DevicesSelector.Show();
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
        var newdate = Modelo?.Date;

        await LIN.Access.Inventory.Controllers.Inflows.Update(Modelo.ID, newdate.Value, Session.Instance.Token);
        edit = false;
        await this.InvokeAsync(StateHasChanged);
    }
}