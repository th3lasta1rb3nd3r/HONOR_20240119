namespace Reporting.Models.Errors;

public class GenericError : ErrorMessage
{
    public Exception Exception { get; }

    public GenericError(Exception exception) : base("Generic exception.")
    {
        Exception = exception;
    }
}