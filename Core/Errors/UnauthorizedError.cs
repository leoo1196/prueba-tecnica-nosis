namespace Core.Errors;

public class UnauthorizedError : Error
{
    public UnauthorizedError(string message) : base(message)
    {
    }
}
