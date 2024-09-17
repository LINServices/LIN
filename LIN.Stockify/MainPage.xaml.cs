namespace LIN;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        Application.Current!.RequestedThemeChanged += (s, a) =>
        {
            MauiProgram.SetUIColors();
        };
    }



    /// <summary>
    /// Evento al abrir la app.
    /// </summary>
    protected override void OnAppearing()
    {
        // Establecer colores.
        MauiProgram.SetUIColors();
        base.OnAppearing();
    }
}
