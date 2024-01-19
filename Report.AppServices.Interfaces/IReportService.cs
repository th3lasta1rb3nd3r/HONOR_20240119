using Reporting.Models.Errors;

namespace AppServices.Interfaces;

public interface IReportService
{
    List<ErrorMessage> ValidateTemplate<T>(string xmlTemplate);
    Task<string> GenerateReport(string json, string xmlTemplate);
}
