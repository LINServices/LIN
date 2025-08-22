using SQLite;

namespace LIN.LocalDataBase.Data;

public class NoteDB
{

    /// <summary>
    /// Base de datos
    /// </summary>
    private SQLiteAsyncConnection? Database;


    /// <summary>
    /// Inicia la base de datos
    /// </summary>
    private async Task Init()
    {
        try
        {
            if (Database is not null)
                return;


            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

            var result = await Database.CreateTableAsync<Models.Note>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Exception en UserLocalDB: " + ex);
        }

    }


    /// <summary>
    /// Guarda una nota
    /// </summary>
    public async Task Save(Models.Note modelo)
    {
        await Init();
        await Delete();
        await Database!.InsertAsync(modelo);
    }


    /// <summary>
    /// Guarda una nota
    /// </summary>
    public async Task Append(Models.Note modelo)
    {
        await Init();
        await Database!.InsertAsync(modelo);
    }


    /// <summary>
    /// Guarda una nota
    /// </summary>
    public async Task Save(List<Models.Note> modelos)
    {
        await Init();
        await Delete();
        await Database!.InsertAllAsync(modelos);
    }


    /// <summary>
    /// Obtiene todas las notas
    /// </summary>
    public async Task<List<Models.Note>> Get()
    {
        await Init();
        return await Database!.Table<Models.Note>().OrderBy(t => t.Id).ToListAsync() ?? new();
    }


    /// <summary>
    /// Obtiene todas las notas
    /// </summary>
    public async Task<List<Models.Note>> GetUntrack()
    {
        await Init();
        return await Database!.Table<Models.Note>().Where(t => !t.IsConfirmed).OrderBy(t => t.Id).ToListAsync() ?? new();
    }


    /// <summary>
    /// Elimina todas las notas
    /// </summary>
    public async Task Delete()
    {
        await Init();

        // Elimina todas las notas
        await Database!.Table<Models.Note>().DeleteAsync(T => 1 == 1);
    }


    /// <summary>
    /// Elimina una nota
    /// </summary>
    public async Task Remove(int id)
    {
        await Init();

        // Elimina la nota
        await Database!.Table<Models.Note>().DeleteAsync(T => T.Id == id);
    }


    /// <summary>
    /// Marca una nota como eliminada y actualiza su estado de confirmación.
    /// </summary>
    public async Task DeleteOne(int id, bool isConfirmed)
    {
        await Init();

        // Marca la nota como eliminada
        await Database.QueryAsync<Models.Note>("UPDATE [Note] SET [IsDeleted] = ?, [IsConfirmed] = ? WHERE [Id] = ?", true, isConfirmed, id);
    }


    /// <summary>
    /// Actualiza una nota
    /// </summary>
    public async Task Update(Models.Note note)
    {
        await Init();
        await Database.QueryAsync<Models.Note>("UPDATE [Note] SET [Tittle] = ?, [Content] = ?, [Color] = ?, [IsConfirmed] = ? WHERE [Id] = ?", note.Tittle, note.Content, note.Color, note.IsConfirmed, note.Id);
    }

}