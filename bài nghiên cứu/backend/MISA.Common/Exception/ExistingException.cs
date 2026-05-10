namespace MISA.Common.Exception;

public class ExistingException: System.Exception
{
    protected ExistingException() : base("Resource already exists")
    {
    }

    protected ExistingException(string message) : base(message)
    {
    }

    protected ExistingException(string message, System.Exception? innerException) : base(message, innerException)
    {
    } 
}