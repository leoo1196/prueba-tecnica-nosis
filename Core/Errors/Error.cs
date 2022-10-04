namespace Core.Errors;

/// <summary>
/// Clase base utilizada para los errores del Core de la aplicación.
/// </summary>
public abstract class Error
{
    /// <summary>
    /// Mensaje informativo sobre el error producido.
    /// </summary>
    public virtual string Message { get; }

    protected Error(string message) => Message = message;
}
