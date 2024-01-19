namespace Reporting.Models.Errors;

public class NotFoundTemplateFieldError : ErrorMessage
{
    public string FieldName { get; }

    public NotFoundTemplateFieldError(string fieldName) : base("Invalid template field")
    {
        FieldName = fieldName;
    }
}