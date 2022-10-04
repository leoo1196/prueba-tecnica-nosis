namespace Core.Errors;

public class NotFoundError : Error
{
    public NotFoundError(string message) : base(message)
    {
    }

    /// <summary>
    /// Genera una instancia de NotFoundError con un mensaje generico para el tipo de dato especificado
    /// </summary>
    /// <typeparam name="Type">Tipo del recurso no encontrado</typeparam>
    /// <param name="invalidId">El Id que resulto ser invalido</param>
    /// <returns>Una instancia de NotFoundError</returns>
    public static NotFoundError For<Type>(object invalidId) where Type : class =>
        new($"No existe {nameof(Type)} con Id: {invalidId}");
}
