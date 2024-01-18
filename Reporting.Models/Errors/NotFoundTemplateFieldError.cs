using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reporting.Models.Errors;

public class NotFoundTemplateFieldError : ErrorMessage
{
    public string FieldName { get; }

    public NotFoundTemplateFieldError(string fieldName) : base("Invalid template field")
    {
        FieldName = fieldName;
    }
}


public abstract class ErrorMessage
{
    public string Message { get; set; }

    protected ErrorMessage(string message)
    {
        this.Message = message;
    }
}