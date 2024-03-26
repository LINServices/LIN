﻿namespace LIN.Pages;


public partial class Account
{


    /// <summary>
    /// Cerrar sesión.
    /// </summary>
    async void Close()
    {

        // Cerrar la sesión.
        Session.CloseSession();

        // Eliminar cuentas.
        await new LIN.LocalDataBase.Data.UserDB().DeleteUsers();

        // Navegar al inicio.
        nav.NavigateTo("/", true, true);

    }



    /// <summary>
    /// Evento: Al inicializar.
    /// </summary>
    protected override void OnInitialized()
    {

        MainLayout.Configure(new()
        {
            OnCenterClick = () => { },
            Section = 3,
            DockIcon = 0
        });

        base.OnInitialized();
    }


}
