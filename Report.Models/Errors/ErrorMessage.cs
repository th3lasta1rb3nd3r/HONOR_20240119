namespace Report.Models.Errors;

public abstract class ErrorMessage
{
    public string Message { get; set; }

    protected ErrorMessage(string message)
    {
        Message = message;
    }
}