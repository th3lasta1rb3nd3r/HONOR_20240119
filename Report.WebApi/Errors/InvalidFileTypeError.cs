using Report.Models.Errors;

namespace Report.WebApi.Errors;

public class InvalidFileTypeError : ErrorMessage
{
    public InvalidFileTypeError(string message) : base(message)
    {

    }
}
