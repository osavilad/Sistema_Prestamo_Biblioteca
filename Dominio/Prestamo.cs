namespace PrestamoBiblioteca.Dominio;

internal sealed class Prestamo
{
    public Prestamo(int lectorId, int libroId, DateTime fechaPrestamo, DateTime fechaLimite, DateTime? fechaDevolucion = null)
    {
        if (lectorId <= 0) throw new ArgumentOutOfRangeException(nameof(lectorId));
        if (libroId <= 0) throw new ArgumentOutOfRangeException(nameof(libroId));
        if (fechaLimite.Date < fechaPrestamo.Date)
            throw new ArgumentException("La fecha limite no puede ser anterior al prestamo.", nameof(fechaLimite));
        if (fechaDevolucion.HasValue && fechaDevolucion.Value.Date < fechaPrestamo.Date)
            throw new ArgumentException("La fecha de devolucion no puede ser anterior al prestamo.", nameof(fechaDevolucion));

        LectorId = lectorId;
        LibroId = libroId;
        FechaPrestamo = fechaPrestamo.Date;
        FechaLimite = fechaLimite.Date;
        FechaDevolucion = fechaDevolucion?.Date;
    }

    public int LectorId { get; }
    public int LibroId { get; }
    public DateTime FechaPrestamo { get; }
    public DateTime FechaLimite { get; }
    public DateTime? FechaDevolucion { get; }
    public bool EstaActivo => !FechaDevolucion.HasValue;
}
