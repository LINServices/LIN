namespace LIN.Components.Layout;


public partial class EmmaDrawer
{


    /// <summary>
    /// ID del elemento Html.
    /// </summary>
    public string _id = $"element-{Guid.NewGuid()}";


    /// <summary>
    /// Abrir el elemento.
    /// </summary>
    public async void Show()
    {

        // Abrir el elemento.
        await JS.InvokeVoidAsync("ShowDrawer", _id, DotNetObjectReference.Create(this), $"btn-close-{_id}", "close-all-all");

    }

    LIN.Emma.UI.Emma DocEmma { get; set; }





    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender)
        {
         //   LIN.Emma.UI.Functions.LoadActions(Scripts.Actions);
            DocEmma.OnPromptRequire += DocEmma_OnPromptRequire;
        }

    }


    private void DocEmma_OnPromptRequire(object? sender, string e)
    {

        if (DocEmma != null)
            DocEmma.ResponseIA = Access.Inventory.Controllers.Profile.ToEmma(e, Access.Inventory.Session.Instance.AccountToken);
    }

}