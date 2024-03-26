namespace LIN.Components.Layout;


public partial class Chart
{


    List<SalesModel>? Sales = null;


    public async void GetData()
    {

        if (Sales != null)
        {
            Render();
            return;
        }

        var session = LIN.Access.Inventory.Session.Instance;
        var sales = await LIN.Access.Inventory.Controllers.Outflows.Sales(session.Informacion.ID, 7);

        Sales = sales.Models
            .GroupBy(s => DateOnly.FromDateTime(s.Date))
            .Select(group => new SalesModel
            {
                Date = new DateTime(group.Key.Year, group.Key.Month, group.Key.Day),
                Money = group.Sum(s => s.Money)
            }).ToList();


        var dateNow = DateOnly.FromDateTime(DateTime.Now);

        var dateOld = DateOnly.FromDateTime(Sales.LastOrDefault()?.Date ?? DateTime.Now);


        var diference = dateNow.ToDateTime(TimeOnly.MinValue) - dateOld.ToDateTime(TimeOnly.MinValue);

        if (diference.Days > 0)
        {

            for (var i = 1; i <= diference.Days; i++)
            {
                Sales.Add(new()
                {
                    Date = dateOld.AddDays(i).ToDateTime(TimeOnly.MinValue),
                    Money = 0
                });
            }

        }





        StateHasChanged();



        //await Task.Delay(1000);
        Render();
    }


    protected override void OnInitialized()
    {
        GetData();
        base.OnInitialized();
    }


    static string FormatearNumero(decimal numero)
    {
        // Definir los límites para las representaciones abreviadas
        const int mill = 1000000;
        const int kilo = 1000;

        // Verificar si el número es mayor a un millón
        if (numero >= mill)
        {
            // Representación abreviada en millones
            return $"{numero / (decimal)mill:F1}M";
        }
        // Verificar si el número es mayor a mil
        else if (numero >= kilo)
        {
            // Representación abreviada en miles
            return $"{numero / (decimal)kilo:F1}K";
        }
        // Si el número es menor a mil, no se realiza ninguna abreviatura
        else
        {
            return numero.ToString();
        }
    }


    public async void Render()
    {

        if (Sales?.Count > 0)
            await js.InvokeVoidAsync("CharLoad", Sales?.Select(t => t.Date.ToString()), Sales?.Select(t => t.Money));
    }


}