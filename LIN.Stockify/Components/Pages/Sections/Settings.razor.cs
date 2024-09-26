﻿using LIN.Access.Inventory.Controllers;
using LIN.Inventory.Realtime.Manager.Models;
using LIN.Types.Inventory.Enumerations;

namespace LIN.Components.Pages.Sections;

public partial class Settings
{

    /// <summary>
    /// Parámetro Id.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = string.Empty;


    /// <summary>
    /// Drawer de integrantes.
    /// </summary>
    private DrawerPeople Drawer = null!;


    /// <summary>
    /// Popup del integrante.
    /// </summary>
    private MemberPopup MemberPopup { get; set; } = null!;


    /// <summary>
    /// Lista de participantes
    /// </summary>
    private readonly List<Types.Cloud.Identity.Abstracts.SessionModel<LIN.Types.Inventory.Models.ProfileModel>> Participantes = [];


    string Name = "";
    string Description = "";



    /// <summary>
    /// Esta cargando.
    /// </summary>
    private bool IsLoading = false;


    /// <summary>
    /// Lista de modelos.
    /// </summary>
    private ReadAllResponse<IntegrantDataModel>? Response { get; set; } = null;


    /// <summary>
    /// Contexto de inventario.
    /// </summary>
    InventoryContext? InventoryContext { get; set; }


    /// <summary>
    /// Evento al inicializar.
    /// </summary>
    protected override Task OnInitializedAsync()
    {
        MainLayout.Configure(new()
        {
            OnCenterClick = Save,
            Section = 1,
            DockIcon = 3
        });

        Reload();
        return base.OnInitializedAsync();
    }


    /// <summary>
    /// Operación de cargar.
    /// </summary>
    public async void Reload()
    {

        // Obtener contexto.
        InventoryContext = InventoryManager.Get(int.Parse(Id));

        // Validar contexto.
        if (InventoryContext == null)
            return;

        // Obtener información.
        Name = InventoryContext.Inventory?.Nombre ?? string.Empty;
        Description = InventoryContext.Inventory?.Direction ?? string.Empty;

        // Rellena los datos
        await RetrieveData();

        StateHasChanged();
    }


    /// <summary>
    /// Obtiene información desde el servidor
    /// </summary>
    private async Task RetrieveData(bool force = false)
    {

        // Validación.
        if ((!force && (Response != null)) || IsLoading)
            return;

        IsLoading = true;
        StateHasChanged();

        var response = await InventoryAccess.GetMembers(int.Parse(Id), Session.Instance.Token, Session.Instance.AccountToken);

        IsLoading = false;
        StateHasChanged();

        // Rellena los items
        Response = response;
        StateHasChanged();

    }


    /// <summary>
    /// Guardar los datos.
    /// </summary>
    private async void Save()
    {

        // Si hay nuevos integrantes.
        if (Participantes.Count > 0)
            await SaveParticipants();

        // Validar campos.
        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description))
            return;

        // Enviar respuesta.
        var response = await LIN.Access.Inventory.Controllers.Inventories.Update(int.Parse(Id), Name, Description, Session.Instance.Token);

        // Validar respuesta.
        if (InventoryContext == null || response.Response != Responses.Success || InventoryContext.Inventory == null)
            return;

        // Actualización.
        InventoryContext.Inventory.Nombre = Name;
        InventoryContext.Inventory.Direction = Description;
        StateHasChanged();
    }


    /// <summary>
    /// Guardar los nuevos integrantes.
    /// </summary>
    private async Task SaveParticipants()
    {

        foreach (var e in Participantes)
        {
            var model = new InventoryAcessDataModel
            {
                Inventario = int.Parse(Id),
                ProfileID = e.Profile.ID,
                Rol = InventoryRoles.Member
            };
            await LIN.Access.Inventory.Controllers.InventoryAccess.Create(model, Session.Instance.Token);
        }
    }


    /// <summary>
    /// Abrir modal de integrantes.
    /// </summary>
    /// <param name="member"></param>
    private void OpenMember(IntegrantDataModel member)
    {
        MemberPopup.Show(member);
    }


    /// <summary>
    /// Al eliminar un integrante.
    /// </summary>
    /// <param name="id">Id del integrante.</param>
    private void OnDelete(int id)
    {
        this.InvokeAsync(() =>
        {
            Response?.Models.RemoveAll(t => t.AccessID == id);
            StateHasChanged();
        });
    }

}