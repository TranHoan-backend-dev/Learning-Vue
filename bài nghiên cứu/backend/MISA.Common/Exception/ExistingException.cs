namespace MISA.Common.Exception;

public class ExistingException: System.Exception
{
    public ExistingException() : base("Resource already exists")
    {
    }

    public ExistingException(string message) : base(message)
    {
    }

    public ExistingException(string message, System.Exception? innerException) : base(message, innerException)
    {
    } 
}