namespace LIN.Components.Layout;


public partial class BottomNavigation
{


    /// <summary>
    /// Sección actual de la barra de navegación.
    /// </summary>
    [Parameter]
    public int Section { get; set; } = 0;


    /// <summary>
    /// Acción a realizar después de presionar en el botón central.
    /// </summary>
    [Parameter]
    public Action OnCenterClick { get; set; } = () => { };


    /// <summary>
    /// Elemento SVG del botón central.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }




    /// <summary>
    /// Ir a la sección.
    /// </summary>
    /// <param name="section">Numero de la sección.</param>
    void GoSection(int section)
    {

        if (section == 0)
            navigationManager.NavigateTo("/home");

        else if (section == 1)
            navigationManager.NavigateTo("/inventory");

        else if (section == 2)
            navigationManager.NavigateTo("/contacts");

        else if (section == 3)
            navigationManager.NavigateTo("/account");

    }


}