using LIN.Inventory.Shared.Services.Models;
using LIN.Inventory.Shared.Services.Observers;

namespace LIN.Components.Layout;


public partial class DrawerProducts : IProduct, IDisposable
{

    /// <summary>
    /// ID del elemento Html.
    /// </summary>
    public string _id = $"element-{Guid.NewGuid()}";


    /// <summary>
    /// Resultado de búsqueda.
    /// </summary>
    private List<Types.Inventory.Models.ProductModel> Result = [];


    /// <summary>
    /// Patron de búsqueda.
    /// </summary>
    private string Pattern { get; set; } = string.Empty;


    /// <summary>
    /// Lista de perfiles seleccionados.
    /// </summary>
    [Parameter]
    public List<ProductModel> Selected { get; set; } = [];


    /// <summary>
    /// Evento al ocultar.
    /// </summary>
    [Parameter]
    public Action OnHide { get; set; } = () => { };


    /// <summary>
    /// Evento al mostrar.
    /// </summary>
    [Parameter]
    public Action OnShow { get; set; } = () => { };



    /// <summary>
    /// Evento al mostrar.
    /// </summary>
    [Parameter]
    public InventoryContextModel Contexto { get; set; } = null!;



    protected override void OnParametersSet()
    {
        Result = Contexto.Products?.Models ?? [];

        ProductObserver.Add(Contexto.Inventory.ID, this);

        base.OnParametersSet();
    }



    /// <summary>
    /// Buscar.
    /// </summary>
    /// <param name="e">evento.</param>
    public void Search(ChangeEventArgs e)
    {

        // Si es null o vacío.
        if (string.IsNullOrWhiteSpace(e.Value?.ToString()))
        {
            Result = Contexto.Products?.Models ?? [];
            StateHasChanged();
            return;
        }


        // Encuentra el usuario
        var products = Contexto.Products?.Models.Where(t => t.Name.Contains(e.Value!.ToString()!)).ToList();

        Result = products ?? [];
        StateHasChanged();
    }



    /// <summary>
    /// Abrir drawer.
    /// </summary>
    public async void Show()
    {
        await JS.InvokeVoidAsync("ShowDrawer", _id, DotNetObjectReference.Create(this), $"btn-close-panel-ide-{_id}");
    }



    /// <summary>
    /// Evento al ocultar.
    /// </summary>
    [JSInvokable("OnHide")]
    public void HideJS() => OnHide.Invoke();



    /// <summary>
    /// Evento al abrir.
    /// </summary>
    [JSInvokable("OnShow")]
    public void ShowJS() => OnShow.Invoke();



    /// <summary>
    /// Seleccionar un perfil.
    /// </summary>
    /// <param name="e">Perfil.</param>
    void Select(ProductModel e)
    {
        Selected.Add(e);
    }



    /// <summary>
    /// Deseleccionar un perfil.
    /// </summary>
    /// <param name="id">Id.</param>
    void UnSelect(int id)
    {
        Selected.RemoveAll(t => t.Id == id);
    }



    /// <summary>
    /// Controlador, Seleccionar / Deseleccionar.
    /// </summary>
    /// <param name="e">Perfil.</param>
    /// <param name="exist">Existe.</param>
    void SelectControl(ProductModel e, bool exist)
    {
        // Deseleccionar.
        if (exist)
            UnSelect(e.Id);

        // Seleccionar.
        else
            Select(e);

        // Notificar estado.
        StateHasChanged();

    }


    /// <summary>
    /// Renderizar.
    /// </summary>
    public void Render()
    {
        InvokeAsync(() =>
        {
            Result = Contexto.Products?.Models ?? [];

            // Eliminar de los seleccionados.
            var es = Contexto.Products?.Models.IntersectBy(Selected.Select(t => t.Id) ?? [], (T) => T.Id).ToList();

            Selected.Clear();
            Selected.AddRange(es ?? []);

            StateHasChanged();
        });
    }

    public void Dispose()
    {
        InvokeAsync(() =>
        {
            ProductObserver.Remove(this);
        });
    }
}