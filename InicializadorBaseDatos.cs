using System.Reflection;
using Microsoft.Data.Sqlite;

namespace PrestamoBiblioteca.Persistencia;

internal sealed class InicializadorBaseDatos
{
    public string RutaBaseDatos { get; } =
        Environment.GetEnvironmentVariable("PRESTAMO_BIBLIOTECA_DB")
        ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "UMG", "PrestamosBiblioteca", "biblioteca.db");

    public string CadenaConexion => new SqliteConnectionStringBuilder
    {
        DataSource = RutaBaseDatos,
        ForeignKeys = true
    }.ToString();

    public void Inicializar()
    {
        var directorio = Path.GetDirectoryName(RutaBaseDatos);
        if (!string.IsNullOrEmpty(directorio) && !Directory.Exists(directorio))
        {
            Directory.CreateDirectory(directorio);
        }

        using var conexion = new SqliteConnection(CadenaConexion);
        conexion.Open();
        using var comando = conexion.CreateCommand();
        comando.CommandText = LeerScriptTablas();
        comando.ExecuteNonQuery();
    }

    private static string LeerScriptTablas()
    {
        using var recurso = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("PrestamoBiblioteca.crear_tablas.sql")
            ?? throw new InvalidOperationException("No se encontro el recurso crear_tablas.sql.");
        using var lector = new StreamReader(recurso);
        return lector.ReadToEnd();
    }
}
