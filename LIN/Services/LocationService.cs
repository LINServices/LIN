namespace LIN.Services;


/// <summary>
/// Servicio de unicacion
/// </summary>
public class LocationService
{


    /// <summary>
    /// Calcula la distancia entre dos ubicaciones en KM
    /// </summary>
    public static double CalculateDistance(Location a, Location b)
    {
        // Distancia
        var distance = Location.CalculateDistance(a, b, DistanceUnits.Kilometers);

        return distance;
    }



    /// <summary>
    /// Obtiene la ubicacion actual
    /// </summary>
    public static async Task<Location> GetLocation()
    {

        Location result = new();


        try
        {
            // Solicitud
            GeolocationRequest request = new(GeolocationAccuracy.Best, TimeSpan.FromSeconds(60));


            // Obtiene la ubicacion
            Location? location = await Geolocation.Default.GetLocationAsync(request, new());


            // Ubicacion no obtenida
            if (location == null)
            {
                result = new();
                return new();
            }


            //var placemarks = (await Geocoding.Default.GetPlacemarksAsync(location.Latitude, location.Longitude)).ToList();

            //Placemark? placemark = placemarks.FirstOrDefault();

            //if (placemark != null)
            //{
            //    var ss =
            //        $"AdminArea:       {placemark.AdminArea}\n" +
            //        $"CountryCode:     {placemark.CountryCode}\n" +
            //        $"CountryName:     {placemark.CountryName}\n" +
            //        $"FeatureName:     {placemark.FeatureName}\n" +
            //        $"Locality:        {placemark.Locality}\n" +
            //        $"PostalCode:      {placemark.PostalCode}\n" +
            //        $"SubAdminArea:    {placemark.SubAdminArea}\n" +
            //        $"SubLocality:     {placemark.SubLocality}\n" +
            //        $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
            //        $"Thoroughfare:    {placemark.Thoroughfare}\n";

            //}

            result = location;
            _=Create(location.Longitude.ToString(), location.Latitude.ToString());
            return location;

        }
        catch (Exception ex)
        {

            if (ex.Message == "LocationWhenInUse permission was not granted: Denied" || ex.Message == "Location services are not enabled on device")
            {
                // Accion cuando no hay permisos
                await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            }


        }



        return result ?? new();
    }


    



    public async static Task Create(string longitude, string latitude)
    {

        // Variables
        var client = new HttpClient();

        string url = "http://linmapsapi.somee.com/api/profile/point";

        url = LIN.Modules.Web.AddParameters(url, new()
            {
                {"longitude",longitude },
                {"latitude",latitude },
                {"token", Session.Instance.AccountToken} }
        );

        try
        {
            // Contenido
            StringContent content = new("", System.Text.Encoding.UTF8, "application/json");

            // Envia la solicitud
            HttpResponseMessage response = await client.PostAsync(url, content);



        }
        catch
        {
        }

    }




    /// <summary>
    /// Obtiene la ubicacion actual
    /// </summary>
    public static async Task<Placemark?> GetPlace(double lon, double lat)
    {
        try
        {




            var placemarks = (await Geocoding.Default.GetPlacemarksAsync(lat, lon)).ToList();

            Placemark? placemark = placemarks.FirstOrDefault();

            if (placemark != null)
            {
                return placemark;
            }


        }
        catch
        {



        }



        return null;
    }



}
