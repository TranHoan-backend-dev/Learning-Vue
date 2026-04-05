namespace DL.Exception;

public class NotFoundException : KeyNotFoundException
{
    public NotFoundException() : base("Resource not found.")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, System.Exception innerException)
        : base(message, innerException)
    {
    }
}