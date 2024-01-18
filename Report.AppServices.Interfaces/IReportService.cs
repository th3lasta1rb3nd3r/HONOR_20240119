using Reporting.Models;
using Reporting.Models.Errors;
using Reporting.Models.Responses;

namespace AppServices.Interfaces;

public interface IReportService
{
    List<ErrorMessage> ValidateTemplate<T>(string xmlTemplate);
    Task<string> GenerateReport(string json, string xmlTemplate);
}
