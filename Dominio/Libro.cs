namespace PrestamoBiblioteca.Dominio;

internal sealed class Libro
{
    public Libro(string codigo, string titulo, string autor, int ejemplares)
    {
        Codigo = ValidarTexto(codigo, nameof(codigo));
        Titulo = ValidarTexto(titulo, nameof(titulo));
        Autor = ValidarTexto(autor, nameof(autor));
        Ejemplares = ejemplares >= 0
            ? ejemplares
            : throw new ArgumentOutOfRangeException(nameof(ejemplares), "Los ejemplares no pueden ser negativos.");
    }

    public string Codigo { get; }
    public string Titulo { get; }
    public string Autor { get; }
    public int Ejemplares { get; }

    private static string ValidarTexto(string valor, string nombreParametro) =>
        string.IsNullOrWhiteSpace(valor)
            ? throw new ArgumentException("El valor no puede estar vacio.", nombreParametro)
            : valor.Trim();
}
