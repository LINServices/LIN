using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIN.Components.Shared;

public partial class ProfileControl
{

    /// <summary>
    /// Modelo.
    /// </summary>
    [Parameter]
    public LIN.Types.Cloud.Identity.Abstracts.SessionModel<LIN.Types.Inventory.Models.ProfileModel>? Model { get; set; }


    /// <summary>
    /// Evento al hacer click.
    /// </summary>
    [Parameter]
    public Action<LIN.Types.Cloud.Identity.Abstracts.SessionModel<LIN.Types.Inventory.Models.ProfileModel>?>? OnClick { get; set; }



    [Parameter]
    public bool State { get; set; }



    /// <summary>
    /// Enviar el evento.
    /// </summary>
    private void SendEvent()
    {
        OnClick?.Invoke(Model);
    }
}
