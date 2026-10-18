namespace PrestamoBiblioteca.Dominio;

internal abstract class Persona
{
    protected Persona(string codigo, string nombre, string correo, string telefono)
    {
        Codigo = ValidarTexto(codigo, nameof(codigo));
        Nombre = ValidarTexto(nombre, nameof(nombre));
        Correo = ValidarTexto(correo, nameof(correo));
        Telefono = ValidarTexto(telefono, nameof(telefono));
    }

    public string Codigo { get; }
    public string Nombre { get; }
    public string Correo { get; }
    public string Telefono { get; }

    public virtual string ObtenerInformacion() => $"{Codigo} - {Nombre} ({Correo}, {Telefono})";

    private static string ValidarTexto(string valor, string nombreParametro) =>
        string.IsNullOrWhiteSpace(valor)
            ? throw new ArgumentException("El valor no puede estar vacio.", nombreParametro)
            : valor.Trim();
}
