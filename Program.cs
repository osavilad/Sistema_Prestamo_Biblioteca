using PrestamoBiblioteca.Aplicacion;
using PrestamoBiblioteca.Persistencia;

namespace PrestamoBiblioteca;

internal static class Program
{
    private static void Main()
    {
        try
        {
            var inicializador = new InicializadorBaseDatos();
            inicializador.Inicializar();

            var repositorio = new RepositorioBiblioteca(
                inicializador.CadenaConexion);

            new MenuBiblioteca(repositorio).Ejecutar();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"No fue posible iniciar: {ex.Message}");
        }
    }
}
