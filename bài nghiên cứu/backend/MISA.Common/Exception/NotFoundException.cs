namespace MISA.Common.Exception;

public class NotFoundException: System.Exception
{
    protected NotFoundException() : base("Resource not found")
    {
    }

    protected NotFoundException(string message) : base(message)
    {
    }

    protected NotFoundException(string message, System.Exception? innerException) : base(message, innerException)
    {
    } 
}