﻿using LIN.Inventory.Shared.Services.Models;
using static System.Collections.Specialized.BitVector32;

namespace LIN.Components.Pages.Sections.New;


public partial class NewProduct
{


    /// <summary>
    /// Id.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;



    /// <summary>
    /// Modelo del producto.
    /// </summary>
    public ProductModel Product { get; set; } = new()
    {
        Details = [new()]
    };



    /// <summary>
    /// Imagen.
    /// </summary>
    public byte[] Photo { get; set; } = [];



    /// <summary>
    /// Categoría.
    /// </summary>
    public int Category { get; set; }



    /// <summary>
    /// Sección actual.
    /// </summary>
    private int Section { get; set; }



    /// <summary>
    /// Contexto del inventario.
    /// </summary>
    private InventoryContextModel? Contexto { get; set; }



    /// <summary>
    /// Evento al establecer los parámetros.
    /// </summary>
    protected override void OnParametersSet()
    {

        // Obtener el contexto.
        Contexto = LIN.Inventory.Shared. Services.InventoryContext.Get(int.Parse(Id));

        // Base.
        base.OnParametersSet();
    }


    string ErrorMessage = "";

    /// <summary>
    /// Crear.
    /// </summary>
    private async void Create()
    {
        try
        {

            Section = 3;
            StateHasChanged();

            //Product.Provider = 1;
            Product.InventoryId = Contexto?.Inventory.ID ?? 0;
            Product.Category = (ProductCategories)Category;
            Product.Statement = ProductBaseStatements.Normal;
            Product.Image = Photo;

            // Respuesta del controlador
            var response = await Access.Inventory.Controllers.Product.Create(Product, LIN.Access.Inventory.Session.Instance.Token);


            switch (response.Response)
            {

                case Responses.Success:
                    break;

                case Responses.Unauthorized:
                    Section = 2;
                    ErrorMessage = "No tienes autorización para crear productos en este inventario.";
                    StateHasChanged();
                    return;

                default:
                    Section = 2;
                    ErrorMessage = "Hubo un error al crear este producto.";
                    StateHasChanged();
                    return;
            }



            Section = 1;
            StateHasChanged();



            _ = Services.Realtime.InventoryAccessHub.SendCommand(new()
            {
                Command = $"addProduct({response.LastID})",
                Inventory = Contexto?.Inventory.ID ?? 0
            });


            await Task.Delay(3000);
            Section = 0;
            StateHasChanged();

        }
        catch (Exception)
        {
        }

    }



    void GoNormal()
    {
        Section = 0;
        StateHasChanged();
    }




}