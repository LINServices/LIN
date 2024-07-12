using LIN.Access.Inventory.Hubs;
using LIN.Components.Pages;
using LIN.Components.Pages.Sections;
using LIN.Components.Pages.Sections.Viewer;
using LIN.Inventory.Shared.Services.Runtime;

namespace LIN.Services;


internal class Realtime
{



    /// <summary>
    /// Id del dispositivo.
    /// </summary>
    public static string DeviceName { get => LIN.Inventory.Shared.Realtime.DeviceName; set => LIN.Inventory.Shared.Realtime.DeviceName = value; }
    public static string DeviceKey { get => LIN.Inventory.Shared.Realtime.DeviceKey; }
    public static string DevicePlatform { get => LIN.Inventory.Shared.Realtime.DevicePlatform; set => LIN.Inventory.Shared.Realtime.DevicePlatform = value; }


    public static InventoryAccessHub InventoryAccessHub { get => LIN.Inventory.Shared.Realtime.InventoryAccessHub; }



    /// <summary>
    /// Iniciar el servicio.
    /// </summary>
    public static void Start()
    {

        LIN.Inventory.Shared.Realtime.Start();

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



        // Visualizar un producto.
        SILFFunction viewProduct = new(async (values) =>
        {

            // Obtener el parámetro.
            var value = values.FirstOrDefault(t => t.Name == "id")?.Objeto.GetValue();

            // Validar el tipo.
            if (value is not decimal)
                return;

            // Id.
            var id = (int)((value as decimal?) ?? 0);


            // Obtener el parámetro.
            value = values.FirstOrDefault(t => t.Name == "inventory")?.Objeto.GetValue();

            // Validar el tipo.
            if (value is not decimal)
                return;

            // Id.
            var inventory = (int)((value as decimal?) ?? 0);



            var context = LIN.Inventory.Shared.Services.InventoryContext.Get(inventory);

            var find = context?.FindProduct(id);

            if (context == null || find == null)
            {
                var xx = await LIN.Access.Inventory.Controllers.Product.Read(id, Session.Instance.Token);

                if (xx.Response != Responses.Success)
                    return;


                find = xx.Model;

            }

            Products.Selected = find;

            Product.Show();





        })
        // Propiedades
        {
            Name = "viewProduct",
            Parameters =
            [
                new("inventory", new("number")),
                new("id", new("number")),
            ]
        };


        // Visualizar un inflow.
        SILFFunction viewInflow = new((values) =>
        {

            // Obtener el parámetro.
            var value = values.FirstOrDefault(t => t.Name == "id")?.Objeto.GetValue();

            // Validar el tipo.
            if (value is not decimal)
                return;

            // Id.
            var id = (int)((value as decimal?) ?? 0);


            Entrada.Show(id);

        })
        // Propiedades
        {
            Name = "viewInflow",
            Parameters =
            [
                new("id", new("number")),
            ]
        };



        // Visualizar un outflow.
        SILFFunction viewOutflow = new((values) =>
        {

            // Obtener el parámetro.
            var value = values.FirstOrDefault(t => t.Name == "id")?.Objeto.GetValue();

            // Validar el tipo.
            if (value is not decimal)
                return;

            // Id.
            var id = (int)((value as decimal?) ?? 0);


            Salida.Show(id);

        })
        // Propiedades
        {
            Name = "viewOutflow",
            Parameters =
            [
                new("id", new("number")),
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
            var inflow = await LIN.Access.Inventory.Controllers.Inflows.Read(id, Session.Instance.Token, true);

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

            inflow.Model.CountDetails = inflow.Model.Details.Count;

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
            var outflow = await LIN.Access.Inventory.Controllers.Outflows.Read(id, Session.Instance.Token, true);

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

            outflow.Model.CountDetails = outflow.Model.Details.Count;

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



        SILFFunction updateProduct = new(async (values) =>
        {

            // Obtener el parámetro.
            var value = values.FirstOrDefault(t => t.Name == "id")?.Objeto.GetValue();

            // Validar el tipo.
            if (value is not decimal)
                return;

            // Id.
            var id = (int)((value as decimal?) ?? 0);

            // Obtener el contacto.
            var x = await LIN.Access.Inventory.Controllers.Product.Read(id, Session.Instance.Token);


            var context = InventoryContext.Get(x.Model.InventoryId);


            var exist = context?.FindProduct(x.Model.Id);

            if (exist == null)
            {
                context?.Products?.Models.Add(x.Model);
                exist = x.Model;
            }
            else
            {
                exist.Image = x.Model.Image;
                exist.Category = x.Model.Category;
                exist.Code = x.Model.Code;
                exist.Description = x.Model.Description;
                exist.Name = x.Model.Name;
                exist.DetailModel.PrecioCompra = x.Model.DetailModel.PrecioCompra;
                exist.DetailModel.PrecioVenta = x.Model.DetailModel.PrecioVenta;
            }


            ProductObserver.Update(exist.InventoryId);


        })
        // Propiedades
        {
            Name = "updateProduct",
            Parameters =
            [
                new("id", new("number"))
            ]
        };



        // Guardar métodos.
        LIN.Inventory.Shared.Realtime.Build([updateContacts, viewContact, viewProduct, addProduct, addInflow, addOutflow, newInvitation, newStateInvitation, viewInflow, viewOutflow, updateProduct]);
    }



    /// <summary>
    /// Cerrar conexión.
    /// </summary>
    public static void Close()
    {
        LIN.Inventory.Shared.Realtime.Close();
    }


}