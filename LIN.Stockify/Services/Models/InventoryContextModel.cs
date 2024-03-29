﻿namespace LIN.Services.Models;


public class InventoryContextModel
{

    /// <summary>
    /// Inventario.
    /// </summary>
    public InventoryDataModel Inventory { get; set; } = null!;


    /// <summary>
    /// Productos.
    /// </summary>
    public ReadAllResponse<ProductModel>? Products { get; set; } = null;


    /// <summary>
    /// Entradas.
    /// </summary>
    public ReadAllResponse<InflowDataModel>? Inflows { get; set; } = null;


    /// <summary>
    /// Salidas.
    /// </summary>
    public ReadAllResponse<OutflowDataModel>? Outflows { get; set; } = null;




    /// <summary>
    /// Encontrar un producto por Id.
    /// </summary>
    /// <param name="id">Id del producto.</param>
    public ProductModel? FindProduct(int id)
    {
        return Products?.Models.Where(t => t.Id == id).FirstOrDefault();
    }



}