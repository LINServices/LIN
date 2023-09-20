using LIN.Access.Inventory.Controllers;
using SILF.Script;
using SILF.Script.Elements;
using SILF.Script.Elements.Functions;
using SILF.Script.Interfaces;

namespace LIN.ScriptRuntime;


internal class Scripts
{

    public class SILFFunction : IFunction
    {
        public Tipo? Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Parameter> Parameters { get; set; } = new();


        Action<List<ParameterValue>> Action;

        public SILFFunction(Action<List<ParameterValue>> action)
        {
            this.Action = action;
        }



        public FuncContext Run(Instance instance, List<ParameterValue> values)
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

        // Llamada
        Actions.Add(new SILFFunction((values) =>
        {
            if (PhoneDialer.Default.IsSupported)
            {
                string number = values[0].Value.ToString();
                PhoneDialer.Default.Open(number);
            }

        })
        // Propiedades
        {
            Name = "call",
            Parameters = new()
                {
                    new("phone", new("string"))
                }
        });


        // Abrir Producto
        Actions.Add(new SILFFunction(async (values) =>
        {

            // Convierte el valor
            bool can = int.TryParse(values[0].Value.ToString(), out int id);

            if (!can)
                return;

            var model = await Access.Inventory.Controllers.Product.Read(id);

            var form = new UI.Views.Products.ViewItem(model.Model);

            form.Show();

        })
        // Propiedades
        {
            Name = "openPr",
            Parameters = new()
                {
                    new("id", new("number"))
                }
        });


        // Abrir Contactos
        Actions.Add(new SILFFunction(async (values) =>
        {
            var param = values[0].Value.ToString();

            // Convierte el valor
            bool can = int.TryParse(param, out int id);

            if (!can)
                return;

            var model = await Access.Inventory.Controllers.Contact.Read(id);


            var pop = new UI.Popups.ContactPopup(model.Model);

            await pop.Show();

        })
        // Propiedades
        {
            Name = "openCt",
            Parameters = new()
                {
                    new("id", new("number"))
                }
        });



        // Exportar entradas
        Actions.Add(new SILFFunction(async (values) =>
        {
            var param = values[0].Value.ToString();

            // Convierte el valor
            bool can = int.TryParse(param, out int id);

            if (!can)
                return;

            var model = await Inflows.Read(id);
            var form = new UI.Views.Inflows.ViewItem(model.Model, true);

            form.Show();

        })
        // Propiedades
        {
            Name = "exportInflow",
            Parameters = new()
                {
                    new("id", new("number"))
                }
        });



        // Exportar salidas
        Actions.Add(new SILFFunction(async (values) =>
        {
            var param = values[0].Value.ToString();

            // Convierte el valor
            bool can = int.TryParse(param, out int id);

            if (!can)
                return;


            var model = await Outflows.Read(id);
            var form = new UI.Views.Outflows.ViewItem(model.Model, true);

            form.Show();

        })
        // Propiedades
        {
            Name = "exportOutflow",
            Parameters = new()
                {
                    new("id", new("number"))
                }
        });


        // Abrir entradas
        Actions.Add(new SILFFunction(async (values) =>
        {
            var param = values[0].Value.ToString();

            // Convierte el valor
            bool can = int.TryParse(param, out int id);

            if (!can)
                return;


            var model = await Inflows.Read(id);
            var form = new UI.Views.Inflows.ViewItem(model.Model);

            form.Show();

        })
        // Propiedades
        {
            Name = "openIF",
            Parameters = new()
                {
                    new("id", new("number"))
                }
        });


        // Abrir salidas
        Actions.Add(new SILFFunction(async (values) =>
        {
            var param = values[0].Value.ToString();

            // Convierte el valor
            bool can = int.TryParse(param, out int id);

            if (!can)
                return;

            var model = await Outflows.Read(id);

            var form = new UI.Views.Outflows.ViewItem(model.Model);

            form.Show();

        })
        // Propiedades
        {
            Name = "openOF",
            Parameters = new()
                {
                    new("id", new("number"))
                }
        });


        // Mensaje
        Actions.Add(new SILFFunction(async (values) =>
        {
            var param = values[0].Value.ToString();

            try
            {

                if (AppShell.ActualPage == null)
                    return;

                await AppShell.ActualPage.DisplayAlert("Mensaje", param ?? "", "OK");
            }
            catch
            {

            }

        })
        // Propiedades
        {
            Name = "msg",
            Parameters = new()
                {
                    new("id", new("string"))
                }
        });



        // Mensaje
        Actions.Add(new SILFFunction((values) =>
        {
            Services.Login.Logout.Start();
        })
        // Propiedades
        {
            Name = "disconnect"
        });


        // Mensaje
        Actions.Add(new SILFFunction((values) =>
        {
            var param = values[0].Value.ToString();

            bool can = int.TryParse(param, out int id);

            if (!can)
                return;

            var form = new UI.Views.Inventorys.Informes(id);

            form.Show();
        })
        // Propiedades
        {
            Name = "openInformesScreen",
            Parameters = new()
                {
                    new("id", new("number"))
                }
        });

    }


}
