using Report.Models.Errors;

namespace Report.WebApi.Errors;

public class NoFilesError : ErrorMessage
{
    public NoFilesError() : base("No files provided.")
    {

    }
}
