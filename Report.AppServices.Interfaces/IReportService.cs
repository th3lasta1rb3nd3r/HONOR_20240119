using Report.Models.Errors;

namespace AppServices.Interfaces;

public interface IReportService
{
    Task<string> GenerateReport(string json, string xmlTemplate);
    List<ErrorMessage> ValidateReportTemplate(string xmlTemplate);
}
