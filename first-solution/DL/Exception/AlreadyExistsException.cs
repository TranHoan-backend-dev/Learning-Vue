namespace DL.Exception;

public class AlreadyExistsException : System.Exception
{
    public AlreadyExistsException() : base("Resource already exists.")
    {
    }

    public AlreadyExistsException(string message) : base(message)
    {
    }

    public AlreadyExistsException(string message, System.Exception? innerException) : base(message, innerException)
    {
    }
}