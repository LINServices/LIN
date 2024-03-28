using LIN.Access.Inventory.Hubs;
using LIN.Pages;
using LIN.Services.Runtime;
using SILF.Script.Interfaces;

namespace LIN.Services;


internal class Realtime
{


    /// <summary>
    /// Id del dispositivo.
    /// </summary>
    public static string DeviceName { get; set; } = string.Empty;



    /// <summary>
    /// Id del dispositivo.
    /// </summary>
    public static string DeviceKey { get; private set; } = string.Empty;



    /// <summary>
    /// Funciones
    /// </summary>
    public static List<IFunction> Actions { get; set; } = [];



    /// <summary>
    /// Hub de tiempo real.
    /// </summary>
    public static InventoryAccessHub? InventoryAccessHub { get; set; } = null;



    /// <summary>
    /// Iniciar el servicio.
    /// </summary>
    public static void Start()
    {

        // Validar si ya existe el hub.
        if (InventoryAccessHub != null)
            return;

        // Llave.
        if (string.IsNullOrWhiteSpace( DeviceKey))
            DeviceKey = Guid.NewGuid().ToString();

        // Generar nuevo hub.
        InventoryAccessHub = new(Session.Instance.Token, new()
        {
            Name = DeviceName,
            Platform = MauiProgram.GetPlatform(),
            LocalId = DeviceKey
        });

        // Evento.
        InventoryAccessHub.On += OnReceiveCommand;

    }



    /// <summary>
    /// Construye las funciones.
    /// </summary>
    public static void Build()
    {


        // Función de actualizar contactos.
        SILFFunction updateContacts = new((values) =>
        {
            Contactos.ToUpdate();
        })
        // Propiedades
        {
            Name = "updateCt",
            Parameters = []
        };


        // Visualizar un contacto.
        SILFFunction viewContact = new(async (values) =>
        {

            // Obtener el parámetro.
            var value = values.FirstOrDefault(t => t.Name == "id")?.Objeto.GetValue();

            // Validar el tipo.
            if (value is not decimal)
                return;

            // Id.
            var id = (int)((value as decimal?) ?? 0);

            // Obtener el contacto.
            ContactModel? contact = Contactos.Response?.Models.FirstOrDefault(t => t.Id == id);

            // Validar.
            if (contact == null)
            {
                // Obtener desde la web.
                var response = await Access.Inventory.Controllers.Contact.Read(id, Session.Instance.ContactsToken);

                // Error.
                if (response.Response != Responses.Success)
                    return;

                // Establecer.
                contact = response.Model;
            }

            // Abrir el pop.
            MainLayout.ContactPop.Show(contact);

        })
        // Propiedades
        {
            Name = "viewContact",
            Parameters =
            [
                new("id", new("number"))
            ]
        };


        // Agregar producto.
        SILFFunction addProduct = new(async (values) =>
        {

            // Obtener el parámetro.
            var value = values.FirstOrDefault(t => t.Name == "id")?.Objeto.GetValue();

            if (value is not decimal)
                return;

            var id = (int)((value as decimal?) ?? 0);



            // Producto.
            var product = await LIN.Access.Inventory.Controllers.Product.Read(id, Session.Instance.Token);

            if (product.Response != Responses.Success)
                return;

            // Contexto.
            var context = InventoryContext.Get(product.Model.InventoryId);

            // Si no se encontró.
            if (context == null)
                return;

            if (context.Products != null && context.Products.Response == Responses.Success)
                context.Products.Models.Add(product.Model);

            ProductObserver.Update(context.Inventory.ID);


        })
        // Propiedades
        {
            Name = "addProduct",
            Parameters =
            [
                new("id", new("number"))
            ]
        };


        // Agregar entrada.
        SILFFunction addInflow = new(async (values) =>
        {

            // Obtener el parámetro.
            var value = values.FirstOrDefault(t => t.Name == "id")?.Objeto.GetValue();

            if (value is not decimal)
                return;

            var id = (int)((value as decimal?) ?? 0);


            // Producto.
            var inflow = await LIN.Access.Inventory.Controllers.Inflows.Read(id, Session.Instance.Token, false);

            if (inflow.Response != Responses.Success)
                return;

            // Contexto.
            var context = InventoryContext.Get(inflow.Model.InventoryId);

            // Si no se encontró.
            if (context == null)
                return;

            if (context.Inflows != null && context.Inflows.Response == Responses.Success)
                context.Inflows.Models.Insert(0, inflow.Model);


            // Actualizar la cantidad.
            foreach (var item in inflow.Model.Details)
            {
                // Detalle.
                var product = context.Products?.Models.Where(t => t.DetailModel?.Id == item.ProductDetailId).FirstOrDefault();

                // Actualizar la cantidad.
                if (product != null && product.DetailModel != null)
                    product.DetailModel.Quantity += item.Cantidad;

            }



            ProductObserver.Update(context.Inventory.ID);
            InflowObserver.Update(context.Inventory.ID);


        })
        // Propiedades
        {
            Name = "addInflow",
            Parameters =
            [
                new("id", new("number")),
                new("increase", new("bool"))
            ]
        };


        // Agregar salida.
        SILFFunction addOutflow = new(async (values) =>
        {

            // Obtener el parámetro.
            var value = values.FirstOrDefault(t => t.Name == "id")?.Objeto.GetValue();

            if (value is not decimal)
                return;

            var id = (int)((value as decimal?) ?? 0);

            // Producto.
            var outflow = await LIN.Access.Inventory.Controllers.Outflows.Read(id, Session.Instance.Token, false);

            if (outflow.Response != Responses.Success)
                return;

            // Contexto.
            var context = InventoryContext.Get(outflow.Model.InventoryId);

            // Si no se encontró.
            if (context == null)
                return;

            if (context.Outflows != null && context.Outflows.Response == Responses.Success)
                context.Outflows.Models.Insert(0, outflow.Model);

            // Actualizar la cantidad.
            foreach (var item in outflow.Model.Details)
            {
                // Detalle.
                var product = context.Products?.Models.Where(t => t.DetailModel?.Id == item.ProductDetailId).FirstOrDefault();

                // Actualizar la cantidad.
                if (product != null && product.DetailModel != null)
                    product.DetailModel.Quantity -= item.Cantidad;

            }

            ProductObserver.Update(context.Inventory.ID);
            OutflowObserver.Update(context.Inventory.ID);


        })
        // Propiedades
        {
            Name = "addOutflow",
            Parameters =
            [
                new("id", new("number")),
                new("decrement", new("bool"))
            ]
        };


        // Agregar salida.
        SILFFunction newInvitation = new(async (values) =>
        {

            // Obtener el parámetro.
            var value = values.FirstOrDefault(t => t.Name == "id")?.Objeto.GetValue();

            if (value is not decimal)
                return;

            var id = (int)((value as decimal?) ?? 0);


            // Modelo.
            var notification = await Access.Inventory.Controllers.InventoryAccess.ReadNotification(id, Session.Instance.Token);


            if (notification == null || notification.Response != Responses.Success)
                return;


            NotificationObserver.Append(notification.Model);

        })
        // Propiedades
        {
            Name = "newInvitation",
            Parameters =
            [
                new("id", new("number"))
            ]
        };


        // Agregar salida.
        SILFFunction newStateInvitation = new((values) =>
        {

            // Obtener el parámetro.
            var value = values.FirstOrDefault(t => t.Name == "id")?.Objeto.GetValue();

            if (value is not decimal)
                return;

            var id = (int)((value as decimal?) ?? 0);

            NotificationObserver.Delete(id);

        })
        // Propiedades
        {
            Name = "newStateInvitation",
            Parameters =
            [
                new("id", new("number"))
            ]
        };


        // Guardar métodos.
        Actions = [updateContacts, viewContact, addProduct, addInflow, addOutflow, newInvitation, newStateInvitation];

    }



    /// <summary>
    /// Evento al recibir un comando.
    /// </summary>
    /// <param name="e">Comando</param>
    private static void OnReceiveCommand(object? sender, CommandModel e)
    {

        // Generar la app.
        var app = new SILF.Script.App(e.Command);

        // Agregar funciones del framework de Inventory.
        app.AddDefaultFunctions(Actions);

        // Ejecutar app.
        app.Run();

    }



    /// <summary>
    /// Cerrar conexión.
    /// </summary>
    public static void Close()
    {
        DeviceKey = string.Empty;
        InventoryAccessHub?.Dispose();
        InventoryAccessHub = null;
    }


}