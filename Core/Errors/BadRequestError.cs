namespace Core.Errors;

public class BadRequestError : Error
{
    public BadRequestError(string message) : base(message)
    {
    }
}
