using Report.Models.Errors;

namespace Report.Models;

public abstract class BaseResponse
{
    public List<ErrorMessage>? ErrorMessages { get; set; }
}
