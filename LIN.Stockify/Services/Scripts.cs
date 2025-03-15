using LIN.Components.Pages;
using LIN.Components.Pages.Sections.Viewer;
using LIN.Inventory.Realtime.Manager;
using SILF.Script.DotnetRun.Interop;

namespace LIN.Services;

internal class Scripts
{

    [SILFFunctionName("updateCt")]
    private static void UpdateContacts() => Contactos.ToUpdate();

    [SILFFunctionName("viewContact")]
    private static async void ViewContact(decimal id)
    {

        // Obtener el contacto.
        ContactModel? contact = Contactos.Response?.Models.FirstOrDefault(t => t.Id == id);

        // Validar.
        if (contact is null)
        {
            // Obtener desde la web.
            var response = await Access.Inventory.Controllers.Contact.Read((int)id, Session.Instance.ContactsToken);

            // Error.
            if (response.Response != Responses.Success)
                return;

            // Establecer.
            contact = response.Model;
        }

        // Abrir el pop.
        MainLayout.ContactPop.Show(contact);
    }

    [SILFFunctionName("viewProduct")]
    private static async void ViewProduct(IServiceProvider provider, decimal inventory, decimal id)
    {

        var manager = provider.GetService<IInventoryManager>();

        var context = manager.Get((int)inventory);

        var find = context?.FindProduct((int)id);

        if (context == null || find == null)
        {
            var xx = await LIN.Access.Inventory.Controllers.Product.Read((int)id, Session.Instance.Token);

            if (xx.Response != Responses.Success)
                return;


            find = xx.Model;

        }

        LIN.Components.Pages.Sections.Products.Selected = find;
        Product.Show();
    }

    [SILFFunctionName("viewInflow")]
    private static void ViewInflow(decimal id)
    {
        Entrada.Show((int)id);
    }

    [SILFFunctionName("viewOutflow")]
    private static void ViewOutflow(decimal id)
    {
        Salida.Show((int)id);
    }

    [SILFFunctionName("addProduct")]
    private static async Task AddProduct(IServiceProvider provider, decimal id)
    {
        // Producto.
        var product = await LIN.Access.Inventory.Controllers.Product.Read((int)id, Session.Instance.Token);

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

        var observer = provider.GetService<IInventoryObserver>();

        observer.Update(context.Inventory.Id);
    }

    [SILFFunctionName("addInflow")]
    private static async Task AddInflow(IServiceProvider provider, decimal id)
    {

        // Producto.
        var inflow = await LIN.Access.Inventory.Controllers.Inflows.Read((int)id, Session.Instance.Token, true);

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
                product.DetailModel.Quantity += item.Quantity;

        }

        inflow.Model.CountDetails = inflow.Model.Details.Count;

        var pObserver = provider.GetService<IInventoryObserver>();
        var iObserver = provider.GetService<IInflowObserver>();

        pObserver.Update(context.Inventory.Id);
        iObserver.Update(context.Inventory.Id);
    }

    [SILFFunctionName("addOutflow")]
    private static async Task AddOutflow(IServiceProvider provider, decimal id)
    {

        // Producto.
        var outflow = await LIN.Access.Inventory.Controllers.Outflows.Read((int)id, Session.Instance.Token, true);

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
                product.DetailModel.Quantity -= item.Quantity;

        }

        outflow.Model.CountDetails = outflow.Model.Details.Count;

        var pObserver = provider.GetService<IInventoryObserver>();
        var oObserver = provider.GetService<IOutflowObserver>();

        pObserver.Update(context.Inventory.Id);
        oObserver.Update(context.Inventory.Id);
    }

    [SILFFunctionName("newInvitation")]
    private static async Task NewInvitation(IServiceProvider provider, decimal id)
    {
        // Modelo.
        var notification = await Access.Inventory.Controllers.InventoryAccess.ReadNotification((int)id, Session.Instance.Token);


        if (notification == null || notification.Response != Responses.Success)
            return;

        var manager = provider.GetService<INotificationObserver>();

        manager.Append(notification.Model);

    }

    [SILFFunctionName("newInvitation")]
    private static void NewStateInvitation(IServiceProvider provider, decimal id)
    {

        var manager = provider.GetService<INotificationObserver>();

        manager.Delete((int)id);
    }

    [SILFFunctionName("updateProduct")]
    private static async Task UpdateProduct(IServiceProvider provider, decimal id)
    {

        // Obtener el contacto.
        var x = await LIN.Access.Inventory.Controllers.Product.Read((int)id, Session.Instance.Token);

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
            exist.DetailModel.PurchasePrice = x.Model.DetailModel.PurchasePrice;
            exist.DetailModel.SalePrice = x.Model.DetailModel.SalePrice;
        }

        var pObserver = provider.GetService<IInventoryObserver>();


        pObserver.Update(exist.InventoryId);
    }

    [SILFFunctionName("deleteProduct")]
    private static void DeleteProduct(IServiceProvider provider, decimal id)
    {

        var manager = provider.GetService<IInventoryManager>();

        var context = manager.GetProduct((int)id);
        if (context == null)
            return;

        context.Statement = Types.Inventory.Enumerations.ProductBaseStatements.Deleted;

        var pObserver = provider.GetService<IInventoryObserver>();
        pObserver.Update(context.InventoryId);
    }

    public static List<Delegate> Get(IServiceProvider provider)
    {
        SILF.Script.App.LoadGlobalDI(provider);
        return [UpdateContacts, ViewContact, ViewProduct, ViewInflow, DeleteProduct, UpdateProduct, NewStateInvitation, NewInvitation, AddOutflow, AddInflow, AddProduct, ViewOutflow];
    }

}