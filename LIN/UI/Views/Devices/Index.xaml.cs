namespace LIN.UI.Views.Devices;

public partial class Index : ContentPage
{


    /// <summary>
    /// Modelo de un dispositivo
    /// </summary>
   // public DeviceModel Modelo { get; set; }



    /// <summary>
    /// Constructo
    /// </summary>
    //public Index(DeviceModel modelo)
    //{
    //    InitializeComponent();
    //    Disappearing += Index_Disappearing;
    //    Modelo = modelo;
    //    LoadModelVisible();

    //}



    /// <summary>
    /// Pagina desaparece
    /// </summary>
    private void Index_Disappearing(object? sender, EventArgs e)
    {
       
    }



    /// <summary>
    /// Renderiza la informacion de modelo
    /// </summary>
    public async void LoadModelVisible()
    {

        // Codigo de prueba para obtener el nombre de Wifi
        {
            string networkName = Connectivity.NetworkAccess.ToString();

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var profiles = Connectivity.ConnectionProfiles;
                if (profiles.Contains(ConnectionProfile.WiFi))
                {
                    var wifi = Connectivity.ConnectionProfiles.FirstOrDefault(p => p == ConnectionProfile.WiFi);
                    networkName = wifi.ToString();
                }
                else if (profiles.Contains(ConnectionProfile.Cellular))
                {
                    networkName = "Cellular Network";
                }
            }

            Console.WriteLine($"Current network name: {networkName}");

        }
















        //// Metadatos
        //lbName.Text = Modelo.Name;
        //cardModel.Contenido = $"{Modelo.Manufacter} {Modelo.Modelo}";
        //card1.Contenido = $"{Modelo.Platform} {Modelo.OsVersion}";
        //displayBateria.Text = $"{Modelo.BateryLevel}%";


        //// Nivel de Bateria y si esta cargando
        //cardBatery.Contenido = $"{Modelo.BateryLevel}% - {Modelo.BateryConected switch
        //{
        //    false => "Descargando",
        //    true => "Cargando"
        //}}";


        //// Imagen (Windows)
        //if (Modelo.Platform == Platforms.Windows)
        //{
        //    img.Source = ImageSource.FromFile("ordenador.png");
        //    card1.Source = ImageSource.FromFile("windows_logo.png");
        //}

        //// Imagen (Android)
        //else if (Modelo.Platform == Platforms.Android)
        //{
        //    img.Source = ImageSource.FromFile("telefono.png");
        //    card1.Source = ImageSource.FromFile("android_logo.png");
        //}

        //// Imagen (Web)
        //else if (Modelo.Platform == Platforms.Web)
        //    img.Source = ImageSource.FromFile("web.png");


        //// Establece el Mapa
        //mapa.SetNewCoordenadas(Modelo.Logitud, Modelo.Latitud);

        //// Name del lugar segun la ubicacion
        //var place = await Services.LocationService.GetPlace(Modelo.Logitud, Modelo.Latitud);

        //// Muetra la ubicacion
        //cardDireccion.Contenido = $"{place?.Thoroughfare} {place?.SubThoroughfare}";
        //cardStates.Contenido = $"{place?.Locality}, {place?.AdminArea}, {place?.CountryName}";

    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        mapa.NavigateToHome();
    }

    private void cardDireccion_Clicked(object sender, EventArgs e)
    {
        mapa.CenterMap();
        mapa.NavigateToHome();
    }
}