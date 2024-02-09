namespace LIN.Services;


internal class PDFService
{

    /// <summary>
    /// Renderiza una entrada
    /// </summary>
    public static async Task RenderOutflow(OutflowDataModel outflow, string exportBy, string createBy, List<ProductModel> tranfers, string path)
    {

        using var stream = await FileSystem.OpenAppPackageFileAsync("plantilla.html");
        using var reader = new StreamReader(stream);

        var contents = reader.ReadToEnd();

        var html = contents ?? string.Empty;

        html = html.Replace("@titulo", "Salida: " + outflow.Type);
        html = html.Replace("@rows", BuildOutflowRows(outflow.Details, tranfers, out decimal total));
        html = html.Replace("@exportUser", exportBy);
        html = html.Replace("@creatorUser", createBy);
        html = html.Replace("@dateModel", outflow.Date.ToString("dd/MM/yyy HH:mm"));
        html = html.Replace("@dateExport", DateTime.Now.ToString("dd/MM/yyy HH:mm"));
        html = html.Replace("@total", total.ToString());
        html = html.Replace("@titleTotal", $"Total ({outflow.Type}):");

        // Consulta a LIN PDF
        //var response = await LIN.Access.Developer.Controllers.PDF.ConvertHTML(html);

        // Guarda el archivo
       // File.WriteAllBytes($"{path}\\salida_{outflow.Id}.pdf", response.File);

    }


    /// <summary>
    /// Construlle las columnas
    /// </summary>
    private static string BuildOutflowRows(List<OutflowDetailsDataModel> detalles, List<ProductModel> tranfers, out decimal totalFinal)
    {
        StringBuilder build = new();

        int count = 1;
        decimal total = 0;
        foreach (var detail in detalles)
        {

            //var price = tranfers.Where(T => T.DetailModel.Id == detail.ProductDetailId).FirstOrDefault()?.DetailModel.PrecioVenta - tranfers.Where(T => T.IDDetail == detail.ProductoDetail).FirstOrDefault()?.PrecioCompra;
            //var ganancia = (price * detail.Cantidad) ?? 0;

            //var row = $"""
            //           <tr>
            //             <th scope="row">{count}</th>
            //             <td>{tranfers.Where(T => T.IDDetail == detail.ProductoDetail).FirstOrDefault()?.Name}</td>
            //             <td>{detail.Cantidad}</td>
            //             <td>{ganancia}</td>
            //           </tr>
            //           """;
            //total += ganancia;
            //build.Append(row);
            //count++;
        }

        totalFinal = total;
        return build.ToString();

    }









    /// <summary>
    /// Renderiza una entrada
    /// </summary>
    public static async Task RenderInflow(InflowDataModel inflow, string exportBy, string createBy, List<ProductModel> tranfers, string path)
    {

        // Leer el archivo
        var fileUrl = "D:\\LIN Services\\Clientes\\LIN\\LIN\\Resources\\Raw\\plantilla.html";
        var html = File.ReadAllText(fileUrl) ?? string.Empty;

        // Remplaza los valores
        html = html.Replace("@titulo", "Entrada: " + inflow.Type);
        html = html.Replace("@rows", BuildInflowRows(inflow.Details, tranfers, out decimal total));
        html = html.Replace("@exportUser", exportBy);
        html = html.Replace("@creatorUser", createBy);
        html = html.Replace("@dateModel", inflow.Date.ToString("dd/MM/yyy HH:mm"));
        html = html.Replace("@dateExport", DateTime.Now.ToString("dd/MM/yyy HH:mm"));
        html = html.Replace("@total", total.ToString());
        html = html.Replace("@titleTotal", "Previcion de ganancias:");


        // Consulta a LIN PDF
      //  var response = await LIN.Access.Developer.Controllers.PDF.ConvertHTML(html);

        // Guarda el archivo
       // File.WriteAllBytes($"{path}\\entrada_{inflow.Id}.pdf", response.File);

    }


    /// <summary>
    /// Construlle las columnas
    /// </summary>
    private static string BuildInflowRows(List<InflowDetailsDataModel> detalles, List<ProductModel> tranfers, out decimal totalFinal)
    {
        StringBuilder build = new();

        int count = 1;
        decimal total = 0;
        foreach (var detail in detalles)
        {

            //var price = tranfers.Where(T => T.IDDetail == detail.ProductoDetail).FirstOrDefault()?.PrecioVenta - tranfers.Where(T => T.IDDetail == detail.ProductoDetail).FirstOrDefault()?.PrecioCompra;
            //var ganancia = (price * detail.Cantidad) ?? 0;
            //var row = $"""
            //           <tr>
            //             <th scope="row">{count}</th>
            //             <td>{tranfers.Where(T => T.IDDetail == detail.ProductoDetail).FirstOrDefault()?.Name}</td>
            //             <td>{detail.Cantidad}</td>
            //             <td>{ganancia}</td>
            //           </tr>
            //           """;
            //total += ganancia;
            //build.Append(row);
            //count++;
        }

        totalFinal = total;
        return build.ToString();

    }


}