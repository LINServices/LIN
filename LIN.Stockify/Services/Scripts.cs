using LIN.Components.Pages;
using LIN.Components.Pages.Sections;
using LIN.Components.Pages.Sections.Viewer;
using LIN.Inventory.Realtime.Manager;
using LIN.Inventory.Realtime.Script;
using SILF.Script.Interfaces;

namespace LIN.Services;

internal class Scripts
{

    /// <summary>
    /// Construye las funciones.
    /// </summary>
    public static List<IFunction> Get(IServiceProvider provider)
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

            var manager = provider.GetService<IInventoryManager>();


            var context = manager.Get(inventory);

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

            var manager = provider.GetService<IInventoryManager>();

            // Contexto.
            var context = manager.Get(product.Model.InventoryId);

            // Si no se encontró.
            if (context == null)
                return;

            if (context.Products != null && context.Products.Response == Responses.Success)
                context.Products.Models.Add(product.Model);

            var observer = provider.GetService<IProductObserver>();

            observer.Update(context.Inventory.ID);


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

            var manager = provider.GetService<IInventoryManager>();


            // Contexto.
            var context = manager.Get(inflow.Model.InventoryId);

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

            var pObserver = provider.GetService<IProductObserver>();
            var iObserver = provider.GetService<IInflowObserver>();


            pObserver.Update(context.Inventory.ID);
            iObserver.Update(context.Inventory.ID);


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

            var manager = provider.GetService<IInventoryManager>();

            // Contexto.
            var context = manager.Get(outflow.Model.InventoryId);

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

            var pObserver = provider.GetService<IProductObserver>();
            var oObserver = provider.GetService<IOutflowObserver>();

            pObserver.Update(context.Inventory.ID);
            oObserver.Update(context.Inventory.ID);


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

            var manager = provider.GetService<INotificationObserver>();

            manager.Append(notification.Model);

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

            var manager = provider.GetService<INotificationObserver>();

            manager.Delete(id);

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

            var manager = provider.GetService<IInventoryManager>();

            var context = manager.Get(x.Model.InventoryId);


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

            var pObserver = provider.GetService<IProductObserver>();


            pObserver.Update(exist.InventoryId);


        })
        // Propiedades
        {
            Name = "updateProduct",
            Parameters =
            [
                new("id", new("number"))
            ]
        };



        SILFFunction deleteProduct = new((values) =>
        {

            // Obtener el parámetro.
            var value = values.FirstOrDefault(t => t.Name == "id")?.Objeto.GetValue();

            // Validar el tipo.
            if (value is not decimal)
                return;

            // Id.
            var id = (int)((value as decimal?) ?? 0);

            var manager = provider.GetService<IInventoryManager>();

            var context = manager.GetProduct(id);
            if (context == null)
                return;

            context.Statement = Types.Inventory.Enumerations.ProductBaseStatements.Deleted;

            var pObserver = provider.GetService<IProductObserver>();
            pObserver.Update(context.InventoryId);
        })
        // Propiedades
        {
            Name = "deleteProduct",
            Parameters =
        [
            new("id", new("number"))
        ]
        };

        // Guardar métodos.
        return [updateContacts, viewContact, viewProduct, addProduct, addInflow, addOutflow, newInvitation, newStateInvitation, viewInflow, viewOutflow, updateProduct, deleteProduct];
    }


}