using Reporting.Models.Errors;

namespace Reporting.Models;

public class BaseResponse
{
    public List<ErrorMessage>? ErrorMessages { get; set; }
}
