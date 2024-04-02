namespace LIN.Components.Layout;


public partial class Chart
{


    /// <summary>
    /// Home DTO.
    /// </summary>
    [Parameter]
    public HomeDto? HomeDto { get; set; } = null;



    /// <summary>
    /// Ventas de esta semana.
    /// </summary>
    public string Value { get; set; } = "VILIES";



    /// <summary>
    /// Ventas de esta semana.
    /// </summary>
    public string Percent { get; set; } = "PERCENT%";



    /// <summary>
    /// Esta cargado.
    /// </summary>
    private bool IsLoad = false;



    /// <summary>
    /// Después de renderizar.
    /// </summary>
    /// <param name="firstRender">Primer renderizado.</param>
    protected override void OnAfterRender(bool firstRender)
    {

        if ((firstRender || !IsLoad) && HomeDto != null)
        {
            Value = FormatearNumero(HomeDto?.WeekSales?.Sum(t => t.Money) ?? 0);
            Percent = Calcular();
            _ = Render();
            IsLoad = true;
            StateHasChanged();
        }

        base.OnAfterRender(firstRender);
    }














































    public string Calcular()
    {
        try
        {

            if (HomeDto == null)
                return "nullo";

            return ((HomeDto.LastWeekSalesTotal - HomeDto.WeekSalesTotal) / HomeDto.WeekSalesTotal * 100).ToString("0.#");
        }
        catch (Exception ex)
        {
            var s = "";
        }
        return "error";
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


    public async Task Render()
    {
        try
        {
            // Validar.
            if (HomeDto == null || HomeDto.WeekSales.Count <= 0)
                return;

            // Invocar ApexChart.
            await js.InvokeVoidAsync("CharLoad", HomeDto.WeekSales.Select(t => t.Date.ToString()), HomeDto.WeekSales?.Select(t => t.Money));

        }
        catch (Exception) { }
    }



    public async void Set(HomeDto dto)
    {
        try
        {

            HomeDto = dto;
            StateHasChanged();
        }
        catch
        {
        }

    }



}