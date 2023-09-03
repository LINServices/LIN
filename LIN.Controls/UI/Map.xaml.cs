namespace LIN.Controls.UI;


public partial class Map : ContentView
{

    /// <summary>
    /// Llave de la API
    /// </summary>
    private static readonly string _apikey = "pk.GmCg87X5033IZg7VsL5RsBAd4L575nzwxoKb";


    /// <summary>
    /// Url base de LIN Maps
    /// </summary>
    private static string UrlBase => @"http://linmaps.somee.com/mapa/light/";



    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="lon">Longitud</param>
    /// <param name="lat">Latitud</param>
    public Map(double lon, double lat)
    {
        InitializeComponent();
        var url = @$"{UrlBase}{lon.ToString().Replace(',', '.')}/{lat.ToString().Replace(',', '.')}/{_apikey}";
        vista.Source = url;
    }




    /// <summary>
    /// Constructor
    /// </summary>
    public Map()
    {
        InitializeComponent();
        var url = @$"{UrlBase}{78.ToString().Replace(',', '.')}/{78.ToString().Replace(',', '.')}/{_apikey}";
        vista.Source = url;
    }




    /// <summary>
    /// Establece nuevas coordenadas
    /// </summary>
    /// <param name="lon">Longitud</param>
    /// <param name="lat">Latitud</param>
    public void SetNewCoordenadas(double lon, double lat)
    {
        var url = @$"{UrlBase}{lon.ToString().Replace(',', '.')}/{lat.ToString().Replace(',', '.')}/{_apikey}";
        vista.Source = url;
    }



    /// <summary>
    /// Navega a las coordenadas base
    /// </summary>
    public void NavigateToHome()
    {
        vista.EvaluateJavaScriptAsync("window.blazorInterop.goToHome()");
    }



    /// <summary>
    /// Centra el Mapa
    /// </summary>
    public void CenterMap()
    {
        vista.EvaluateJavaScriptAsync("window.blazorInterop.resetNort()");
    }



    bool l = false;
    private void vista_Loaded(object sender, EventArgs e)
    {

        if (!l)
        {
            vista.HeightRequest += vista.Height + 90;
            l = true;
        }

    }
}