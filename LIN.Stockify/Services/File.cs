
namespace LIN.Services;

internal class File
{


    /// <summary>
    /// Carga la imagen de perfil
    /// </summary>
    public static async Task<byte[]> OpenImage()
    {

        // Carga el archivo
        var result = await FilePicker.Default.PickAsync();

        // analisa el resultado
        if (result == null)
            return [];


        // Extension del archivo
        if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) || result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
        {

            FileInfo dd = new(result.FullPath);
            var stream = dd.OpenRead();

            MemoryStream ms = new();
            stream.CopyTo(ms);
            var bytes = ms.ToArray();


            return bytes;


        }


        return [];

    }

}
