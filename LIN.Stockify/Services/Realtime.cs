using LIN.Access.Inventory.Hubs;
using LIN.Pages;
using SILF.Script;
using SILF.Script.Elements.Functions;
using SILF.Script.Enums;
using SILF.Script.Interfaces;

namespace LIN.Services;

public class Realtime
{


    public static string DeviceName { get; set; }

    public class SILFFunction : IFunction
    {
        public Tipo? Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Parameter> Parameters { get; set; } = new();


        Action<List<SILF.Script.Elements.ParameterValue>> Action;

        public SILFFunction(Action<List<SILF.Script.Elements.ParameterValue>> action)
        {
            this.Action = action;
        }

        public FuncContext Run(Instance instance, List<SILF.Script.Elements.ParameterValue> values)
        {
            Action.Invoke(values);
            return new();
        }
    }


    /// <summary>
    /// Funciones
    /// </summary>
    public static List<IFunction> Actions { get; set; } = new();





    /// <summary>
    /// Construye las funciones
    /// </summary>
    public static void Build()
    {

        // Ve
        Actions.Add(new SILFFunction((values) =>
        {
            Contactos.ToUpdate();
        })
        // Propiedades
        {
            Name = "updateCt",
            Parameters = new()
            {
            }
        });


        // Ver contacto.
        Actions.Add(new SILFFunction(async (values) =>
        {

            // Obtener el parámetro.
            bool can = int.TryParse((string)values.LastOrDefault(t => t.Name == "id")!.Value ?? "", out int id);

            // Error al convertir.
            if (!can)
                return;

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
            LIN.Components.Layout.MainLayout.ContactPop.Show(contact);

        })
        // Propiedades
        {
            Name = "viewContact",
            Parameters = new()
            {
                new("id", new("number"))
            }
        });


    }



    public static InventoryAccessHub InventoryAccess { get; set; }


    public static void Start()
    {
        if (InventoryAccess != null)
            return;

        InventoryAccess = new(LIN.Access.Inventory.Session.Instance.Token, 0, new()
        {
            Name = DeviceName,
            Platform = MauiProgram.GetPlatform()
        });

        InventoryAccess.On += InventoryAccess_On;

    }

    private static void InventoryAccess_On(object? sender, Types.Inventory.Transient.CommandModel e)
    {
        var app = new SILF.Script.App(e.Command, new Con());

        app.AddDefaultFunctions(Actions);

        app.Run();
    }

}


class Con : IConsole
{
    public void InsertLine(string result, LogLevel logLevel)
    {
    }
}