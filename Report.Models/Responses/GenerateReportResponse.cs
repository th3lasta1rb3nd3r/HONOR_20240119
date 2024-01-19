namespace Report.Models.Responses;

public class GenerateReportResponse : BaseResponse
{
    public IEnumerable<ReportModel>? Reports { get; set; }
}
